using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomObjectField: ObjectField
    {
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSFieldClassName = "lbs-object-field";
        
        public LBSCustomObjectField() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
            AddToClassList(LBSFieldClassName);
        }
    }
    
}
