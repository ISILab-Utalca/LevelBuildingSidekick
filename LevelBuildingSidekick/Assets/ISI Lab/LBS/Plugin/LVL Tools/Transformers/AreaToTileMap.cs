using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Teselation;
using LBS.Components.TileMap;



namespace LBS.Tools.Transformer
{
    public class AreaToTileMap : Transformer
    {
        AreaModule<Area> areaModule;
        TileMapModule<ConnectedTile> tileMap;

        public AreaToTileMap(AreaModule<Area> area, TileMapModule<ConnectedTile> tileMap)
        {
            this.areaModule = area;
            this.tileMap = tileMap;
        }

        public override void Switch()
        {
            for(int i = 0; i < tileMap.TileCount; i++)
            {
                if(!areaModule.ContainsPoint(tileMap.GetTile(i).Position))
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

        public override void OnAdd()
        {
            throw new System.NotImplementedException();
        }

        public override void OnRemove()
        {
            throw new System.NotImplementedException();
        }
    }
}
