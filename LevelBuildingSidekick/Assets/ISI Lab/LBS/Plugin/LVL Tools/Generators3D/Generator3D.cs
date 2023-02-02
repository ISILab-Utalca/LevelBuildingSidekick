using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS.Generator
{
    [System.Serializable]
    public abstract class Generator3D
    {
        [SerializeField]
        protected Vector2 scale;
        [SerializeField]
        protected Vector2 resize;
        [SerializeField]
        protected Vector3 position;
        [SerializeField]
        protected string objName;

        public Vector2 Resize
        {
            get => resize;
            set => resize = value;
        }

        public Vector2 Scale
        {
            get => scale;
            set => scale = value;
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public string ObjName
        {
            get => objName;
            set => objName = value;
        }

        public abstract GameObject Generate(LBSLayer layer);

        public abstract void Init(LBSLayer layer);

        //public abstract void Init(Layer layer); // (!!) implementar
    }

    
}