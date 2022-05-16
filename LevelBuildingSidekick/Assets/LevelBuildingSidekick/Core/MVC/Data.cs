using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick 
{
    [System.Serializable]
    public abstract class Data : ScriptableObject
    {
        public abstract Type ControllerType { get; }
    }

}
