using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick 
{
    [System.Serializable]
    public abstract class Data
    {
        [JsonIgnore, HideInInspector]
        public abstract Type ControllerType { get; }
    }

}
