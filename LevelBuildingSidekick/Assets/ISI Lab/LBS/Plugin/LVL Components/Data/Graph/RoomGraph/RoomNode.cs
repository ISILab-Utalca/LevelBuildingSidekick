using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using Newtonsoft.Json;
using System;

namespace LBS.Components.Specifics
{
    [System.Serializable]
    public class RoomNode : LBSNode
    {
        #region FIELDS

        [SerializeField, JsonRequired]
        private RoomData room;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public RoomData Room => room;

        #endregion

        #region CONSTRUCTOR

        public RoomNode(): base()
        {
        }

        public RoomNode(string id, Vector2 position, RoomData room) : base(id, position)
        {
            this.room = room;
        }

        #endregion;

        #region METHODS

        public override object Clone()
        {
            return new RoomNode(this.ID, this.Position, this.room.Clone() as RoomData);
        }

        #endregion
    }
}


