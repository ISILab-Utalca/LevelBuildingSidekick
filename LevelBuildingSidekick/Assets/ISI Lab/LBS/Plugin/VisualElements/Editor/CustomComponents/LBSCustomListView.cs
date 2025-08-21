using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomListView: ListView
    {
        public LBSCustomListView() : base()
        {
            AddToClassList("lbs");
            AddToClassList("lbs-list-view");
        }
    }
    
}
