using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ActionButton : VisualElement
    {
        public readonly Label Label;
        private readonly Button _button;

        private ActionButton()
        {
            var visualtree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ActionButton");
            visualtree.CloneTree(this);

            Label = this.Q<Label>(name: "Action");
            _button = this.Q<Button>(name: "Button");
        }

        public ActionButton(string text, Action action) : this()
        {
            Label.text = char.ToUpper(text[0]) + text.Substring(1);
            _button.clicked += action;
        }
    }
}