using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LBS.Components;
using UnityEditor;
using LBS.Bundles;
using ISILab.Commons;
using ISILab.LBS.Modules;
using ISILab.LBS.Internal;

namespace ISILab.LBS.Generators
{
    [System.Serializable]
    public class PopulationRuleGenerator : LBSGeneratorRule // FIX: Change to a better name
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

            var objects = new List<GameObject>();

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
                var go = PrefabUtility.InstantiatePrefab(pref.obj) as GameObject;
#else
                var go = GameObject.Instantiate(pref.obj);
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


                objects.Add(go);
            }

            var x = objects.Average(o => o.transform.position.x);
            var y = objects.Min(o => o.transform.position.y);
            var z = objects.Average(o => o.transform.position.z);

            parent.transform.position = new Vector3 (x, y, z);

            foreach (var obj in objects)
            {
                obj.transform.parent = parent.transform;
            }

            parent.transform.position += settings.position;

            return parent;
        }
    }

}