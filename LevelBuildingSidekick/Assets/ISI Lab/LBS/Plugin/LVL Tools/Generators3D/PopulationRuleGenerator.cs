using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LBS.Representation;
using Utility;
using System.Linq;
//using UnityEditor;
using LBS.Components;
using UnityEditor;
using LBS.Bundles;

namespace LBS.Generator
{
    [System.Serializable]
    public class PopulationRuleGenerator : LBSGeneratorRule // PopulationGenerator -> TaggedTileMapGenerator (?)
    {
        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            return new PopulationRuleGenerator();
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var data = layer.GetModule<BundleTileMap>();
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            var scale = settings.scale;

            var parent = new GameObject("Population");
            var tiles = data.Tiles;
            foreach (var tile in tiles)
            {
                Bundle current = null;
                foreach (var b in bundles)
                {
                    var id = b.name;
                    
                    if (id.Equals(tile.BundleData.BundleName))
                        current = b;
                }

                if(bundles == null)
                {
                    Debug.Log("[ISI Lab]: no exite ningun asset asignado a ");
                    continue;
                }

                var pref = current.Assets[Random.Range(0, current.Assets.Count)];
#if UNITY_EDITOR
                var go = PrefabUtility.InstantiatePrefab(pref.obj, parent.transform) as GameObject;
#else
                var go = GameObject.Instantiate(pref.obj, parent.transform);
#endif

                go.transform.position = new Vector3(
                    scale.x * tile.Tile.Position.x,
                    0,
                    scale.y * tile.Tile.Position.y) - new Vector3(scale.x, 0, scale.y) / 2;
                var r = Directions.Bidimencional.Edges.FindIndex(v => v == tile.Rotation);
                if(r % 2 == 0)
                    go.transform.rotation = Quaternion.Euler(0, -90 * (r - 1), 0);
                else
                    go.transform.rotation = Quaternion.Euler(0, -90 * (r - 3), 0);

            }

            parent.transform.position += settings.position;

            return parent;
        }
    }

}