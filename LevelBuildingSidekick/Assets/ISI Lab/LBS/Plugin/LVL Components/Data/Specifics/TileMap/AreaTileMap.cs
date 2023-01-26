using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using LBS.Components.Teselation;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class AreaTileMap<T,U> : TeselationModule where T : TiledArea<U> where U : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        protected List<TiledArea<LBSTile>> areas;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public int RoomCount => areas.Count;
        [JsonIgnore]
        public List<T> Areas => new List<T>(areas.Select( a => a as T));

        #endregion

        #region CONSTRUCTOR
        
        public AreaTileMap() : base()
        {
            areas = new List<TiledArea<LBSTile>>();
        }

        #endregion

        #region METHODS

        public bool AddArea(T area)
        {
            if (area == null)
                return false;
            if (GetArea(area.ID) != null)
                return false;
            var a = new TiledArea<LBSTile>(area.Tiles, area.ID,area.Key);
            areas.Add(a);
            area.OnAddTile = (t) => 
            {
                RemoveTile(t);
            };
            return true;
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

        public TiledArea<LBSTile> GetArea(string id)
        {
            return areas.Find(r => r.Key == id);
        }

        public TiledArea<LBSTile> GetRoom(int index)
        {
            return areas[index];
        }

        public bool RemoveRoom(T area)
        {
            var x = area as TiledArea<LBSTile>; // (??) funciona 
            return areas.Remove(x);
        }

        public int GetRoomDistance(string r1, string r2) // O2 - manhattan
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

        public override object Clone()
        {
            var atm = new AreaTileMap<T, U>();
            atm.areas = new List<TiledArea<LBSTile>>(areas);
            return atm;
        }

        #endregion

    }

}

