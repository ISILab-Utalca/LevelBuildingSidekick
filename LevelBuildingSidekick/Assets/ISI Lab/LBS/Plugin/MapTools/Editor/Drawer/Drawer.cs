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
        protected Dictionary<object, List<GraphElement>> StoredGraphElements;
        protected List<object> RepaintList;
        public Vector2 DefalutSize
        {
            get => LBSSettings.Instance.general.TileSize;
        }
        
        public Drawer() { }

        public abstract void Draw(object target, MainView view, Vector2 teselationSize);

        public virtual void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize) { }

        public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            LBSMainWindow.MessageNotify("Texture generation not implemented.", LogType.Warning);
            return new Texture2D(16, 16);
        }

        public virtual GraphElement[] GetRepaintElements()
        {
            List<GraphElement> elements = new List<GraphElement>();
            foreach (object o in RepaintList)
            {
                if (StoredGraphElements.TryGetValue(o, out var element))
                {
                    element.AddRange(element);
                }
            }
            RepaintList.Clear();
            return elements.ToArray();
        }
    }
}