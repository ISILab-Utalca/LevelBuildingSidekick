using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{

    [UxmlElement]
    public partial class LBSCustomImage : Image
    {
        public LBSCustomImage(): base()
        {
            this.AddToClassList("lbs-icon");
        }
    }

}
