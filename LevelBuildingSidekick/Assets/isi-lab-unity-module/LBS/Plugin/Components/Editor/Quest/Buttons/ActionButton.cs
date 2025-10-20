using ISILab.Commons.Utility.Editor;
using System;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    [UxmlElement]
    public partial class ActionButton : VisualElement
    {
        private static ActionButton _activeButton;
        
        public readonly Label Label;
        private  Button _button;

        public  ActionButton()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ActionButton");
            visualTree.CloneTree(this);
            AddToClassList("lbs-quest-list-item");
            Label = this.Q<Label>(name: "Action");
            _button = this.Q<Button>(name: "Button");
            _button.RemoveFromClassList(Button.ussClassName);
            _button.clicked += Highlight;
        }

        public ActionButton(string text, Action action) : this()
        {
            Label.text = char.ToUpper(text[0]) + text.Substring(1);
            _button.clicked += action;
        }

        private void Highlight()
        {
            if (_button is not null)
            {
                _button.RemoveFromClassList(".lbs-action-button_selected");
            }

            _button = this.Q<Button>(name: "Button");
            _button.AddToClassList(".lbs-action-button_selected");
        }
    }
}