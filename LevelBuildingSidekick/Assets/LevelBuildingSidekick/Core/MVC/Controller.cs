using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public abstract class Controller
    {
        View view;
        Data data;
        public abstract void Update();
    }
}


