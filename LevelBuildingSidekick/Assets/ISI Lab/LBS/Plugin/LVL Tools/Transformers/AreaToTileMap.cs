using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using LBS.Components;
using System;

namespace LBS.Tools.Transformer
{
    [Obsolete("Los trasformar ya no suan devido a que la implentacion no era trivial")]
    public class AreaToTileMap //: Transformer
    {
        /*
        private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
        {
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up
        };

        AreaTileMap<TiledArea> areaModule;
        TiledArea tileMap;

        public AreaToTileMap() : base(null, null)
        {
        }

        public AreaToTileMap(Type from, Type to) : base(from, to)
        {
        }

        public AreaToTileMap(LBSLayer layer) : base(typeof(AreaTileMap<TiledArea>), typeof(AreaTileMap<TiledArea>))
        {
            this.areaModule = layer.GetModule(To) as AreaTileMap<TiledArea>; 
        }

        public override void Switch(ref LBSLayer layer)
        {
            this.areaModule = layer.GetModule(To) as AreaTileMap<TiledArea>;
            if (true) //tileMap.IsEmpty()
            {
                CreateDataFrom();
            }
            else
            {
                //EditDataFrom();
            }
        }

        private void CreateDataFrom()
        {
            var nAreas = new List<TiledArea>();
            foreach (var area in areaModule.Areas)
            {
                var nArea = new TiledArea(new List<LBSTile>(), area.ID, area.Color);

                foreach (var tile in area.Tiles)
                {
                    var ct = new ConnectedTile(tile.Position, tile.ID);

                    for (int i = 0; i < dirs.Count; i++)
                    {
                        var nei = areaModule.GetTileNeighbor(tile as ConnectedTile, dirs[i]);

                        if (nei == null)
                        {
                            ct.SetConnection("Wall", i);
                        }
                        else if (area.GetTile(nei.Position) != null)
                        {
                            ct.SetConnection("Empty", i);
                        }
                        else
                        {
                            ct.SetConnection("Wall", i); // (?) o puerta
                        }
                    }
                    nArea.AddTile(ct);
                }
                nAreas.Add(nArea);
            }
            areaModule.Clear();
            nAreas.ForEach(a => areaModule.AddArea(a));

        }

        private void EditDataFrom()
        {
            Debug.LogError("[ISI Lab]: Implementar!!");

            /*
           for(int i = 0; i < tileMap.TileCount; i++)
           {
               if(!areaModule. ContainsPoint(tileMap.GetTile(i).Position))
               {
                   tileMap.RemoveAt(i);
               }
           }

           for(int k = 0; k < areaModule.AreaCount; k++)
           {
               var area = areaModule.GetArea(k);
               var rect = area.Rect;

               for(int j = 0; j < rect.height; j += (int)tileMap.CellSize.y)
               {
                   for (int i = 0; i < rect.width; i += (int)tileMap.CellSize.x)
                   {
                       var pos = rect.position + new Vector2(i * tileMap.CellSize.x, j * tileMap.CellSize.y);
                       pos = tileMap.SnapPosition(pos);

                       if(area.ContainsPoint(pos))
                       {
                           if (tileMap.GetTile(pos.ToInt()) == null)
                           {
                               var tile = new ConnectedTile(pos, area.ID);
                               tileMap.AddTile(tile);
                           }
                       }
                   }
               }
           }
        }

        public override void ReCalculate(ref LBSLayer layer)
        {
            throw new NotImplementedException();
        }
        */
    }
}
