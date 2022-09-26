using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class Element : Button
    {
        public new class UxmlFactory : UxmlFactory<Element, VisualElement.UxmlTraits> { }

        public VisualElement image;
        public Label nameElement;

        public Element()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ElementUSS");
            visualTree.CloneTree(this);

            nameElement = this.Q<Label>(name: "NameE");
            image = this.Q<VisualElement>(name: "image");

            nameElement.text = "Name Element";
        }

        public Element(string labelText, Texture2D texture)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ElementUSS");
            visualTree.CloneTree(this);

            nameElement = this.Q<Label>(name: "NameE");
            image = this.Q<VisualElement>(name: "image");

            nameElement.text = labelText;
            image.style.backgroundImage = texture;
        }
    }
}