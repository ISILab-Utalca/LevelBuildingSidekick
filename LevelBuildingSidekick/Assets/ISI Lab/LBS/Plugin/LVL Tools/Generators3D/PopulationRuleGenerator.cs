using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LBS.Representation;
using Utility;
using System.Linq;
//using UnityEditor;
using LBS.Components;
using UnityEditor;

namespace LBS.Generator
{
    [System.Serializable]
    public class PopulationRuleGenerator : LBSGeneratorRule // PopulationGenerator -> TaggedTileMapGenerator (?)
    {
        public override bool CheckIfIsPosible(LBSLayer layer,out string msg)
        {
            msg = "The layer does not contain any module corresponding to 'TaggedTileMap'.";

            var data = layer.GetModule<TaggedTileMap>();

            return (data != null);
        }

        public override object Clone()
        {
            return new PopulationRuleGenerator();
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var data = layer.GetModule<TaggedTileMap>();
            var bundles = Utility.DirectoryTools.GetScriptables<SimpleBundle>();
            var scale = settings.scale;

            var parent = new GameObject("Population");
            var tiles = data.PairTiles.Select(x => x.tile);
            foreach (var tile in tiles)
            {
                var tag = data.GetBundleData(tile).BundleTag;
                var bundle = bundles.Find(b => b.ID.Label == tag);

                var pref = bundle.GetObject(Random.Range(0, bundle.Assets.Count));

                var go = GameObject.Instantiate(pref, parent.transform);
                go.transform.position = new Vector3(
                    scale.x * tile.Position.x,
                    0,
                    -scale.y * tile.Position.y) + new Vector3(scale.x, 0, -scale.y) / 2;
            }

            parent.transform.position += settings.position;

            return parent;
        }
    }

}