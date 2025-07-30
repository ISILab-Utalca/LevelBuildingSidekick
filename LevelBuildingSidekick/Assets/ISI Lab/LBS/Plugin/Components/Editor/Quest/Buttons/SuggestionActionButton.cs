using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Button to apply an effect to a Quest Action
    /// </summary>
    public class SuggestionActionButton : VisualElement
    {
        private readonly Label _label;
        private readonly Button _button;
        
        private SuggestionActionButton()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("SuggestionActionButton");
            visualTree.CloneTree(this);

            _label = this.Q<Label>(name: "Action");
            _button = this.Q<Button>(name: "Button");
        }

        public SuggestionActionButton(string text = "", Action action = null) : this()
        {
          SetAction(text,action);
          AddToClassList("lbs-actionbutton");
        }

        public void SetAction(string text, Action action)
        {
            if(text == string.Empty || action is null) return;
            _label.text = char.ToUpper(text[0]) + text.Substring(1);
            _button.clicked += action;
        }
    }
}