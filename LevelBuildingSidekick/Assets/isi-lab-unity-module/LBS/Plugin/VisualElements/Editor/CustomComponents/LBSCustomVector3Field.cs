using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomVector3Field: Vector3Field,  ILBSField
    {
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSFieldClassName = "lbs-vector3-field";
        public LBSCustomVector3Field() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
            AddToClassList(LBSFieldClassName);
        }
    }
}
