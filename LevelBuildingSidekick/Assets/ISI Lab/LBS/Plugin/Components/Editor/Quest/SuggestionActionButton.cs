using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class SuggestionActionButton : VisualElement
    {
        public readonly Label Label;
        private readonly Button _button;

        private SuggestionActionButton()
        {
            var visualtree = DirectoryTools.GetAssetByName<VisualTreeAsset>("SuggestionActionButton");
            visualtree.CloneTree(this);

            Label = this.Q<Label>(name: "Action");
            _button = this.Q<Button>(name: "Button");
        }

        public SuggestionActionButton(string text, Action action) : this()
        {
            Label.text = char.ToUpper(text[0]) + text.Substring(1);
            _button.clicked += action;
        }
    }
}