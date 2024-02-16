using ISILab.Commons.Utility.Editor;
using LBS.Bundles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class BundleAssetView : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<BundleAssetView, UxmlTraits> { }
        #endregion

        private Label label;
        private VisualElement icon;
        private VisualElement tab;

        private Bundle target;

        public BundleAssetView()
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("BundleAssetView");
            visualTree.CloneTree(this);

            label = this.Q<Label>("Name");
            icon = this.Q<VisualElement>("Icon");
            tab = this.Q<VisualElement>("Tab");
        }

        public void SetInfo(Bundle target, int value)
        {
            this.target = target;

            label.text = target.name;

            icon.style.backgroundImage = target.Icon;

            tab.style.width = 20 * value;
        }
    }
}