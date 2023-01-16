using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Teselation
{
    public class AreaModule : TeselationModule
    {
        List<Area> areas;

        public override void Clear()
        {
            areas.Clear();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}

