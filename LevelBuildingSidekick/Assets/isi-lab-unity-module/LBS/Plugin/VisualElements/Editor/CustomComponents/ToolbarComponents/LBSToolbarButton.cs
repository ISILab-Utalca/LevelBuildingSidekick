using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;



namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSToolbarButton: ToolbarButton
    {
        readonly string lbsClassName = "lbs-toolbar-button";
        private Color iconColor = Color.white;


        [UxmlAttribute]
        public Color IconColor
        {
            get => iconColor;
            set
            {
                iconColor = value;
                Image imageVe = this.Q<Image>(classes:"unity-button__image");
                if (iconColor != Color.white && imageVe != null)
                {
                    //imageVe.style.unityBackgroundImageTintColor = new StyleColor(iconColor);
                    imageVe.tintColor = IconColor;
                    
                }
            }
        }
        
        
        
        public LBSToolbarButton() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(lbsClassName);
            
        }
    }
}

