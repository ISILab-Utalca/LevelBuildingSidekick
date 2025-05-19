using ISILab.Commons.Utility.Editor;
using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ToolButton : VisualElement, IGrupable
    {
        #region FIELDS
        public Color color = LBSSettings.Instance.view.toolkitNormal;
        private Color selected = LBSSettings.Instance.view.newToolkitSelected;
        #endregion

        #region FIELDS VIEW
        private Button button;
        private VisualElement icon;
        #endregion

        #region EVENTS
        public event Action OnFocusEvent;
        public event Action OnBlurEvent;
        #endregion

        #region CONSTRUCTORS
        public ToolButton(LBSTool tool)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolButton");
            visualTree.CloneTree(this);
            
            button = this.Q<Button>("Button");
            
            icon = this.Q<VisualElement>("Icon");
            if (tool.Icon == null) return;
            icon.style.backgroundImage = new StyleBackground(tool.Icon); 
            
            tooltip = tool.Name;
        }
        #endregion

        #region IGRUPABLE
        public void AddGroupEvent(Action action)
        {
            button.clicked += action;
        }

        public void OnBlur()
        {
            button.style.backgroundColor = color;
            OnBlurEvent?.Invoke();
        }

        public void OnFocus()
        {
            button.style.backgroundColor = selected;
            OnFocusEvent?.Invoke();
        }

        public void OnFocusWithoutNotify()
        {
            button.style.backgroundColor = selected;
        }

        public void SetColorGroup(Color color, Color selected)
        {
            this.color = color;
            this.selected = selected;
        }

        public string GetLabel()
        {
            return tooltip;
        }
        #endregion
    }
}