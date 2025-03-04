using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ISILab.LBS.Generators
{
    [System.Serializable]
    public class PopulationRuleGenerator : LBSGeneratorRule // FIX: Change to a better name
    {
        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new System.NotImplementedException(); // TODO: Implement CheckViability method
        }

        public override object Clone()
        {
            return new PopulationRuleGenerator();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PopulationRuleGenerator;

            if (other == null) return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var data = layer.GetModule<BundleTileMap>();
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            var scale = settings.scale;

            var parent = new GameObject("Types");

            var parentEntity = new GameObject("Entity");
            var parentObject = new GameObject("Object");
            var parentInteractable = new GameObject("Interactable");
            var parentArea = new GameObject("Area");
            var parentProp = new GameObject("Prop");
            var parentMisc = new GameObject("Misc");

            var groups = data.Groups;

            var objects = new Dictionary<GameObject, Bundle.PopulationTypeE>();
            foreach (TileBundleGroup group in groups){ 
                foreach (var tile in group.TileGroup)
                {
                    Bundle current = null;
                    foreach (var b in bundles)
                    {
                        var id = b.name;

                        if (id.Equals(group.BundleData.BundleName))
                            current = b;
                    }

                    /*
                    if (bundles == null)
                    {
                        Debug.LogWarning("[ISI Lab]: There is no asset named '" + tile.BundleData.BundleName +
                        "'. Please verify the bundles present in the project or the elements assigned in the level.");
                        continue;
                    }*/

                    if (current == null) continue;

                    var pref = current.Assets[Random.Range(0, current.Assets.Count)];
                    if (pref == null)
                    {
                        Debug.LogError("Null reference in asset: " + current.Name);
                        continue;
                    }

    #if UNITY_EDITOR
                    var go = PrefabUtility.InstantiatePrefab(pref.obj) as GameObject;
    #else
                    var go = GameObject.Instantiate(pref.obj);
    #endif
                    if (go == null)
                    {
                        Debug.LogError("Could not find prefab for: " + current.Name);
                        continue;
                    }

                    var r = Directions.Bidimencional.Edges.FindIndex(v => v == group.Rotation);
                    go.transform.rotation = Quaternion.Euler(0, -90 * (r - 1), 0);

                    if (settings.useBundleSize)
                        if (current != null)
                            scale = current.TileSize;

                    // Set General position
                    go.transform.position =
                        settings.position +
                        new Vector3(tile.Position.x * scale.x, 0, tile.Position.y * scale.y) +
                        -(new Vector3(scale.x, 0, scale.y) / 2f);

                    LBSGenerated generatedComponent = go.AddComponent<LBSGenerated>();
                    generatedComponent.BundleRef = current;
                    objects.Add(go, current.PopulationType);
                }
            }
            
            if(objects.Count == 0)
            {
                Debug.LogWarning("No population objects were created. Assign a valid bundle type");
                return parent;
            }
            
            
            var x = objects.Keys.Average(o => o.transform.position.x);
            var y = objects.Keys.Min(o => o.transform.position.y);
            var z = objects.Keys.Average(o => o.transform.position.z);
            
            foreach (var obj in objects)
            {
                switch(obj.Value)
                {
                    case Bundle.PopulationTypeE.Character: 
                        obj.Key.transform.SetParent(parentEntity.transform);
                        break;
                    case Bundle.PopulationTypeE.Item:
                        obj.Key.transform.SetParent(parentObject.transform);
                        break;
                    case Bundle.PopulationTypeE.Interactable:
                        obj.Key.transform.SetParent(parentInteractable.transform);
                        break;
                    case Bundle.PopulationTypeE.Area:
                        obj.Key.transform.SetParent(parentArea.transform);
                        break;
                    case Bundle.PopulationTypeE.Prop:
                        obj.Key.transform.SetParent(parentProp.transform);
                        break;
                    case Bundle.PopulationTypeE.Misc:
                        obj.Key.transform.SetParent(parentMisc.transform);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                parent.transform.position = new Vector3 (x, y, z);
  
                
            }

            parentEntity.transform.SetParent(parent.transform);
            parentObject.transform.SetParent(parent.transform);
            parentInteractable.transform.SetParent(parent.transform);
            parentArea.transform.SetParent(parent.transform);
            parentProp.transform.SetParent(parent.transform);
            
            
            parentMisc.transform.SetParent(parent.transform);
            parent.transform.position += settings.position;
            
            return parent;
        }
    }

}