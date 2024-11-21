using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            // Referencia a interfaz
            window = EditorWindow.GetWindow(typeof(PathOSWindow), false, "PathOS+") as PathOSWindow;
            // Objeto contenedor padre
            parent = new GameObject("PathOS+ Tags");
            // Objeto hijo, contenedor de los objetos etiquetados
            levelMarkupContainer = new GameObject("Level Markup");
            levelMarkupContainer.transform.SetParent(parent.transform);
            // Lista de GameObjects a retornar
            List<GameObject> boxes = new List<GameObject>();

            // Instanciacion de Prefabs y colocacion de referencias en PathOSWindow
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
                var manager = managerGameObject.GetComponent<PathOSManager>();
                manager.AddLevelEntity(currInstance, tile.Tag.EntityType);
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
