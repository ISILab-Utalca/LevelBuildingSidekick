using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using LBS.Components;
using System;

namespace LBS.AI
{
    [System.Serializable]
    public abstract class LBSAIAgent : ICloneable // unused (!!!)
    {
        protected LBSLayer layer;

        protected string id;

        protected string name;

        public LBSLayer Layer
        {
            get => layer;
        }

        public string ID
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public System.Action OnStart;
        public System.Action OnTermination;

        public LBSAIAgent() { }

        public LBSAIAgent(LBSLayer layer, string id, string name)
        {
            this.layer = layer;
            ID = id;
            Name = name;
        }

        public abstract void Init(ref LBSLayer layer);

        public abstract void Execute();

        public abstract VisualElement GetInspector();

        public abstract object Clone();
    }
}

