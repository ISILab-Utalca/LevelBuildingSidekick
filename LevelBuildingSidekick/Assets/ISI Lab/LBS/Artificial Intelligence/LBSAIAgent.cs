using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using LBS.Components;

namespace LBS.AI
{
    public abstract class LBSAIAgent
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

        public LBSAIAgent() { }

        public LBSAIAgent(LBSLayer layer, string id, string name)
        {
            this.layer = layer;
            ID = id;
            Name = name;
        }

        public abstract void Init(LBSLayer layer);

        public abstract void Execute();

        public abstract VisualElement GetInspector();
    }
}

