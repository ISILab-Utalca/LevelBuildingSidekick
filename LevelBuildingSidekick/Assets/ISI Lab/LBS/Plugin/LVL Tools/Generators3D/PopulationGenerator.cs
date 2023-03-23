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
    public class PopulationGenerator : Generator3D
    {
        public override GameObject Generate(LBSLayer layer)
        {
            var data = layer.GetModule<TaggedTileMap>();

            var parent = new GameObject(objName);

            foreach(var k in data.PairTiles.Select(x => x.tile))
            {
                var sc = Utility.DirectoryTools.GetScriptables<SimpleBundle>().Find(b => b.ID.Label == data.GetPair(k).BundleTag);

                var pref = sc.GetObject(Random.Range(0, sc.objects.Count));

                var go = GameObject.Instantiate(pref, parent.transform);
                go.transform.position = new Vector3(k.Position.x, 0, k.Position.y);
            }

            return parent;
        }

        public override void Init(LBSLayer layer)
        {
            //throw new System.NotImplementedException();
        }
    }
}