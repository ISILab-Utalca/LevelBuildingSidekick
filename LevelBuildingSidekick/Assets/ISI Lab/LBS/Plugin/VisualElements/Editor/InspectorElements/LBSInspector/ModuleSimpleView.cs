using ISILab.Commons.Utility.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class ModuleSimpleView : VisualElement
    {
        private Label labelName;
        private Label labelID;
        private VisualElement icon;

        public ModuleSimpleView()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ModuleSimpleView");
            visualTree.CloneTree(this);

            labelName = this.Q<Label>("LabelName");
            icon = this.Q<VisualElement>("Icon");
            labelID = this.Q<Label>("LabelID");
        }

        public void SetInfo(string name, string id = "", Texture2D icon = null)
        {
            labelName.text = name;


            labelID.text = id == "" ? "" : "[" + id + "]";

            if (icon != null)
                this.icon.style.backgroundImage = icon;
        }
    }
}