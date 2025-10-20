using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    
    [UxmlElement]
    public partial class LBSToolbar: Toolbar
    {
        
        readonly string _lbsClassName = "lbs-toolbar";
        public LBSToolbar() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(_lbsClassName);
        }
    }
}


