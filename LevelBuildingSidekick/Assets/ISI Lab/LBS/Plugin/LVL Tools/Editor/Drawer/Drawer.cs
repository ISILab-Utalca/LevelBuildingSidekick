using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Drawers
{
    [Serializable]
    public abstract class Drawer
    {
        public Vector2 DefalutSize
        {
            get => LBSSettings.Instance.general.TileSize;
        }

        public Drawer() { }

        public abstract void Draw(object target, MainView view, Vector2 teselationSize);

        public virtual void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize) { }

        public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            Debug.LogWarning("[ISI Lab]: Texture generation not implemented.");
            return new Texture2D(16, 16);
        }
    }
}