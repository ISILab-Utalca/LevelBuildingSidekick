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
    public class PopulationGenerator : Generator3D
    {
        public override GameObject Generate(LBSLayer layer)
        {
            Init(layer);

            var data = layer.GetModule<TaggedTileMap>();

            var parent = new GameObject(objName);

            foreach(var k in data.PairTiles.Select(x => x.tile))
            {
                var sc = Utility.DirectoryTools.GetScriptables<SimpleBundle>().Find(b => b.ID.Label == data.GetBundleData(k).BundleTag);

                var pref = sc.GetObject(Random.Range(0, sc.Assets.Count));

                var go = GameObject.Instantiate(pref, parent.transform);
                go.transform.position = new Vector3(
                    scale.x * k.Position.x, 
                    0,
                    -scale.y * k.Position.y) + new Vector3(scale.x, 0, -scale.y)/2;
            }

            parent.transform.position = position;

            return parent;
        }

        public override void Init(LBSLayer layer)
        {
            //throw new System.NotImplementedException();
        }
    }

}