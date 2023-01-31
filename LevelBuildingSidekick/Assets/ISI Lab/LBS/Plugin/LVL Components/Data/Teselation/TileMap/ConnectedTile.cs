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

        [SerializeField, JsonRequired, SerializeReference]
        string[] connections;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string[] Connections => connections.Select(s => s).ToArray();


        #endregion

        #region CONSTRUCTOR

        public ConnectedTile() : base ()
        {
            connections = new string[Sides];
        }

        public ConnectedTile(Vector2 position, string id, int sides = 4) : base(position, id, sides)
        {
            connections = new string[Sides];
        }

        #endregion

        #region METHODS

        public void SetConnections(params string[] connections)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                this.connections[i] = connections[i];
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

        #endregion
    }
}

