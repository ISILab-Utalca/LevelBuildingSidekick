using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS.Tools.Transformer
{

    public abstract class Transformer
    {
        public abstract void Switch();

        public abstract void OnAdd();
        public abstract void OnRemove();
    }
}

