using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace LBS
{
    [System.Serializable]
    public abstract class Controller
    {
        public VisualElement View { get; set; }
        public Data Data { get; set; }

        public Controller(Data data)
        {
            Data = data;
            //Debug.Log(this);
            LoadData();
        }

        public abstract void Update(); // no se estan usando (!!!!)

        public abstract void LoadData(); // no se estan usando (!!!!)

    }
}


