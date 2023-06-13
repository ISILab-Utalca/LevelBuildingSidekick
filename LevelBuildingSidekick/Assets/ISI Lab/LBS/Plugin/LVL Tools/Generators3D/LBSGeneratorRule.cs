using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Generator
{
    [SerializeField, System.Serializable]
    public abstract class LBSGeneratorRule : ICloneable
    {
        [JsonIgnore]
        internal Generator3D generator3D;

        public LBSGeneratorRule() { }

        public abstract GameObject Generate(LBSLayer layer, Generator3D.Settings settings);

        public abstract bool CheckIfIsPosible(LBSLayer layer, out string msg); // mejorar nombre (!!!)

        public abstract object Clone();
    }
}

