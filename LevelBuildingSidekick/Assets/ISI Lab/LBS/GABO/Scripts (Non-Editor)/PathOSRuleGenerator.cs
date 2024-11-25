using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace ISILab.LBS.Generators
{
    public class PathOSRuleGenerator : LBSGeneratorRule
    {
        #region FIELDS
        PathOSModule module;
        List<PathOSTile> tiles;
        GameObject parent;
        GameObject levelMarkupContainer;
        Vector2 scale;
        // Referencia a PathOSManager original
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
        GameObject prefabBox;
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
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
#if UNITY_EDITOR
            // Prefabs
            agentPrefab = PathOSStorage.Instance.agentPrefab.gameObject;
            managerPrefab = PathOSStorage.Instance.managerPrefab.gameObject;
            worldCameraPrefab = PathOSStorage.Instance.worldCameraPrefab.gameObject;
            screenshotCameraPrefab = PathOSStorage.Instance.screenshotCameraPrefab.gameObject;
            prefabBox = Resources.Load<GameObject>("Prefabs/BoxWithTexture");
            // Variables
            module = layer.GetModule<PathOSModule>();
            tiles = module.GetTiles();
            scale = settings.scale;
            // Obtiene (o crea) instancia de PathOSWindow
            window = EditorWindow.GetWindow(typeof(PathOSWindow), false, "PathOS+") as PathOSWindow;
            // Objeto contenedor padre
            parent = new GameObject("PathOS+ Tags");
            // Objeto hijo, contenedor de los objetos etiquetados
            levelMarkupContainer = new GameObject("Level Markup");
            levelMarkupContainer.transform.SetParent(parent.transform);
            // Lista de GameObjects a retornar
            List<GameObject> boxes = new List<GameObject>();

            // Instanciacion de Prefabs + colocacion de referencias en PathOSWindow
            GenerateManager();
            GenerateWorldCamera();
            GenerateScreenshotCamera();

            // Instanciacion de tags
            foreach (PathOSTile tile in tiles)
            {
                // Si el tile es de agente, instanciar prefab.
                GameObject currInstance;
                if (tile.Tag.Label == "PathOSAgent")
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
                else
                {
                    // Instanciar prefab
                    currInstance = PrefabUtility.InstantiatePrefab(prefabBox, levelMarkupContainer.transform) as GameObject;
                }

                // Agregar icono del Tag asociado a este tile como textura al cubo (excepto al tile agente)
                MeshRenderer currRenderer = currInstance.GetComponentInChildren<MeshRenderer>();
                Material originalMaterial = currRenderer.sharedMaterial;
                Material currMaterial = new Material(originalMaterial);
                currMaterial.SetTexture("_MainTex", tile.Tag.Icon);
                currRenderer.material = currMaterial;

                // Setear posicion
                currInstance.transform.position = settings.position +
                                                  new Vector3(tile.X * scale.x, 0, tile.Y * scale.y)
                                                  - new Vector3(scale.x, 0, scale.y) / 2f;

                // Agregar Tag a entidades del PathOSManager
                manager = managerGameObject.GetComponent<PathOSManager>();
                manager.AddLevelEntity(currInstance, tile.Tag.EntityType);
            }

            // GABO TERMINAR:
            // Baking automatico (asume que ya se encuentran instanciados los objetos de los otros modulos,
            // y que estos tienen colliders)
            GenerateNavMesh();

            // Obtener posicion planar promedio de las cajas, y altura del objeto mas bajo,
            // para asignar al padre.
            if (boxes.Count > 0)
            {
                var x = boxes.Average(o => o.transform.position.x);
                var y = boxes.Min(o => o.transform.position.y);
                var z = boxes.Average(o => o.transform.position.z);
                // Asignar esta posicion al objeto contenedor padre
                parent.transform.position = new Vector3(x, y, z);
            }

            // Asignar padre
            foreach (var box in boxes)
            {
                box.transform.parent = parent.transform;
            }

            // Ya unidos los objetos hijos con padre, trasladar segun Settings
            // GABO TODO: No es esto un error? Basado en PopulationRuleGenerator.
            parent.transform.position += settings.position;

            return parent;
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
            List<GameObject> interiorLayerGameObjects = GameObject.FindObjectsOfType<GameObject>().Where(
                obj => obj.transform.childCount == 2 &&
                obj.transform?.GetChild(0).name == "Schema" &&
                obj.transform?.GetChild(1).name == "Schema outside").ToList();

            // Exterior Layers: GameObjects
            List<GameObject> exteriorLayerGameObjects = GameObject.FindObjectsOfType<GameObject>().Where(
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
            // Agrega componente de superficie y limita su efecto a los hijos de tempParent
            var surface = tempParent.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            // Guarda padres viejos para la posterior reinsertacion
            GameObject[] interiorOldParents = new GameObject[interiorLayerGameObjects.Count];
            GameObject[] exteriorOldParents = new GameObject[exteriorLayerGameObjects.Count];

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

            // Genera NavMesh (Bake)
            surface.BuildNavMesh();

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

            // Padre temporal cambia de nombre (pasa a contener unicamente el navmesh)
            tempParent.name = "NavMeshSurface";
        }

        private GameObject SimpleBoxGenerate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Variables
            module = layer.GetModule<PathOSModule>();
            tiles = module.GetTiles();
            scale = settings.scale;
            // Objeto contenedor padre
            parent = new GameObject("PathOS+ Tags");
            // Prefab
            prefabBox = Resources.Load<GameObject>("Prefabs/BoxWithTexture");
            // GameObject List
            List<GameObject> boxes = new List<GameObject>();

            foreach (PathOSTile tile in tiles)
            {
                // Instanciar prefab
#if UNITY_EDITOR
                GameObject currInstance = PrefabUtility.InstantiatePrefab(prefabBox) as GameObject;
#else
                Debug.LogError("Attempting to use PathOSRuleGenerator class outside of Editor!"); return null;
#endif
                // Agregar icono del Tag asociado a este tile como textura al cubo
                MeshRenderer currRenderer = currInstance.GetComponentInChildren<MeshRenderer>();
                Material originalMaterial = currRenderer.sharedMaterial;
                Material currMaterial = new Material(originalMaterial);
                currMaterial.SetTexture("_MainTex", tile.Tag.Icon);
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
