using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class ConnectedTile : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired]
        private List<string> connections = new List<string>();

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string[] Connections => new List<string>(connections).ToArray();

        #endregion

        #region CONSTRUCTOR

        public ConnectedTile() : base ()
        {
            this.connections = new List<string>();
        }

        public ConnectedTile(Vector2 position, string id, int sides = 4, string[] connections = null) : base(position, id, sides)
        {
            if (connections == null)
                this.connections = new string[sides].ToList();
            else
                this.connections = connections.ToList(); 
        }

        #endregion

        #region METHODS

        public void SetConnections(params string[] connections)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if(this.connections.Count <= i)
                {
                    this.connections.Add(connections[i]);
                }
                else
                {
                    this.connections[i] = connections[i];
                }
            }
        }

        public void SetConnection(string connection, int index)
        {
            /*
            if(!connections.ContainsIndex(index))
            {
                return false;
            }

            if(connections[index] == connection)
            {
                return false;
            }
            */

            connections[index] = connection;
            //return true;
        }

        public string GetConnection(int index)
        {
            if(connections.ContainsIndex(index))
            {
                return connections[index];
            }
            return null;
        }

        public bool RemoveConnection(string connection)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            return new ConnectedTile(this.Position, this.ID, this.sides, new List<string>(this.connections).ToArray());
        }

        #endregion
    }
}

