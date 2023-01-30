using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS.Generator
{
    public abstract class Generator
    {
        public abstract GameObject Generate(LBSLayer layer);

        public abstract void Init(LBSLayer layer);

        //public abstract void Init(Layer layer); // (!!) implementar
    }

    
}