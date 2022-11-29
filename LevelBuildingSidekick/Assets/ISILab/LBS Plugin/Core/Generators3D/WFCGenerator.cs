using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class WFCGenerator : Generator
    {
        public MapData wfc;
        public float size;

        public override GameObject Generate()
        {
            var tiles = DirectoryTools.GetScriptables<TileConections>();
            var wfcTiles = wfc.Tiles;

            if (wfc == null)
            {
                Debug.LogWarning("cannot be generated, there is no information about the map to load.");
                return null;
            }

            var mainPivot = new GameObject("New level 3D");
            for (int i = 0; i < wfc.Tiles.Count; i++)
            {
                var r = 0;
                var posibleTile = new List<TileConections>();
                for (int j = 0; j < tiles.Count; j++)
                {
                    var tc = tiles[j];
                    var c1 = tc.Connections;
                    var c2 = wfcTiles[i].Connections;
                    for (int k = 0; k < 4; k++)
                    {
                        r = k;
                        if (Compare(c1, c2))
                        {
                            posibleTile.Add(tiles[i]);
                        }

                        c2 = Rotate(c2);
                    }
                }
                
                if(posibleTile.Count <= 0)
                {
                    Debug.Log("there is no tile matching these connections." + wfcTiles.ToString());
                    continue;
                }

                var pref = GetPref(posibleTile);

                if(pref == null)
                {
                    Debug.Log("problems loading gameobject");
                    continue;
                }

                var go = SceneView.Instantiate(pref, mainPivot.transform);
                go.transform.position = new Vector3(wfc.Tiles[i].Position.x, 0, -wfc.Tiles[i].Position.y) * size* 10;

                if (r % 2 != 0) // parche, no quitar (!!!)
                    r += 2;

                for (int k = 0; k < r; k++)
                    go.transform.Rotate(new Vector3(0, 1, 0), 90);
            }
            return mainPivot;
        }

        private GameObject GetPref(List<TileConections> posibles) // (!) esto puede estar mejor o estar en un mejor lugar
        {
            var v = 0f;
            for (int i = 0; i < posibles.Count; i++)
            {
                v += posibles[i].weight;
            }
            var s = Random.Range(0f,v);
            var c = 0f;
            for (int i = 0; i < posibles.Count; i++)
            {
                c += posibles[i].weight;
                if (s <= c)
                    return posibles[i].Tile;
            }
            return null;
        }

        private bool Compare(string[] c1, string[] c2)
        {
            for (int i = 0; i < c1.Length; i++)
            {
                if (c1[i] == "" || c2[i] == "")
                    continue;

                if (c1[i] != c2[i])
                    return false;
            }

            return true;
        }

        private string[] Rotate(string[] c)
        {
            var last = c.Last();
            var cc = c.ToList().Remove(last);
            var r = new List<string>() { last };
            r.AddRange(c);
            r.Remove(r.Last());

            var rr = new string[4];
            for (int i = 0; i < 4; i++)
            {
                rr[i] = r[i];
            }
            
            return rr;
        }

        public override void Init(LevelData levelData)
        {
            this.wfc = levelData.GetRepresentation<MapData>();
            this.size = levelData.TileSize;
        }
    }
}
