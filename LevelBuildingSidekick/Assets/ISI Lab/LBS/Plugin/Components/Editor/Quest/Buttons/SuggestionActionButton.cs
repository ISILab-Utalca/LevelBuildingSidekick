using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Button to apply an effect to a Quest Action
    /// </summary>
    [UxmlElement]
    public partial class SuggestionActionButton : VisualElement
    {
        private readonly Label _label;
        private readonly Button _button;
        
        public SuggestionActionButton()
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

        public void SetIconColor(Color viewWarningColor)
        {
            var icon = this.Q<VisualElement>(name: "Icon");
            icon.style.unityBackgroundImageTintColor = viewWarningColor;
        }
    }
}