using System;
using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Internal;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    public abstract class ExteriorTileView : GraphElement
    {
        protected static float scaleFactor = 1f;
        protected static Color fillColor = Color.grey;
        protected static float colorBrightenner = 0.1f;
        public ExteriorTileView(List<string> connections, string viewElementName)
        {
            this.SetMargins(0);
            this.SetPaddings(0);

            this.SetBackgroundColor(fillColor);
            this.SetBorderRadius(0);
            this.transform.scale = Vector3.one * scaleFactor;
        }

        protected static void SetBackgroundColor(VisualElement ve, Color color)
        {
            if (ve == null) return;
            ve.style.backgroundColor = color;
        }

        protected static void SetImageTint(VisualElement ve, Color color)
        {
            if (ve == null) return;
            if (ve.style.backgroundImage == null) return;
            ve.style.unityBackgroundImageTintColor = color;

        }

        protected static void SetBorderColor(VisualElement ve, Color color)
        {
            if (ve == null) return;
            ve.style.borderBottomColor = color;
            ve.style.borderLeftColor = color;
            ve.style.borderTopColor = color;
            ve.style.borderRightColor = color;
            
        }

        public abstract void SetConnections(string[] tags);

        public virtual void SetTileCenter(LBSTag identifier)
        {
            throw new NotImplementedException();
        }

        protected static Color BrightenColor(Color color)
        {
            color.r += colorBrightenner;
            color.b += colorBrightenner;
            color.g += colorBrightenner;
            return color;
        }
    }
}