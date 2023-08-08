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
            var directionChars = group.Weights.Select(w => w.target.GetCharacteristic<LBSDirection>()).ToList();

            foreach (var dirChar in directionChars)
            {
                var intiDir = new List<string>(dirChar.Connections);
                for (int i = 0; i < 4; i++)
                {
                    var curDir = intiDir.Rotate(i);
                    if (curDir.SequenceEqual(conections))
                    {
                        return new Tuple<LBSDirection, int>(dirChar, i);
                    }

                }

            }
            return null;
        }

        
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var storage = LBSAssetsStorage.Instance;
            var modulo = layer.GetModule<ExteriorModule>();
            var bundles = storage.Get<Bundle>()
                .Where(b => b.GetCharacteristic<LBSDirectionedGroup>() != null && !b.isPreset)
                .Select(b => b.GetCharacteristic<LBSDirectionedGroup>())
                .ToList();
            
            var selectedBundle = bundles[0]; // esto tiene que ser seleccionado en la interfaz y no sacar el primero que pilla (!!!)

            
            var mainPivot = new GameObject("Exterior");
            var scale = settings.scale;

            var tiles = modulo.Tiles.Select(t => t as ConnectedTile);
            foreach (var tile in tiles)
            {
                var tileCon = tile.Connections;
                var pair = GetBundle(selectedBundle, tileCon); // char y rotacion

                var pref = pair.Item1.Weights.RandomRullete(w => w.weigth).target;

#if UNITY_EDITOR
                var go = PrefabUtility.InstantiatePrefab(pref, mainPivot.transform) as GameObject;
#else
    var go = GameObject.Instantiate(pref, mainPivot.transform);
#endif

                go.transform.position = new Vector3((tile.Position.x) * scale.x, 0, -(tile.Position.y) * scale.y) + new Vector3(scale.x, 0, -scale.y) / 2;

                var rot = pair.Item2;

                go.transform.localScale = new Vector3(1, 1, -1);
                go.transform.rotation = Quaternion.Euler(0, -90 + (-90 * (rot)) % 360, 0);
            }
            mainPivot.transform.position += settings.position;
            return mainPivot;
        }

        /*
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var storage = LBSAssetsStorage.Instance;
            var modulo = layer.GetModule<Exterior>();
            var bundles = storage.Get<Bundle>().Select(b => b.GetCharacteristic<LBSDirectionedGroup>()).ToList();
            
            var selectedBundle = bundles[0]; // esto tiene que ser seleccionado en la interfaz y no sacar el primero que pilla (!!!)

            var mirrs = new List<Vector3>() {
                 new Vector3(1, 1, 1),
                 new Vector3(1, 1, -1),
                 //new Vector3(1, -1, 1),
                 new Vector3(-1, 1, 1),
                 //new Vector3(1, -1, -1),
                 new Vector3(-1, 1, -1),
                 //new Vector3(-1, -1, 1),
                 //new Vector3(-1, -1, -1),
            };

            var sts = new List<int>()
            {
                0,90,-90
            };

            var rots = new List<int>()
            {
                90,-90
            };

            var all = new GameObject("all");
            var d = 70;
            int cx = 0, cy = 0, cz = 0;
            foreach (var _rot in rots)
            {
                cz = 0;
                foreach (var _sts in sts)
                {
                    cx = 0;
                    foreach (var _mirr in mirrs)
                    {
                        var mainPivot = new GameObject("Exterior");
                        mainPivot.SetParent(all);
                        var scale = settings.scale;

                        var tiles = modulo.Tiles.Select(t => t as ConnectedTile);
                        foreach (var tile in tiles)
                        {
                            var tileCon = tile.Connections;
                            var pair = GetBundle(selectedBundle, tileCon); // char y rotacion

                            var pref = pair.Item1.Weights.RandomRullete(w => w.weigth).target;

#if UNITY_EDITOR
                            var go = PrefabUtility.InstantiatePrefab(pref, mainPivot.transform) as GameObject;
#else
                            var go = GameObject.Instantiate(pref, mainPivot.transform);
#endif

                            go.transform.position = new Vector3((tile.Position.x) * scale.x, 0, -(tile.Position.y) * scale.y) + new Vector3(scale.x, 0, -scale.y) / 2;

                            var rot = pair.Item2;

                            go.name = "mir: " + _mirr + ",sts: " + _sts + ",rot: " + _rot + ",r: " + rot;
                            go.transform.localRotation = Quaternion.Euler(0, _sts + ((_rot * rot) % 360), 0);
                            go.transform.localScale = _mirr;

                        }
                        mainPivot.transform.position += settings.position;

                        mainPivot.transform.position += new Vector3(cx,cy*2f,cz) * d;
                        cx++;
                    }
                    cz++;
                }
                cy++;
            }

            
            return all;
        }
        */
    }
}
