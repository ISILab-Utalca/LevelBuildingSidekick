using ISILab.Commons.Utility.Editor;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
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
