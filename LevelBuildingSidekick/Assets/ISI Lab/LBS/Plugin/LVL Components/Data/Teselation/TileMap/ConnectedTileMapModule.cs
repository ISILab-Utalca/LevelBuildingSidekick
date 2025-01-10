using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [System.Serializable]
    public class ConnectedTileMapModule : LBSModule
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private int connectedDirections = 4;

        [SerializeField, JsonRequired, SerializeReference]
        private List<TileConnectionsPair> pairs = new List<TileConnectionsPair>();
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public int ConnecteDirections
        {
            get => connectedDirections;
            set => connectedDirections = value;
        }

        [JsonIgnore]
        public List<TileConnectionsPair> Pairs => new List<TileConnectionsPair>(pairs);
        #endregion

        #region EVENTS
        public event Action<ConnectedTileMapModule, TileConnectionsPair> OnAddPair;
        public event Action<ConnectedTileMapModule, TileConnectionsPair> OnRemovePair;
        #endregion

        #region CONSTRUCTORS
        public ConnectedTileMapModule() : base()
        {
            id = GetType().Name;
        }

        public ConnectedTileMapModule(IEnumerable<TileConnectionsPair> tiles, int connectedDirections, string id = "ConnectedTileMapModule") : base(id)
        {
            this.connectedDirections = connectedDirections;
            foreach (var t in tiles)
            {
                AddPair(t.Tile, t.Connections, t.EditedByIA);
            }
        }
        #endregion

        #region METHODS
        public void SetConnection(LBSTile tile, int direction, string connection, bool editedByIA)
        {
            var pair = GetPair(tile);

            var old = new TileConnectionsPair(tile, pair.Connections, pair.EditedByIA);

            pair.SetConnection(direction, connection, editedByIA);

            OnChanged?.Invoke(this, new List<object>() { old }, new List<object>() { pair });
        }

        public void SetConnections(LBSTile tile, List<string> connections, List<bool> canBeEditedByIA)
        {
            var pair = GetPair(tile);

            var old = new TileConnectionsPair(tile, pair.Connections, pair.EditedByIA);

            pair.SetConnections(connections, canBeEditedByIA);

            OnChanged?.Invoke(this, new List<object>() { old }, new List<object>() { pair });
        }

        public void AddPair(LBSTile tile, List<string> connections, List<bool> canBeEditedByIA)
        {
            var pair = new TileConnectionsPair(tile, connections, canBeEditedByIA);
            var current = GetPair(pair.Tile);
            if (current != null)
            {
                pairs.Remove(current);
                OnRemovePair?.Invoke(this, current);
            }
            pairs.Add(pair);

            OnChanged?.Invoke(this, null, new List<object>() { pair });
            OnAddPair?.Invoke(this, pair);
        }

        public TileConnectionsPair GetPair(LBSTile tile)
        {
            if (pairs.Count <= 0)
                return null;
            return pairs.Find(t => t.Tile.Equals(tile));
        }

        public List<string> GetConnections(LBSTile tile)
        {
            var p = GetPair(tile);
            return p.Connections;
        }

        public void RemoveTile(LBSTile tile)
        {
            var pair = GetPair(tile);
            pairs.Remove(pair);
            OnChanged?.Invoke(this, new List<object>() { pair }, null);
            OnRemovePair?.Invoke(this, pair);
        }

        public void RemoveTile(int index)
        {
            var pair = pairs[index];
            pairs.RemoveAt(index);

            OnChanged?.Invoke(this, new List<object>() { pair }, null);
            OnRemovePair?.Invoke(this, pair);
        }

        public override Rect GetBounds()
        {
            if (pairs.Count == 0)
            {
                return default(Rect);
            }
            return pairs.Select(t => t.Tile).GetBounds();
        }

        public override bool IsEmpty()
        {
            return pairs.Count <= 0;
        }

        public override void Clear()
        {
            pairs.Clear();
        }

        public override object Clone()
        {
            var pairs = this.pairs.Select(t => t.Clone()).Cast<TileConnectionsPair>();
            var clone = new ConnectedTileMapModule(pairs, connectedDirections, ID);
            return clone;
        }

        public override void Rewrite(LBSModule module)
        {
            var connectedTileMap = module as ConnectedTileMapModule;
            if (connectedTileMap == null)
            {
                return;
            }
            Clear();
            connectedDirections = connectedTileMap.connectedDirections;
            foreach (var t in connectedTileMap.pairs)
            {
                AddPair(t.Tile, t.Connections, t.EditedByIA);
            }
        }

        public override void Print()
        {
            string msg = "";
            msg += "Type: " + GetType() + "\n";
            msg += "Hash code: " + GetHashCode() + "\n";
            msg += "ID: " + ID + "\n";
            msg += "\n";
            foreach (var pair in pairs)
            {
                msg += pair.Tile.Position + " - ";
                foreach (var connect in pair.Connections)
                {
                    msg += connect + "| ";
                }
                msg += "\n";
            }
            Debug.Log(msg);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ConnectedTileMapModule;

            if (other == null) return false;

            if (other.connectedDirections != this.connectedDirections) return false;

            var pCount = this.pairs.Count;

            if (pCount != other.pairs.Count) return false;

            for (int i = 0; i < pCount; i++)
            {
                var p1 = other.pairs[i];
                var p2 = this.pairs[i];

                if (!p1.Equals(p2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }

    [System.Serializable]
    public class TileConnectionsPair : ICloneable
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired]
        private LBSTile tile;

        [SerializeField, SerializeReference, JsonRequired]
        private List<string> connections = new List<string>();

        [SerializeField, SerializeReference, JsonRequired]
        private List<bool> editedByIA = new List<bool>();
        #endregion

        #region PROEPRTIES
        [JsonIgnore]
        public LBSTile Tile
        {
            get => tile;
        }

        [JsonIgnore]
        public List<string> Connections
        {
            get => connections;
        }

        public List<bool> EditedByIA => editedByIA;
        #endregion

        #region CONSTRUCTORS
        public TileConnectionsPair(LBSTile tile, IEnumerable<string> connections, List<bool> editedByIA)
        {
            this.tile = tile;
            this.connections = connections.ToList();
            this.editedByIA = editedByIA;
        }
        #endregion

        #region METHODS
        public void SetConnections(IEnumerable<string> connections, List<bool> editedByIA)
        {
            this.connections = new List<string>(connections);
            this.editedByIA = new List<bool>(editedByIA);
        }

        public void SetConnection(int index, string connection, bool editedByIA)
        {
            this.connections[index] = connection;
            this.editedByIA[index] = editedByIA;
        }

        public object Clone()
        {
            return new TileConnectionsPair(
                CloneRefs.Get(tile) as LBSTile,
                connections.Select(c => c.Clone() as string),
                new List<bool>(editedByIA)
                );
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileConnectionsPair;

            if (other == null) return false;

            if (!this.tile.Equals(other.tile)) return false;

            var cCount = other.connections.Count;

            for (int i = 0; i < cCount; i++)
            {
                var c1 = this.connections[i];
                var c2 = other.connections[i];

                if (!c1.Equals(c2)) return false;
            }

            var eCount = other.editedByIA.Count;

            for (int i = 0; i < eCount; i++)
            {
                var e1 = this.editedByIA[i];
                var e2 = other.editedByIA[i];

                if (!e1.Equals(e2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}