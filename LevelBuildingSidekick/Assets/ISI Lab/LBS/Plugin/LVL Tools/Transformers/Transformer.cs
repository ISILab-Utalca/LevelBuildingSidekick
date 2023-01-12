using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS.Tools
{

    public abstract class Transformer
    {
        public abstract void From();
        public abstract void To();

        public abstract void OnAdd();
        public abstract void OnRemove();
    }
}

