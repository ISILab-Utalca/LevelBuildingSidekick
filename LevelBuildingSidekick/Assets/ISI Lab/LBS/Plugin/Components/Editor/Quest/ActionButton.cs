using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ActionButton : VisualElement
    {
        private static ActionButton _activeButton;
        
        public readonly Label Label;
        private readonly Button _button;

        private ActionButton()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ActionButton");
            visualTree.CloneTree(this);

            Label = this.Q<Label>(name: "Action");
            _button = this.Q<Button>(name: "Button");
            _button.clicked += () =>
            {
                Highlight();
            };
        }

        public ActionButton(string text, Action action) : this()
        {
            Label.text = char.ToUpper(text[0]) + text.Substring(1);
            _button.clicked += action;
            AddToClassList("lbs-actionbutton");
        }

        private void Highlight()
        {
            if (_activeButton is not null && _activeButton != this)
            {
                _activeButton.RemoveFromClassList("lbs-actionbutton_selected");
            }

            _activeButton = this;
            AddToClassList("lbs-actionbutton_selected");
        }
    }
}