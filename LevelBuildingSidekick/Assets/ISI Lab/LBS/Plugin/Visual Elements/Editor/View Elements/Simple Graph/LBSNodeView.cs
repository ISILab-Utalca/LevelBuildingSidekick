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
        
        private Color normalColor;
        // Set outline width here NOT in uxml
        private float normalBorder = 2f;
        
        private float hoverIncrease = 1.5f;
        
        #region VIEW FIELDS
        private static VisualTreeAsset view;

        private Label label;
        private VisualElement background;
        private VisualElement outline;
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
            
            outline = this.Q<VisualElement>("Outline");
            background = outline.Q<VisualElement>("Background");
            label = background.Q<Label>("Label");
      
            RegisterCallback<PointerEnterEvent>(OnHover);
            RegisterCallback<PointerLeaveEvent>(OnUnhover);
        }

        private void OnHover(PointerEnterEvent evt)
        {
            // Color change
            normalColor = background.style.backgroundColor.value;
            var _hoverColor = normalColor;
            _hoverColor.a = 1f;
            _hoverColor.r *= hoverIncrease;
            _hoverColor.g *= hoverIncrease;
            _hoverColor.b *= hoverIncrease;
            background.style.backgroundColor = _hoverColor;
            
            // Border
            background.style.borderTopWidth = hoverIncrease + normalBorder;
            background.style.borderBottomWidth = hoverIncrease + normalBorder;
            background.style.borderLeftWidth = hoverIncrease + normalBorder;
            background.style.borderRightWidth = hoverIncrease + normalBorder;
        }

        private void OnUnhover(PointerLeaveEvent evt)
        {
            background.style.backgroundColor = normalColor;
            
            background.style.borderTopWidth = normalBorder;
            background.style.borderBottomWidth = normalBorder;
            background.style.borderLeftWidth = normalBorder;
            background.style.borderRightWidth = normalBorder;
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