using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    
    [UxmlElement]
    public partial class LBSCustomButton: Button
    {
        public readonly String LBSClassName = "lbs-button";
        
        [UxmlAttribute]
        public Color ButtonTint
        {
            get => buttonTint;
            set
            {
                buttonTint = value;
                if (tintOverlayElement != null)
                {
                    if (buttonTint != Color.white)
                    {
                        
                        tintOverlayElement.style.backgroundColor = new StyleColor(buttonTint); 
                        tintOverlayElement.style.display = DisplayStyle.Flex;
                    
                    }
                    else
                    {
                        tintOverlayElement.style.display = DisplayStyle.None;
                    }
                }

            }
        }

        private Color buttonTint = Color.white;
        private Color baseTint = Color.white;
        
        private VisualElement tintOverlayElement;
        public LBSCustomButton() : base()
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSClassName);
            baseTint = this.style.backgroundColor.value;
            tintOverlayElement = new VisualElement();
            tintOverlayElement.AddToClassList("lbs-button-tint-overlay");
            this.Add(tintOverlayElement);
            tintOverlayElement.SendToBack();
            tintOverlayElement.style.position = Position.Absolute;
            tintOverlayElement.pickingMode = PickingMode.Ignore;
            if (baseTint != Color.white)
            {
                tintOverlayElement.style.backgroundColor = new StyleColor(baseTint);
            }
            else
            {
                tintOverlayElement.style.display = DisplayStyle.None;
            }
            
            


        }
    }
}

