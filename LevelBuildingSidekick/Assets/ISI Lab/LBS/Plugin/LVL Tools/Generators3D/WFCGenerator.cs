using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class WFCGenerator : Generator //  (!!!) esta clase mescla lo que tiene que hacer la IA de WFC con generar 3d posteriormente
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
                var posibleTile = new List<TileConnectWFC>();
                for (int j = 0; j < tiles.Count; j++)
                {
                    var tc = tiles[j];
                    var c1 = tc.Connections;
                    var c2 = wfcTiles[i].Connections;
                    for (int k = 0; k < 4; k++)
                    {
                        if (Compare(c1, c2))
                        {
                            posibleTile.Add(new TileConnectWFC(tc, k));
                        }

                        c1 = Rotate(c1);
                    }
                }

                if (posibleTile.Count <= 0)
                {
                    Debug.Log("there is no tile matching these connections." + wfcTiles.ToString());
                    continue;
                }

                var pref = GetPref(posibleTile);

                if (pref == null)
                {
                    Debug.Log("problems loading gameobject");
                    continue;
                }

                Debug.Log(pref.Rotation);

                var go = SceneView.Instantiate(pref.Tile.Tile, mainPivot.transform);
                go.transform.position = new Vector3(wfc.Tiles[i].Position.x, 0, -wfc.Tiles[i].Position.y) * size* 10; // (!!) *10 parche

                var r = pref.Rotation;
                // if (r % 2 != 0) // parche, no quitar (!!!)
                //     r += 2;
                go.transform.Rotate(new Vector3(0, -1, 0), 90); // (!!!) esto es un ´parche por que el WFC empieza con Right a la derecha y estpo empieza con Top a la derecha
                for (int k = 0; k < r; k++)
                    go.transform.Rotate(new Vector3(0, 1, 0), 90);
            }
            return mainPivot;
        }

        private TileConnectWFC GetPref(List<TileConnectWFC> posibles) // (!) esto puede estar mejor o estar en un mejor lugar
        {
            var v = 0f;
            for (int i = 0; i < posibles.Count; i++)
            {
                v += posibles[i].Tile.weight;
            }
            var s = Random.Range(0f,v);
            var c = 0f;
            for (int i = 0; i < posibles.Count; i++)
            {
                c += posibles[i].Tile.weight;
                if (s <= c)
                    return posibles[i];
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
            var temp = c.ToList();
            var last = c.Last();
            temp.RemoveAt(temp.Count - 1);
            var r = new List<string>() { last };
            r.AddRange(temp);

            var toR = new string[4];
            for (int i = 0; i < 4; i++)
            {
                toR[i] = r[i];
            }
            
            return toR;
        }

        public override void Init(LevelData levelData)
        {
            this.wfc = levelData.GetRepresentation<MapData>();
            this.size = levelData.TileSize;
        }
    }
}
