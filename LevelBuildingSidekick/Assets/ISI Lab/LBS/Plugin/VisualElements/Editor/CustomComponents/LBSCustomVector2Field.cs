using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomVector2Field: Vector2Field, ILBSField
    {
        
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSFieldClassName = "lbs-vector2-field";
        
        public LBSCustomVector2Field() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
            AddToClassList(LBSFieldClassName);

            //VisualElement inputSpace = this.Q<VisualElement>(classes: inputUssClassName);
            VisualElement spacer = this.Q<VisualElement>(classes: spacerUssClassName);
            spacer.SendToBack();
            

        }
    }
}

