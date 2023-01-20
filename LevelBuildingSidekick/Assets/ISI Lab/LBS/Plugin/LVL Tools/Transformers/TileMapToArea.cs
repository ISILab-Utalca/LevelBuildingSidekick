using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System.Linq;

namespace LBS.Tools.Transformer
{
    public class TileMapToArea : Transformer
    {
        AreaModule<Area> areaModule;
        TileMapModule<ConnectedTile> tileMap;

        public TileMapToArea(TileMapModule<ConnectedTile> tileMap, AreaModule<Area> area)
        {
            this.areaModule = area;
            this.tileMap = tileMap;
        }

        public override void Switch()
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
            return null;
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

