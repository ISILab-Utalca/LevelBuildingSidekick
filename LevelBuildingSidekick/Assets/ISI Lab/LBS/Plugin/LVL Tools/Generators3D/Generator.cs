using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Generator
{
    public abstract class Generator
    {
        public abstract GameObject Generate();

        public abstract void Init(LevelDataOld levelData);

        //public abstract void Init(Layer layer); // (!!) implementar
    }

    
}