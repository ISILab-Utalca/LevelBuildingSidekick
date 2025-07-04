using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Editor.Windows;
using LBS.Components;
using ISILab.LBS.Settings;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ISILab.LBS.Drawers
{
    [Serializable]
    public abstract class Drawer
    {
        protected bool Loaded = false;
        public Vector2 DefalutSize
        {
            get => LBSSettings.Instance.general.TileSize;
        }
        
        public Drawer() { }

        public abstract void Draw(object target, MainView view, Vector2 teselationSize);

        #region RECOMMENDED METHODS
        // To optimally handle the visual elements, it is recommended that a drawer's Draw method contains the next steps.
        // Not all uses of a drawer needs to use all of them, but they can be used as a general guideline.
        
        // Most drawers must load the tiles that have already been created, whether it was from a missing
        // function, or loading the map from a save file.
        protected void LoadAllTiles() { throw new NotImplementedException(); }
        
        // When a new visualElement must be created, a representative object is stored in a related behaviour class,
        // then, the Drawer gets them and create visualElements with the information associated to the objects.
        protected void PaintNewTiles() { throw new NotImplementedException(); }
        
        // Some visualElements need to be updated through their life cycle, to do this, you can use the Keys property
        // in a related behaviour class. This property stores all objects used to create a visualElement, and you
        // should also use it in LoadAllTiles.
        // With these keys, you can ask the MainView for a the visualElements associated with it, and update its
        // parameters.
        protected void UpdateLoadedTiles() { throw new NotImplementedException(); }
        #endregion

        public abstract void HideVisuals(object target, MainView view);
        public abstract void ShowVisuals(object target, MainView view);
        
        public virtual void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize) { }

        public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            LBSMainWindow.MessageNotify("Texture generation not implemented.", LogType.Warning);
            return new Texture2D(16, 16);
        }

    }
}