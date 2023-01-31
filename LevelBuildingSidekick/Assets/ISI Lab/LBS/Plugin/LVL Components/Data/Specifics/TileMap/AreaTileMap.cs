using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class AreaTileMap<T, U> : TeselationModule where T : TiledArea<U> where U : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        //protected List<BasedTiledArea> areas;
        protected List<TiledArea<LBSTile>> areas;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public int RoomCount => areas.Count;
        [JsonIgnore]
        public List<T> Areas
        {
            get
            {
                var areas = new List<T>();
                foreach (var ar in this.areas)
                {
                    var tiles = new List<U>();
                    foreach (var tile in ar.Tiles)
                    {
                        tiles.Add(tile as U);
                        //var t = typeof(T).GetGenericArguments()[0];
                        //var c = Convert.ChangeType(ti, t);
                    }
                    var nArea = new TiledArea<U>(tiles,ar.ID,ar.Key,ar.Color) as T;
                    areas.Add(nArea);
                }
                return areas;
            }
        }

        #endregion

        #region CONSTRUCTOR
        
        public AreaTileMap() : base()
        {
            //areas = new List<BasedTiledArea>();
            areas = new List<TiledArea<LBSTile>>();
        }

        #endregion

        #region METHODS

        public void AddArea(T area)
        {
            //var a = new BasedTiledArea(area.Tiles, area.ID,area.Key);
            var tArea = new TiledArea<LBSTile>(area.Tiles, area.ID,area.Key,area.Color);
            areas.Add(tArea);
            area.OnAddTile = (t) => 
            {
                RemoveTile(t);
            };
        }

        private void RemoveTile(U t)
        {
            foreach(var r in areas)
            {
                if(r.Contains(t.Position))
                {
                    r.RemoveTile(t);
                }
            }
        }

        public U GetTileNeighbor(U tile, Vector2Int dir)
        {
            var pickedArea = areas.Find(a => a.Contains(tile.Position + dir));

            if (pickedArea == null)
                return null;

            return (U)pickedArea.GetTile(tile.Position + dir);
        }

        public TiledArea<LBSTile> GetArea(string id)
        {
            return areas.Find(r => r.Key == id);
        }

        public TiledArea<LBSTile> GetRoomPos(Vector2Int tilePos)
        {
            return areas.Find(r => r.Contains(tilePos));
        }

        public TiledArea<LBSTile> GetArea(int index)
        {
            return areas[index];
        }

        public bool RemoveArea(T area)
        {
            var x = area as TiledArea<LBSTile>; // (??) funciona 
            return areas.Remove(x);
        }

        private int GetRoomDistance(string r1, string r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var room1 = GetArea(r1);
            var room2 = GetArea(r2);
            for (int i = 0; i < room1.TileCount; i++)
            {
                var dist = room2.GetDistance(room1.GetTile(i).Position);

                if (dist <= lessDist)
                {
                    lessDist = dist;
                }
            }
            return lessDist;
        }

        public override void Clear()
        {
            areas.Clear();
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsEmpty()
        {
            return (areas.Count <= 0 || areas.Sum(a => a.TileCount) <= 0);
        }

        public override object Clone()
        {
            var atm = new AreaTileMap<T, U>();
            var nAreas = areas.Select(a => a.Clone() as T).ToList();// el "as" puede causar problemas
            foreach (var nArea in nAreas)
            {
                atm.AddArea(nArea);
            }
            
            return atm;
        }

        #endregion

    }


}

public class BasedTiledArea : TiledArea<LBSTile>
{
    public BasedTiledArea(List<LBSTile> tiles, string id, string key, Color color) 
        : base(tiles, id, key,color)
    {
    }
}

