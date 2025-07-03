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
        
        
        protected void PaintNewTiles() { throw new NotImplementedException(); }
        protected void UpdateLoadedTiles() { throw new NotImplementedException(); }
        protected void LoadAllTiles() { throw new NotImplementedException(); }
        protected void UpdateTileView() { throw new NotImplementedException(); }

        public abstract void HideVisuals(object target, MainView view, Vector2 teselationSize);
        public abstract void ShowVisuals(object target, MainView view, Vector2 teselationSize);
        
        public virtual void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize) { }

        public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            LBSMainWindow.MessageNotify("Texture generation not implemented.", LogType.Warning);
            return new Texture2D(16, 16);
        }

    }
}