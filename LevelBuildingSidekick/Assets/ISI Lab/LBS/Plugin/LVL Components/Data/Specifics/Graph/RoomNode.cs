using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;

namespace LBS.Components.Specifics
{
    public class RoomNode : LBSNode
    {
        RoomData room;

        public RoomData Room => room;

        public RoomNode(): base()
        {
            room = null;
        }

        public RoomNode(RoomData room) : base()
        {
            this.room = room;
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}


