using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    public abstract class LBSModule
    {
        bool visible;
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }
    }
}

