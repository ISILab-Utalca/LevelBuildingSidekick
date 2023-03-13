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
    public class AreaTileMap<T> : TeselationModule where T : TiledArea
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        //protected List<BasedTiledArea> areas;
        protected List<TiledArea> areas = new List<TiledArea>();

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public int AreaCount => areas.Count;
        [JsonIgnore]
        public List<T> Areas
        {
            get
            {
                var areas = this.areas.Select(a => a as T).ToList();
                return areas;
            }
        }

        #endregion

        #region CONSTRUCTOR
        
        public AreaTileMap() : base()
        {
            areas = new List<TiledArea>();
        }

        public AreaTileMap(List<TiledArea> areas, string key) : base(key)
        {
            this.areas = new List<TiledArea>();
            areas.ForEach(a => {
                AddArea(a as T);
            });
        }

        #endregion

        #region METHODS

        public void AddArea(T area)
        {
            if (areas.Any(a => a.ID == area.ID))
            {
                //Debug.Log("Area already exist");
                return;
            }
            areas.Add(area);
            area.Owner = this.Owner;
            area.OnAddTile = (t) => 
            {
                RemoveTile(t);
            };
        }

        public void AddTile(string areaID, LBSTile tile)
        {
            RemoveTile(tile);
            var a = GetArea(areaID);
            a.AddTile(tile);
        }

        public void RemoveTile(LBSTile t)
        {
            foreach(var r in areas)
            {
                if(r.Contains(t.Position))
                {
                    r.RemoveTile(t);
                }
            }
        }

        public void RemoveTile(Vector2Int pos)
        {
            
            foreach (var r in areas)
            {
                if (r.Contains(pos))
                {

                    r.RemoveAt(pos);
                }
            }
        }

        public LBSTile GetTileNeighbor(LBSTile tile, Vector2Int dir)
        {
            var pickedArea = areas.Find(a => a.Contains(tile.Position + dir));

            if (pickedArea == null)
                return null;

            return pickedArea.GetTile(tile.Position + dir);
        }

        public TiledArea GetArea(string id)
        {
            return areas.Find(r => r.ID.Equals(id));
        }

        public TiledArea GetArea(Vector2Int tilePos)
        {
            return areas.Find(r => r.Contains(tilePos));
        }

        public TiledArea GetArea(int index)
        {
            return areas[index];
        }

        public bool RemoveArea(T area)
        { 
            return areas.Remove(area);
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

        public override bool IsEmpty()
        {
            return (areas.Count <= 0 || areas.Sum(a => a.TileCount) <= 0);
        }

        public override object Clone()
        {
            var atm = new AreaTileMap<T>();
            var nAreas = areas.Select(a => a.Clone() as T).ToList();// el "as" puede causar problemas
            foreach (var nArea in nAreas)
            {
                atm.AddArea(nArea);
            }
            
            return atm;
        }

        public override void OnAttach(LBSLayer layer)
        {
            throw new NotImplementedException();
        }

        public override void OnDetach(LBSLayer layer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }


}

