using System;
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
        
        private uint maxValue = 100;
        private uint minValue = 0;

        private Button addButton;
        private Button minusButton;
        private VisualElement iconVisualElement;

        [UxmlAttribute]
        public VectorImage TypeIcon
        {
            get => typeIcon;
            set => typeIcon = value;
        }

        [UxmlAttribute]
        public uint MaxValue
        {
            get => maxValue; 
            set => maxValue = value;
        }
        
        [UxmlAttribute]
        public uint MinValue { 
            get => minValue;
            set => minValue = value; 
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
                value = Math.Clamp(value + 1, minValue, maxValue);
                
            });
            
            minusButton.RegisterCallback<ClickEvent>((evt) =>
            {
                value = Math.Clamp(value - 1, minValue, maxValue);
                
            });
            
        }
    }
}


