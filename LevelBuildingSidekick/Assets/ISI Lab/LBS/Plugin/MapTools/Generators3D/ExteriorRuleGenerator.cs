using System;
using System.Collections.Generic;
using System.Linq;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;

using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.Generators
{
    public class ExteriorRuleGenerator : LBSGeneratorRule 
    {
        private Tuple<LBSDirection, int> GetBundle(LBSDirectionedGroup group, string[] conections)
        {
            // Get connections
            var connections = group.GetDirs();

            foreach (var connection in connections)
            {
                for (int i = 0; i < 4; i++)
                {
                    var curDir = connection.Connections.Rotate(i);
                    if (curDir.SequenceEqual(conections))
                    {
                       // Debug.Log("found");
                        return new Tuple<LBSDirection, int>(connection, i);
                    }
                }
            }
            return null;
        }


        public override Tuple<GameObject, string> Generate(LBSLayer layer, Generator3D.Settings settings)
        {

            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

            if (layer.Behaviours.Count == 0)
            {
                return Tuple.Create<GameObject,string>(null,"No behaviours found");
            }
            
            var exteriorBehaviour = layer.Behaviours.Find(b => b is ExteriorBehaviour) as ExteriorBehaviour;
            var bundle = exteriorBehaviour?.Bundle; 
            if (bundle == null)
            {
                return Tuple.Create<GameObject, string>(null, "Bundle not found");
            }
            
            var selected = bundle.GetCharacteristics<LBSDirectionedGroup>()[0];
            
            // Create pivot
            var mainPivot = new GameObject("Exterior");
            var scale = settings.scale;

            // Get modules
            var mapMod = layer.GetModule<TileMapModule>();
            var connctMod = layer.GetModule<ConnectedTileMapModule>();
            
            var tiles = new List<GameObject>();

            foreach (var tile in mapMod.Tiles)
            {
                // Get connection
                var tileConnection = connctMod.GetConnections(tile);
                // Get pref
                var pair = GetBundle(selected, tileConnection.ToArray());
                //pair.owner = bundle;
                var pref = pair?.Item1?.Owner?.Assets?.RandomRullete(w => w.probability)?.obj;

                if(pref == null)
                {
 
                    Debug.LogWarning("[ISILab]: Element generation has failed, " +
                        "make sure you have properly configured and assigned " +
                        "the Bundles you want to generate with.");
                    continue;
                }

#if UNITY_EDITOR
                var go = PrefabUtility.InstantiatePrefab(pref,null) as GameObject;
#else
                var go = GameObject.Instantiate(pref,null);
#endif

                var pos = new Vector3(tile.Position.x * scale.x, 0, tile.Position.y * scale.y);
                var delta = (new Vector3(scale.x, 0, scale.y) / 2f);
                go.transform.position = settings.position + pos - delta;

                if (pair.Item2 % 2 == 0)
                    go.transform.rotation = Quaternion.Euler(0, 90 * (pair.Item2) % 360, 0);
                else
                    go.transform.rotation = Quaternion.Euler(0, 90 * (pair.Item2 - 2) % 360, 0);
                
                tiles.Add(go);

                var current = pair.Item1.Owner;
                // Add ref component
                LBSGenerated generatedComponent = go.AddComponent<LBSGenerated>();
                generatedComponent.BundleRef = current;
                
            }

            if (tiles.Count == 0)
            {
                return Tuple.Create<GameObject, string>(null, "[ISILab]: No tiles were created in the tool. Can't generate game object.");
            }
            
            var x = tiles.Average(t => t.transform.position.x);
            var y = tiles.Min(t => t.transform.position.y);
            var z = tiles.Average(t => t.transform.position.z);

            mainPivot.transform.position = new Vector3(x, y, z);

            foreach (var tile in tiles)
            {
                tile.transform.parent = mainPivot.transform;
            }

            mainPivot.transform.position += settings.position;

            return Tuple.Create<GameObject, string>(mainPivot, "");
        }

        public override object Clone()
        {
            return new ExteriorRuleGenerator();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExteriorRuleGenerator;

            if (other == null) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new NotImplementedException(); // TODO: Implement this method to check if the rule is viable for the layer
        }
    }
}
