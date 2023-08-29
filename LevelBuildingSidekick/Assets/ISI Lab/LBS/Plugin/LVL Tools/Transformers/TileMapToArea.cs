using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System.Linq;
using System;
using LBS.Components;

namespace LBS.Tools.Transformer
{
    [Obsolete("Los trasformar ya no suan devido a que la implentacion no era trivial")]
    public class TileMapToArea //: Transformer
    {
        /*
        AreaModule<Area> areaModule;
        TileMapModule tileMap;

        public TileMapToArea(TileMapModule tileMap, AreaModule<Area> area): base(tileMap.GetType(),area.GetType())
        {
            this.areaModule = area;
            this.tileMap = tileMap;
        }

        public override void Switch(ref LBSLayer layer)
        {
            var tiles = tileMap.Tiles;
            var IDs = tiles.Select(t => t.ID).Distinct();

            foreach(var id in IDs)
            {
                var room = tiles.Where(t => t.ID.Equals(id)).ToList();
                var corners = GetCorners(room.Select(t => t as ConnectedTile).ToList());

                var area = areaModule.GetArea(id);
                if(area != null)
                {
                    areaModule.Remove(area);
                }
                area = new Area(id, corners);
                areaModule.AddArea(area);
            }
        }

        public List<Vector2> GetCorners(List<ConnectedTile> tiles)
        {
            var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };
            var diagDir = new List<Vector2>() { Vector2.right + Vector2.up, Vector2.up +Vector2.left, Vector2.left + Vector2.down, Vector2.down + Vector2.right };

            var corners = new List<Vector2>();

            for(int i = 0; i < tileMap.TileCount; i++)
            {
                if (IsConvexCorner(tileMap.GetTile(i).Position, sideDir) || IsConcaveCorner(tileMap.GetTile(i).Position, diagDir))
                    corners.Add(tileMap.GetTile(i).Position);
            }

            var edges = new List<Tuple<Vector2, Vector2>>();
            
            foreach(var corner in corners)
            {
                foreach(var dir in sideDir)
                {
                    var pos = corner;
                    while(true)
                    {
                        pos += dir;
                        if (!tileMap.Contains(pos))
                            break;
                        if(corners.Contains(pos))
                        {
                            edges.Add(new Tuple<Vector2, Vector2>(corner, pos));
                        }
                    }
                }
            }

            while(corners.Count > 0)
            {

            }



            return corners;
        }

        public bool IsConvexCorner(Vector2 pos, List<Vector2> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s != 0)
            {
                if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                    return true;
            }
            return false;
        }

        public bool IsConcaveCorner(Vector2 pos, List<Vector2> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s != 0)
                return false;
            if (s == 1 || s == 2 || s == 4 || s == 8)
                return true;
            return false;
        }

        private int NeighborhoodValue(Vector2Int position, List<Vector2> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
        {
            var value = 0;
            for (int i = 0; i < directions.Count; i++)
            {
                var otherPos = position + directions[i];
                if (tileMap.GetTile(otherPos.ToInt()) == null)
                {
                    value += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
            }

            return value;
        }

        public override void ReCalculate(ref LBSLayer layer)
        {
            throw new NotImplementedException();
        }
        */
    }
}

