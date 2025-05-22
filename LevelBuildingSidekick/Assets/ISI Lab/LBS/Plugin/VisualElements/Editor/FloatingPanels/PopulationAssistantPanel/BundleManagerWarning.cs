using ISILab.Commons.Utility.Editor;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerWarning : VisualElement
    {
        private readonly Label _warningContent;

        public BundleManagerWarning()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerWarning");
            visualTree.CloneTree(this);
            
            _warningContent = this.Q<Label>("WarningContent");
        }

        public void SetWarningContent(string warningContent)
        {
            _warningContent.text = warningContent;
        }
    }
}
