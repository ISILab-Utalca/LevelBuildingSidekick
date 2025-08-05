using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomMinMaxSlider : MinMaxSlider
    {
        
        private IntegerField _minField;
        private IntegerField _maxField;

        private Vector2Int intValue = Vector2Int.zero;

        [UxmlAttribute]
        public Vector2Int IntValue
        {
            get => this.intValue;
            set
            {
                this.intValue.x = Math.Clamp(value.x, (int)lowLimit, (int)highLimit);
                this.intValue.y = Math.Clamp(value.y, (int)lowLimit, (int)highLimit);
                SetValueWithoutNotify((Vector2)intValue);
            } 
            
        }
        
        
        public LBSCustomMinMaxSlider() : base()
        {
            _minField = new IntegerField();
            _maxField = new IntegerField();
            
            _minField.value = (int)value.x;
            _maxField.value = (int)value.y;
            
            this.Add(_minField);
            this.Add(_maxField);
            
            _minField.style.minWidth = 32;
            _maxField.style.minWidth = 32;
            
            // Register When the Slider changes the value.
            RegisterCallback<ChangeEvent<Vector2>>(_evt =>
            {
                IntValue = new Vector2Int(Mathf.RoundToInt(_evt.newValue.x), Mathf.RoundToInt(_evt.newValue.y));
                _minField.SetValueWithoutNotify(Mathf.RoundToInt(
                    Mathf.Clamp(value.x, this.lowLimit, this.highLimit)));
                _maxField.SetValueWithoutNotify(Mathf.RoundToInt(
                    Mathf.Clamp(value.y, this.lowLimit, this.highLimit)));
                _evt.StopPropagation();
                
            });

            _minField.RegisterCallback<FocusOutEvent>(OnMinFieldUnFocused);


            void OnMinFieldUnFocused(FocusOutEvent _evt)
            {
                IntegerField target = _evt.target as IntegerField;
                if (target == null) return;
                
                if (target.value > this.intValue.y)
                {
                    Debug.LogWarning("Min Range Value is greater than Max Value");
                    _minField.value = intValue.y;
                    _evt.StopPropagation();
                    return;
                }

                if (target.value < lowLimit)
                {
                    this.IntValue = new Vector2Int((int)lowLimit, intValue.y);
                }
                else
                {
                    this.IntValue = new Vector2Int(target.value, intValue.y);
                }
                _evt.StopPropagation();
            }
            
            
            
            _maxField.RegisterCallback<ChangeEvent<int>>(_evt =>
            {
                
                if (_evt.newValue < this.value.x)
                {
                    Debug.LogWarning("Max Range Value is less than Min Value");
                    return;
                }

                if (_evt.newValue > highLimit)
                {
                    this.IntValue = new Vector2Int(intValue.x, (int)highLimit);
                }
                else
                {
                    IntValue = new Vector2Int(intValue.x, _evt.newValue);
                }
                _evt.StopPropagation();
            });
            
        }
        
    }
}
