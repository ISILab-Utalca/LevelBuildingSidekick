using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS.Generator
{
    public abstract class Generator
    {
        protected Vector2 scale;
        protected Vector3 position;
        protected string objName;

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