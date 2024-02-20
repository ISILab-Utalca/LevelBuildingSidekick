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
        Label text;
        Button button;

        public ActionButton()
        {
            var visualtree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ActionButton");
            visualtree.CloneTree(this);

            text = this.Q<Label>(name: "Action");
            button = this.Q<Button>(name: "Button");
        }

        public ActionButton(string text, Action action) : this()
        {
            this.text.text = text;
            button.clicked += action;
        }
    }
}