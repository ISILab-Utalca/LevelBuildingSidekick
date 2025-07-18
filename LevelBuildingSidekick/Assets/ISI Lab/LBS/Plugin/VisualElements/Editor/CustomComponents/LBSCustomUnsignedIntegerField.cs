using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomUnsignedIntegerField: UnsignedIntegerField
    {
        
        
        private VectorImage typeIcon;
        private VectorImage addIcon;
        private VectorImage minusIcon;

        private Button addButton;
        private Button minusButton;
        private VisualElement iconVisualElement;

        [UxmlAttribute]
        public VectorImage TypeIcon
        {
            get => typeIcon;
            set => typeIcon = value;
        }
        
        
        
        public LBSCustomUnsignedIntegerField() : base()
        {
            addButton = new Button() { text = "+" };
            minusButton = new Button() { text = "-" };
            iconVisualElement = new VisualElement();
            
            minusButton.AddToClassList("minusButton");
            this.Add(minusButton);
            
            addButton.AddToClassList("addButton");
            this.Add(addButton);
            
            addButton.RegisterCallback<ClickEvent>((evt) =>
            {
                value += 1;
            });
            
            minusButton.RegisterCallback<ClickEvent>((evt) =>
            {
                value -= 1;
                value = Math.Clamp(value, 0, int.MaxValue);
            });
            
        }
    }
}


