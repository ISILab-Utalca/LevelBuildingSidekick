using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public abstract class Controller
    {
        public string Name { get; set; }
        public View View { get; set; }
        public Data Data { get; set; }
        public abstract void Update();

        public abstract void LoadData();
    }
}


