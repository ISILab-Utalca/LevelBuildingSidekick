using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using System;
using Newtonsoft.Json;

namespace LBS.Tools.Transformer
{
    [System.Serializable]
    [Obsolete("Los trasformar ya no suan devido a que la implentacion no era trivial")]
    public abstract class Transformer
    {
        [SerializeField]
        private string from;
        [SerializeField]
        private string to;

        [JsonIgnore]
        public Type From
        {
            get => Type.GetType(this.from);
            set => from = value?.FullName;
        }
        [JsonIgnore]
        public Type To
        {
            get => Type.GetType(this.to);
            set => to = value?.FullName;
        }

        public Transformer(Type from, Type to) 
        {
            this.From = from;
            this.To = to;
        }

        public abstract void Switch(ref LBSLayer layer);
        public abstract void ReCalculate(ref LBSLayer layer);
    }
}

