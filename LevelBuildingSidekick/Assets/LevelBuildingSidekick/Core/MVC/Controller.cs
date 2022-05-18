using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public abstract class Controller
    {
        public View View { get; set; }
        public Data Data { get; set; }

        public Controller(Data data)
        {
            Data = data;
            LoadData();
        }

        public abstract void Update();

        public abstract void LoadData();
    }
}


