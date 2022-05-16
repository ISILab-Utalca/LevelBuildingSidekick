using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public abstract class View
    {
        public Controller Controller { get; set; }
        public View(Controller controller)
        {
            Controller = controller;
        }
        public abstract void Draw();

        public abstract void Display();
    }
}

