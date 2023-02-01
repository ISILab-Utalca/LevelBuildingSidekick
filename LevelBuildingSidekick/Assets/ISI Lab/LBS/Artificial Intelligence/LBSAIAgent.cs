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

        public LBSLayer Layer
        {
            get => layer;
        }

        public LBSAIAgent() { }

        public LBSAIAgent(LBSLayer layer)
        {
            this.layer = layer;
        }

        public abstract void Init(LBSLayer layer);

        public abstract void Execute();

        public abstract VisualElement GetInspector();
    }
}

