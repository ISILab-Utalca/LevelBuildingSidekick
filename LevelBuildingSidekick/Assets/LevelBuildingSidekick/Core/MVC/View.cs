using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public abstract class View : IEditorDrawable, I2DDrawable
    {
        public Controller Controller { get; set; }
        public View(Controller controller)
        {
            Controller = controller;
        }
        public virtual void Draw2D() { }

        public virtual void DrawEditor() { }

        public virtual void Display2DWindow() { }
        public virtual void DisplayInspectorWindow() { }
    }

    public interface IEditorDrawable
    {
        public void DrawEditor();
    }

    public interface I2DDrawable
    {
        public void Draw2D();
    }

}

