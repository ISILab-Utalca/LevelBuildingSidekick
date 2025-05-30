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
        public Label text;
        public Button button;

        public ActionButton()
        {
            var visualtree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ActionButton");
            visualtree.CloneTree(this);

            text = this.Q<Label>(name: "Action");
            button = this.Q<Button>(name: "Button");
        }

        public ActionButton(string text, Action action) : this()
        {
            this.text.text = char.ToUpper(text[0]) + text.Substring(1);
            button.clicked += action;
        }
    }
}