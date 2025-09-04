using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomTreeView: TreeView   {
        public LBSCustomTreeView(): base()
        {
            AddToClassList("lbs-tree-view");
        }
    } 
}


