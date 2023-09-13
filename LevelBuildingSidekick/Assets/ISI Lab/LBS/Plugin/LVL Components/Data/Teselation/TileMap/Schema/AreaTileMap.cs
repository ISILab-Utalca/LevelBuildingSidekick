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
    [Obsolete("UWU")]
    public class AreaTileMap<T> : LBSModule where T : TiledArea
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

        public AreaTileMap(IEnumerable<TiledArea> areas, string key) : base(key)
        {
            this.areas = new List<TiledArea>();
            var ars = areas.Cast<T>();
            foreach(var a in ars)
            {
                AddArea(a);
            }
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
        }

        public void AddTile(string areaID, LBSTile tile)
        {
            RemoveTile(tile);
            var a = GetArea(areaID);
            a.AddTile(tile);
        }

        public void RemoveTile(LBSTile t)
        {
            RemoveTile(t.Position);
        }

        public void RemoveTile(Vector2Int pos)
        {
            foreach (var r in areas)
            {
                if (r.GetTile(pos) != null)
                {
                    r.RemoveAt(pos);
                }
            }
        }

        public LBSTile GetTileNeighbor(LBSTile tile, Vector2Int dir)
        {
            var pickedArea = areas.Find(a => a.GetTile(tile.Position + dir) != null);

            if (pickedArea == null)
                return null;

            return pickedArea.GetTile(tile.Position + dir);
        }

        public void MoveArea(int index, Vector2Int dir)
        {
            var area = areas[index];
            area.Centroid += dir;
            foreach(var a in areas)
            {
                if(a.ID != area.ID)
                {
                    foreach(var t in a.Tiles)
                    {
                        a.RemoveTile(t);
                    }
                }
            }
        }

        public TiledArea GetArea(string id)
        {
            return areas.Find(r => r.ID.Equals(id));
        }

        public TiledArea GetArea(Vector2Int tilePos)
        {
            return areas.Find(r => r.GetTile(tilePos) != null);
        }

        public TiledArea GetArea(int index)
        {
            return areas[index];
        }

        public LBSTile GetTile(Vector2Int position)
        {
            foreach(var a in areas)
            {
                var t = a.GetTile(position);
                if (t != null)
                    return t;
            }
            return null;
        }

        public bool RemoveArea(T area)
        { 
            return areas.Remove(area);
        }

        public float GetRoomDistance(string r1, string r2) // O2 - manhattan
        {
            //var lessDist = float.MaxValue;
            
            var room1 = GetArea(r1);
            var room2 = GetArea(r2);

            /*
            var tileWalls1 = room1.GetWalls().SelectMany(x => x.Tiles).ToList();
            var tileWalls2 = room2.GetWalls().SelectMany(x => x.Tiles).ToList();

            for (int i = 0; i < tileWalls1.Count; i++)
            {
                for (int j = 0; j < tileWalls2.Count; j++)
                {
                    var dist = Vector2Int.Distance(tileWalls1[i], tileWalls2[j]);
                    if (dist <= lessDist)
                    {
                        lessDist = dist;
                    }
                }
            }
            return lessDist;
            */
            var lessDist = room1.Tiles.Min(t => room2.GetDistance(t.Position));
            /*for (int i = 0; i < room1.TileCount; i++)
            {
                var dist = room2.GetDistance(room1.GetTile(i).Position);

                if (dist <= lessDist)
                {
                    lessDist = dist;
                }
            }*/
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
            var nAreas = areas.Select(a => a.Clone()).Cast<T>();
            return new AreaTileMap<T>(nAreas, id);
        }

        public override void OnAttach(LBSLayer layer)
        {

        }

        public override void OnDetach(LBSLayer layer)
        {

        }

        public override Rect GetBounds()
        {
            if(Areas.Count() <= 0)
            {
                return new Rect(0,0,1,1);
            }

            var x = Areas.Min(a => a.GetBounds().min.x);
            var y = Areas.Min(a => a.GetBounds().min.y);
            var width = Areas.Max(a => a.GetBounds().max.x) - x;
            var height = Areas.Max(a => a.GetBounds().max.y) - y;
            return new Rect(x, y, width, height);
        }

        public List<Vector2> OccupiedPositions()
        {
            var occupied = new List<Vector2>();

            /*foreach(var a in areas)
            {
                occupied.AddRange(a.OccupiedPositions());
            }*/

            return occupied;
        }

        public List<Vector2> EmptyPositions()
        {
            var r = GetBounds();
            var occupied = OccupiedPositions();

            List<Vector2> empty = new List<Vector2>();

            for (int j = 0; j < r.height; j++)
            {
                for (int i = 0; i < r.width; i++)
                {
                    var v = new Vector2(i, j) + r.position;
                    if (!occupied.Contains(v))
                    {
                        empty.Add(v);
                    }
                }
            }

            return empty;
        }

        public List<int> OccupiedIndexes()
        {
            var r = GetBounds();
            return OccupiedPositions().Select((v,x) => x).ToList();
        }

        public List<int> EmptyIndexes()
        {
            var r = GetBounds();
            return EmptyPositions().Select((v, x) => x).ToList();
        }

        public override void Rewrite(LBSModule module)
        {
            throw new NotImplementedException();
        }

        public override void Reload(LBSLayer layer)
        {

        }

        #endregion

    }


}

