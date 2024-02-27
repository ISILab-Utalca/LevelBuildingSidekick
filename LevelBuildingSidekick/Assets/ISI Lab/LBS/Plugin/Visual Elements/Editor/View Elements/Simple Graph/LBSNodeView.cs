using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;

namespace ISILab.LBS.VisualElements
{
    public class LBSNodeView : GraphElement
    {
        public readonly Color unselected = Color.white;
        public readonly Color selcted = new Color(150f / 255f, 243f / 255f, 255f / 255f);

        #region VIEW FIELDS
        private static VisualTreeAsset view;

        private Label label;
        private VisualElement background;
        #endregion

        #region EVENTS
        public Action<Rect> OnMoving;
        #endregion

        public LBSNodeView()
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeUxml");
            }
            view.CloneTree(this);

            // Label
            label = this.Q<Label>();

            // Background
            background = this.Q<VisualElement>("Background");
        }

        public void SetColor(Color color)
        {
            background.style.backgroundColor = color;
        }

        public void SetText(string text)
        {
            if (text.Length > 11)
            {
                text = text.Substring(0, 8) + "...";
            }

            label.text = text;
        }

        public override void SetPosition(Rect newPos)
        {
            // Set new Rect position
            base.SetPosition(newPos);

            // call movement event
            OnMoving?.Invoke(newPos);

            MarkDirtyRepaint();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            background.SetBorder(selcted, 8);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            background.SetBorder(unselected, 8);
        }
    }
}