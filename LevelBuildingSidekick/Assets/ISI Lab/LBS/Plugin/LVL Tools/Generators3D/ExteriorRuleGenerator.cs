using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class ExteriorRuleGenerator : LBSGeneratorRule //  (!!!) esta clase mescla lo que tiene que hacer la IA de WFC con generar 3d posteriormente
    {
        public override bool CheckIfIsPosible(LBSLayer layer, out string msg)
        {
            var module = layer.GetModule<ExteriorModule>();

            msg = "The layer does not contain any module corresponding to 'Exterior'.";

            return (module != null);
        }

        public override object Clone()
        {
            return new ExteriorRuleGenerator();
        }

        private Tuple<LBSDirection,int> GetBundle(LBSDirectionedGroup group, string[] conections)
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
                        return new Tuple<LBSDirection, int>(connection, i);
                    }
                }
            }
            return null;
        }

        
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Get bundles
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

            // Get CharacteristicGroup bundles
            bundles = bundles.Where(b => b.GetCharacteristics<LBSDirectionedGroup>().Count > 0 && !b.IsPresset).ToList();
            
            var selected = bundles[0].GetCharacteristics<LBSDirectionedGroup>()[0];

            // Create pivot
            var mainPivot = new GameObject("Exterior");
            var scale = settings.scale;

            // Get modules
            var mapMod = layer.GetModule<TileMapModule>();
            var connctMod = layer.GetModule<ConnectedTileMapModule>();

            foreach (var tile in mapMod.Tiles)
            {
                // Get connection
                var con = connctMod.GetConnections(tile);

                // Get pref
                var pair = GetBundle(selected, con.ToArray());
                var pref = pair.Item1.Weights.RandomRullete(w => w.weigth).target;

#if UNITY_EDITOR
                var go = PrefabUtility.InstantiatePrefab(pref, mainPivot.transform) as GameObject;
#else
                var go = GameObject.Instantiate(pref, mainPivot.transform);
#endif

                go.transform.position = new Vector3((tile.Position.x) * scale.x, 0, (tile.Position.y) * scale.y) + new Vector3(scale.x, 0, scale.y) / 2;

                if(pair.Item2 % 2 == 0)
                    go.transform.rotation = Quaternion.Euler(0, 90 * (pair.Item2 -1) % 360, 0);
                else
                    go.transform.rotation = Quaternion.Euler(0, 90 * (pair.Item2 -3) % 360, 0);
            }

            mainPivot.transform.position += settings.position;
            return mainPivot;
        }
    }
}
