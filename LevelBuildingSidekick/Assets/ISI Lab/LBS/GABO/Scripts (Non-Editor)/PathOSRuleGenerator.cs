using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using Unity.AI.Navigation;
using PathOS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using ISILab.Extensions;

namespace ISILab.LBS.Generators
{
    public class PathOSRuleGenerator : LBSGeneratorRule
    {
        #region FIELDS
        PathOSModule module;
        List<PathOSTile> tiles;
        GameObject parent;
        GameObject levelMarkupContainer; // Contiene objetos marcados de PathOS
        GameObject wallsContainer; // Contiene los objetos muro
        Vector2 scale;
        // Referencias a muros colocados en ultima generacion
        List<(PathOSTile, GameObject)> lastGenerationWalls;
        // Referencia al PathOSManager instanciado
        PathOSManager manager;
        // Ventana de PathOS+
        PathOSWindow window;
        // PathOS+ Prefabs originales
        [System.NonSerialized]
        GameObject agentPrefab;
        [System.NonSerialized]
        GameObject managerPrefab;
        [System.NonSerialized]
        GameObject worldCameraPrefab;
        [System.NonSerialized]
        GameObject screenshotCameraPrefab;
        // Instancias de los prefabs
        GameObject agentGameObject;
        GameObject managerGameObject;
        GameObject worldCameraGameObject;
        GameObject screenshotCameraGameObject;
        // Otros prefabs
        GameObject elementPrefab;
        GameObject wallPrefab;
        #endregion

        #region METHODS
        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new System.NotImplementedException(); // GABO TODO: Implementar? (Proyecto no la usa todavia)
        }

        public override object Clone()
        {
            return new PathOSRuleGenerator();
        }

        // GABO TODO: TERMINARR
        public override Tuple<GameObject, string> Generate(LBSLayer layer, Generator3D.Settings settings)
        {
#if UNITY_EDITOR
            PathOSStorage storage = PathOSStorage.Instance;
            // Prefabs
            agentPrefab = storage.agentPrefab.gameObject;
            managerPrefab = storage.managerPrefab.gameObject;
            worldCameraPrefab = storage.worldCameraPrefab.gameObject;
            screenshotCameraPrefab = storage.screenshotCameraPrefab.gameObject;
            elementPrefab = Resources.Load<GameObject>("Prefabs/ElementPrefab");
            wallPrefab = Resources.Load<GameObject>("Prefabs/WallPrefab");
            // Variables
            module = layer.GetModule<PathOSModule>();
            tiles = module.GetTiles();
            scale = settings.scale;
            // Se reinicia lista de muros antigua
            lastGenerationWalls = new();
            // Obtiene (o crea) instancia de PathOSWindow
            window = EditorWindow.GetWindow(typeof(PathOSWindow), false, "PathOS+", false) as PathOSWindow;
            // Objeto contenedor padre
            parent = new GameObject("PathOS+");
            // Objeto hijo, contenedor de los objetos etiquetados
            levelMarkupContainer = new GameObject("Level Markup");
            levelMarkupContainer.transform.SetParent(parent.transform);
            // 2do Objeto hijo, contenedor de los muros
            wallsContainer = new GameObject("Walls");
            wallsContainer.transform.SetParent(parent.transform);
            // Referencia temporal a entidades creadas con su tile correspondiente
            // (ej.: Para agregarles sus obstaculos dinamicos post-instanciacion)
            List<(PathOSTile, LevelEntity)> entitiesTemporaryReference = new();
            // Lista de GameObjects a retornar
            List<GameObject> boxes = new List<GameObject>();

            // Instanciacion de Prefabs + colocacion de referencias en PathOSWindow
            GenerateManager();
            GenerateWorldCamera();
            GenerateScreenshotCamera();

            // Instanciacion de tags
            manager = managerGameObject.GetComponent<PathOSManager>();
            foreach (PathOSTile tile in tiles)
            {
                // Si el tile es de agente, instanciar prefab.
                GameObject currInstance;
                if (tile.Tag.Label == "Player")//"PathOSAgent")
                {
                    // Instanciar agente
                    agentGameObject = PrefabUtility.InstantiatePrefab(agentPrefab, parent.transform) as GameObject;
                    currInstance = agentGameObject;
                    // Setear posicion
                    currInstance.transform.position = settings.position +
                                                      new Vector3(tile.X * scale.x, 0, tile.Y * scale.y)
                                                      - new Vector3(scale.x, 0, scale.y) / 2f;
                    // Se asigna a su campo respectivo de PathOSWindow
                    window.SetAgentReference(agentGameObject.GetComponent<PathOSAgent>());
                    // Terminar este ciclo para evitar errores
                    continue;
                }
                // Si el tile es Wall, instanciar muro.
                else if (tile.Tag.Label == "Wall")
                {
                    currInstance = PrefabUtility.InstantiatePrefab(wallPrefab, wallsContainer.transform) as GameObject;
                    // Se modifican dimensiones segun la escala actual (la altura del muro es equivalente a la primera dimension X,
                    // de manera de calcular correctamente el NavMesh)
                    currInstance.transform.localScale = new Vector3(scale.x, scale.x, scale.y);
                    // Agrega y setea modificador de NavMesh
                    NavMeshModifier modifier = currInstance.AddComponent<NavMeshModifier>();
                    modifier.ignoreFromBuild = false;
                    modifier.overrideArea = true;
                    modifier.area = NavMesh.GetAreaFromName("Not Walkable");
                    // Guarda referencias para su uso durante el Baking
                    lastGenerationWalls.Add((tile, currInstance)); 
                }
                else
                {
                    // Instanciar prefab
                    currInstance = PrefabUtility.InstantiatePrefab(elementPrefab, levelMarkupContainer.transform) as GameObject;
                }

                // Agregar icono del Tag asociado a este tile como textura al cubo
                MeshRenderer currRenderer = currInstance.GetComponentInChildren<MeshRenderer>();
                Material originalMaterial = currRenderer.sharedMaterial;
                Material currMaterial = new Material(originalMaterial);
                //currMaterial.SetTexture("_MainTex", tile.Tag.Icon);
                currRenderer.material = currMaterial;

                // Setear posicion
                currInstance.transform.position = settings.position +
                                                  new Vector3(tile.X * scale.x, 0, tile.Y * scale.y)
                                                  - new Vector3(scale.x, 0, scale.y) / 2f;

                // Crear entidades de marcado de PathOS y guardarlas temporalmente para su posterior manipulacion
                if (tile.Tag.Label != "Wall")
                {
                    entitiesTemporaryReference.Add((tile, manager.AddLevelEntity(currInstance, tile.Tag.EntityType)));
                }
            }

            // OBSTACULOS DINAMICOS: Crear y agregar obstaculos dinamicos para cada entidad recien creada (si le corresponde)
            foreach (var entityPair in entitiesTemporaryReference)
            {
                if (entityPair.Item1.IsDynamicObstacleTrigger)
                {
                    foreach (var obstaclePair in entityPair.Item1.GetObstacles())
                    {
                        if (obstaclePair.Item1.Tag.Label != "Wall")
                        {
                            var otherEntityPair = entitiesTemporaryReference.Find(otherPair => otherPair.Item1 == obstaclePair.Item1);
                            entityPair.Item2.dynamicObstacles.Add(new EntityObstaclePair(otherEntityPair.Item2.objectRef, obstaclePair.Item2));
                        }
                        else
                        {
                            var wallPair = lastGenerationWalls.Find(wallPair => wallPair.Item1 == obstaclePair.Item1);
                            entityPair.Item2.dynamicObstacles.Add(new EntityObstaclePair(wallPair.Item2, obstaclePair.Item2));
                        }
                    }
                }
            }

            // BAKING AUTOMATICO (asume que ya se encuentran instanciados los objetos de los otros modulos,
            // y que estos tienen colliders)
            GenerateNavMesh();

            // Ya unidos los objetos hijos con padre, trasladar segun Settings
            // GABO TODO: No es esto un error? Basado en PopulationRuleGenerator (todos los modulos base lo hacen)
            parent.transform.position += settings.position;

            return new Tuple<GameObject, string>(parent, "");
#else
                Debug.LogError("Attempting to use PathOSRuleGenerator class outside of Editor!"); return null;
#endif
        }

        private void GenerateManager()
        {
            managerGameObject = PrefabUtility.InstantiatePrefab(managerPrefab, parent.transform) as GameObject;
            // Se asigna a su campo respectivo de PathOSWindow
            window.SetManagerReference(managerGameObject.GetComponent<PathOSManager>());
        }

        private void GenerateWorldCamera()
        {
            worldCameraGameObject = PrefabUtility.InstantiatePrefab(worldCameraPrefab, parent.transform) as GameObject;
        }

        private void GenerateScreenshotCamera()
        {
            screenshotCameraGameObject = PrefabUtility.InstantiatePrefab(screenshotCameraPrefab, parent.transform) as GameObject;
            // Se asigna a su campo respectivo de PathOSWindow
            window.SetScreenshotCameraReference(screenshotCameraGameObject.GetComponent<ScreenshotManager>());
        }

        private void GenerateNavMesh()
        {
            // Si existe un NavMesh, evita generar otro.
            //NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            //if (triangulation.vertices.Length == 0 && triangulation.indices.Length == 0)
            //{
            //    Debug.LogWarning("Ya existe un NavMesh! No se generara uno nuevo.");
            //    return;
            //}

            // Interior Layers: GameObjects
            List<GameObject> interiorLayerGameObjects = 
                //GameObject.FindObjectsOfType<GameObject>()
                GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(
                obj => obj.transform.childCount == 2 &&
                obj.transform?.GetChild(0).name == "Schema" &&
                obj.transform?.GetChild(1).name == "Schema outside").ToList();

            // Exterior Layers: GameObjects
            List<GameObject> exteriorLayerGameObjects =
                //GameObject.FindObjectsOfType<GameObject>()
                GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(
                obj => obj.transform.childCount == 1 &&
                obj.transform?.GetChild(0).name == "Exterior").ToList();

            // Si no se encuentra, advierte.
            if (interiorLayerGameObjects.Count == 0 && exteriorLayerGameObjects.Count == 0)
            {
                Debug.LogWarning("Ninguna instancia de Exterior o Interior Layer encontrada. No se generara un NavMesh.");
                return;
            }

            // Crea padre temporal para usarlos en un solo mesh
            GameObject tempParent = new GameObject();
            tempParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // NavMeshSurface: Agrega componente de superficie y limita su efecto a los hijos de tempParent.
            // NOTA***: Producto de la potencial existencia de meshes sin acceso de lectura, usamos, en vez, los colliders.
            // Asi se evitan excepciones al realizar "re-Bakes" en Play Mode (ej.: Con obstaculos dinamicos)
            var surface = tempParent.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            // Arreglos donde guardar padres viejos para la posterior reinsertacion
            GameObject[] interiorOldParents = new GameObject[interiorLayerGameObjects.Count];
            GameObject[] exteriorOldParents = new GameObject[exteriorLayerGameObjects.Count];
            GameObject[] wallsOldParents = new GameObject[lastGenerationWalls.Count];

            // Interior Layers: Nuevo padre temporal
            for (int i = 0; i < interiorLayerGameObjects.Count; i++)
            {
                interiorOldParents[i] = interiorLayerGameObjects[i].transform.parent?.gameObject;
                // Agrega objetos al padre temporal
                interiorLayerGameObjects[i].transform.parent = tempParent.transform;
            }
            // Exterior Layers: Nuevo padre temporal
            for (int i = 0; i < exteriorLayerGameObjects.Count; i++)
            {
                exteriorOldParents[i] = exteriorLayerGameObjects[i].transform.parent?.gameObject;
                // Agrega objetos al padre temporal
                exteriorLayerGameObjects[i].transform.parent = tempParent.transform;
            }
            // Muros: Nuevo padre temporal
            for (int i = 0; i < lastGenerationWalls.Count; i++)
            {
                wallsOldParents[i] = lastGenerationWalls[i].Item2.transform.parent?.gameObject;
                // Agrega objetos al padre temporal
                lastGenerationWalls[i].Item2.transform.parent = tempParent.transform;
            }

            // Si un objeto recolectado no tiene un Collider, pero renderiza (MeshRenderer), se
            // le asigna temporalmente un BoxCollider.
            // ***Los objetos del modulo de exteriores, por defecto, no vienen con Colliders.
            // ***Para ello se usa esta seccion (2024-12-16).
            var doNotHaveColliderList = new List<GameObject>();
            var totalList = new List<GameObject>();
            totalList.AddRange(interiorLayerGameObjects);
            totalList.AddRange(exteriorLayerGameObjects);
            //Debug.Log("TOTAL GAMEOBJECTS: " + totalList.Count);
            foreach (var obj in totalList)
            {
                if (obj.GetComponentsInChildren<MeshRenderer>().Length > 0 && obj.GetComponentsInChildren<Collider>().Length == 0)
                {
                    var currMeshPlusChildren = obj.GetComponentsInChildren<MeshRenderer>();
                    foreach(var mesh in currMeshPlusChildren)
                    {
                        mesh.gameObject.AddComponent<BoxCollider>();
                        doNotHaveColliderList.Add(mesh.gameObject);
                    }
                }
            }

            // Genera NavMesh (Bake)
            surface.BuildNavMesh();

            // Remover colliders temporales
            int meshCount = doNotHaveColliderList.Count;
            //Debug.Log("CREATED COLLIDERS: " +  meshCount);
            for (int i = 0; i < meshCount; i++)
            {
                GameObject.DestroyImmediate(doNotHaveColliderList[i].GetComponent<BoxCollider>());
            }

            // Interior Layers: Reasigna padre original
            for (int i = 0; i < interiorLayerGameObjects.Count; i++)
            {
                interiorLayerGameObjects[i].transform.parent = interiorOldParents[i]?.transform;
            }
            // Exterior Layers: Reasigna padre original
            for (int i = 0; i < exteriorLayerGameObjects.Count; i++)
            {
                exteriorLayerGameObjects[i].transform.parent = exteriorOldParents[i]?.transform;
            }
            // Muros: Reasigna padre original
            for (int i = 0; i < lastGenerationWalls.Count; i++)
            {
                lastGenerationWalls[i].Item2.transform.parent = wallsOldParents[i]?.transform;
            }

            // Padre temporal cambia de nombre (pasa a contener unicamente el navmesh)
            tempParent.name = "NavMeshSurface";

            tempParent.SetParent(parent);
        }

        // [GABO DEBUG] Generador de prueba hecho originalmente para probar colocacion de elementos
        private GameObject SimpleBoxGenerate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Variables
            module = layer.GetModule<PathOSModule>();
            tiles = module.GetTiles();
            scale = settings.scale;
            // Objeto contenedor padre
            parent = new GameObject("PathOS+ Tags");
            // Prefab
            elementPrefab = Resources.Load<GameObject>("Prefabs/BoxWithTexture");
            // GameObject List
            List<GameObject> boxes = new List<GameObject>();

            foreach (PathOSTile tile in tiles)
            {
                // Instanciar prefab
#if UNITY_EDITOR
                GameObject currInstance = PrefabUtility.InstantiatePrefab(elementPrefab) as GameObject;
#else
                Debug.LogError("Attempting to use PathOSRuleGenerator class outside of Editor!"); return null;
#endif
                // Agregar icono del Tag asociado a este tile como textura al cubo
                MeshRenderer currRenderer = currInstance.GetComponentInChildren<MeshRenderer>();
                Material originalMaterial = currRenderer.sharedMaterial;
                Material currMaterial = new Material(originalMaterial);
                //currMaterial.SetTexture("_MainTex", tile.Tag.Icon);
                currRenderer.material = currMaterial;

                // Setear posicion
                currInstance.transform.position = settings.position +
                                                  new Vector3(tile.X * scale.x, 0, tile.Y * scale.y)
                                                  - new Vector3(scale.x, 0, scale.y) / 2f; // GABO TODO: Necesario ??? Basado en PopulationRuleGenerator.
                boxes.Add(currInstance);
            }

            if (boxes.Count > 0)
            {
                // Obtener posicion planar promedio de las cajas, y altura del objeto mas bajo.
                var x = boxes.Average(o => o.transform.position.x);
                var y = boxes.Min(o => o.transform.position.y);
                var z = boxes.Average(o => o.transform.position.z);
                // Asignar esta posicion al objeto contenedor padre
                parent.transform.position = new Vector3(x, y, z);
            }

            foreach (var box in boxes)
            {
                box.transform.parent = parent.transform;
            }

            // Ya unidos los objetos hijos con padre, trasladar segun Settings
            // GABO TODO: No es esto un error? Basado en PopulationRuleGenerator.
            parent.transform.position += settings.position;

            return parent;
        }
        #endregion
    }
}
