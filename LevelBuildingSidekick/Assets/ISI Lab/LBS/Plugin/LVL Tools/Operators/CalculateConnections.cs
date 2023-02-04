using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalculateConnections
{

    public static void Operate<T>(AreaTileMap<T> module) where T : TiledArea
    {
        List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
        {
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up
        };

        var m = module;
        foreach (var area in m.Areas)
        {

            foreach (var tile in area.Tiles)
            {
                for (int i = 0; i < dirs.Count; i++)
                {
                    var nei = m.GetTileNeighbor(tile as ConnectedTile, dirs[i]);

                    if (nei == null)
                    {
                        (tile as ConnectedTile).SetConnection("Wall", i);
                    }
                    else if (area.Contains(nei.Position))
                    {
                        (tile as ConnectedTile).SetConnection("Empty", i);
                    }
                    else
                    {
                        if ((tile as ConnectedTile).GetConnection(i) != "Door")
                            (tile as ConnectedTile).SetConnection("Wall", i); // (?) o puerta
                                                                              //ct.SetConnection("Wall", i); // (?) o puerta
                    }
                }
            }
        }
    }
}
