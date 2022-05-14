using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public abstract class View
    {
        public Controller Controller { get; set; }
        public abstract void Draw(Rect rect);
    }
}

