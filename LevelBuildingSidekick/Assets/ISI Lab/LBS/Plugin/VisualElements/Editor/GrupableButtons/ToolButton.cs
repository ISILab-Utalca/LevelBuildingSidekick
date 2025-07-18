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

        private Color _color = LBSSettings.Instance.view.toolkitNormal;
        private Color _selected = LBSSettings.Instance.view.newToolkitSelected;
        #endregion

        #region FIELDS VIEW
        private readonly Button _button;

        #endregion

        #region EVENTS
        public event Action OnFocusEvent;
        public event Action OnBlurEvent;
        #endregion

        #region CONSTRUCTORS
        public ToolButton(LBSTool tool)
        {
            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolButton");
            visualTree.CloneTree(this);
            
            _button = this.Q<Button>("Button");
            
            var icon = this.Q<VisualElement>("Icon");
            if (tool.Icon == null) return;
            icon.style.backgroundImage = new StyleBackground(tool.Icon); 
            
            tooltip = tool.Name;
        }
        #endregion

        #region IGRUPABLE
        public void AddGroupEvent(Action action)
        {
            _button.clicked += action;
        }

        public void OnBlur()
        {
            _button.style.backgroundColor = _color;
            OnBlurEvent?.Invoke();
        }

        public void OnFocus()
        {
            _button.style.backgroundColor = _selected;
            OnFocusEvent?.Invoke();
        }

        public void OnFocusWithoutNotify()
        {
            _button.style.backgroundColor = _selected;
        }

        public void SetColorGroup(Color color, Color selected)
        {
            _color = color;
            _selected = selected;
        }

        public string GetLabel()
        {
            return tooltip;
        }
        #endregion
    }
}