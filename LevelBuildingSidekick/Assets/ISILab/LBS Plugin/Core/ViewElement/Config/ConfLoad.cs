using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ConfLoad : Button
    {
        public new class UxmlFactory : UxmlFactory<ConfLoad, VisualElement.UxmlTraits> { }

        public Label fileSave;

        public ConfLoad()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConfigLoadUXML");
            visualTree.CloneTree(this);

            fileSave = this.Q<Label>(name: "fileSave");

            fileSave.text = "Default";
        }

        public ConfLoad(string fileS)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConfigLoadUXML");
            visualTree.CloneTree(this);

            fileSave = this.Q<Label>(name: "fileSave");

            fileSave.text = fileS;
        }
    }
}
