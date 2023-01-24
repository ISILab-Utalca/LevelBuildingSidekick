using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using Newtonsoft.Json;

namespace LBS.Components.Specifics
{
    [System.Serializable]
    public class RoomNode : LBSNode
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        RoomData room;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public RoomData Room => room;

        #endregion

        #region CONSTRUCTOR

        public RoomNode(): base()
        {
            room = null;
        }

        public RoomNode(string id, Vector2 position, RoomData room) : base(id, position)
        {
            this.room = room;
        }

        #endregion;

        #region METHODS

        public override object Clone()
        {
            return new RoomNode(ID, Position, room.Clone() as RoomData);
        }

        #endregion
    }
}


