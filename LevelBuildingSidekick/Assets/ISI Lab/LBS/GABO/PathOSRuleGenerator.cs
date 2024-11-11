using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ISILab.LBS.Generators
{
    public class PathOSRuleGenerator : LBSGeneratorRule
    {
        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new System.NotImplementedException(); // GABO TODO: Implementar? (Proyecto no la usa todavia)
        }

        public override object Clone()
        {
            return new PathOSRuleGenerator();
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            //GABO TODO: Borrar Generador de testeo temporal y hacer generador real
            var boxCollection = SimpleBoxGenerate(layer, settings);

            return boxCollection;
        }

        // GABO TODO: Terminar generador de testeo.
        private GameObject SimpleBoxGenerate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Variables
            PathOSModule module = layer.GetModule<PathOSModule>();
            List<PathOSTile> tiles = module.GetTiles();
            Vector2 scale = settings.scale;
            // Objeto contenedor padre
            GameObject parent = new GameObject("PathOS+ Tags");
            // Prefab
            GameObject prefabBox = Resources.Load<GameObject>("Prefabs/BoxWithTexture");
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
                currMaterial.SetTexture(tile.Tag.Icon.ToString(), tile.Tag.Icon);
                currRenderer.material = currMaterial;

                // Setear posicion
                currInstance.transform.position = settings.position +
                                                  new Vector3(tile.X * scale.x, 0, tile.Y * scale.y);
                                                  //- new Vector3(scale.x, 0, scale.y) / 2f; // GABO TODO: Necesario agreg. este vector???
                boxes.Add(currInstance);
            }

            // Obtener posicion planar promedio de las cajas, y altura del objeto mas bajo.
            var x = boxes.Average(o => o.transform.position.x);
            var y = boxes.Min(o => o.transform.position.y);
            var z = boxes.Average(o => o.transform.position.z);
            // Asignar esta posicion al objeto contenedor padre
            parent.transform.position = new Vector3(x, y, z);

            foreach(var box in boxes)
            {
                box.transform.parent = parent.transform;
            }

            // Ya unidos los objetos hijos con padre, trasladar segun Settings
            // GABO TODO: No es esto un error? Basado en PopulationRuleGenerator. TESTEAR!
            //parent.transform.position += settings.position;

            return parent; 
        }
    }
}
