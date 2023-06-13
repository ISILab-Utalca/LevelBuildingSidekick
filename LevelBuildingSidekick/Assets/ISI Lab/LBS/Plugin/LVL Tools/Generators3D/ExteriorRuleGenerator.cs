using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    [System.Serializable]
    public class ExteriorRuleGenerator : LBSGeneratorRule //  (!!!) esta clase mescla lo que tiene que hacer la IA de WFC con generar 3d posteriormente
    {
        public override bool CheckIfIsPosible(LBSLayer layer, out string msg)
        {
            var module = layer.GetModule<Exterior>();

            msg = "The layer does not contain any module corresponding to 'Exterior'.";

            return (module != null);
        }

        public override object Clone()
        {
            return new ExteriorRuleGenerator();
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var modulo = layer.GetModule<Exterior>();
            var tiles = Utility.DirectoryTools.GetScriptables<WFCBundle>();

            var mainPivot = new GameObject("Exterior");
            var scale = settings.scale;
            foreach (var tile in modulo.Tiles.Select(t => t as ConnectedTile))
            {
                var tuple = tiles.Get(tile.Connections);

                if (tuple == null)
                    continue;

                var prefs = tuple.Item1.Pref;
                GameObject selected = prefs[0].gameObject;

                // rullete selector
                var tWeight = prefs.Sum(w => w.weight);
                var vWeigth = Random.Range(0f, tWeight);
                var current = 0f;
                for (int i = 0; i < prefs.Count; i++)
                {
                    current += prefs[i].weight;
                    if (vWeigth <= current)
                    {
                        selected = prefs[i].gameObject;
                        break;
                    }
                }

                var go = GameObject.Instantiate(selected, mainPivot.transform);
                go.transform.position = new Vector3((tile.Position.x) * scale.x, 0,-(tile.Position.y) * scale.y) + new Vector3(scale.x, 0, -scale.y) / 2;

                var rot = tuple.Item2;

                go.transform.localScale = new Vector3(1,1,-1);
                go.transform.rotation = Quaternion.Euler(0, -90 + (-90 * (rot)) % 360, 0);

            }
            mainPivot.transform.position += settings.position;
            return mainPivot;
        }

    }
}
