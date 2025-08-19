using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;



namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSToolbarButton: ToolbarButton
    {
        readonly string lbsClassName = "lbs-toolbar-button";
        public LBSToolbarButton() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(lbsClassName);
        }
    }
}

