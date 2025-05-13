using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerElement : VisualElement
    {
        public Label bundleName;
        public BundleManagerElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerElement");
            visualTree.CloneTree(this);
            
            bundleName = this.Q<Label>("BundleName");
        }
    }
}
