using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSSideBarPanel: VisualElement
    {
        public LBSSideBarPanel(): base()
        {
            VisualTreeAsset visualTreeAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSSideBarPanel");
            visualTreeAsset.CloneTree(this);
            
            
            
        }
    }
}
