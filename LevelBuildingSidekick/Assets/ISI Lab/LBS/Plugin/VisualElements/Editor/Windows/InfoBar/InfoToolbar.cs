using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class InfoToolbar: VisualElement
    {
        public VisualTreeAsset VisualTree;
        
        public InfoToolbar()
        {
            VisualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("InfoToolbar");
            VisualTree.CloneTree(this);
        }
    }
}


