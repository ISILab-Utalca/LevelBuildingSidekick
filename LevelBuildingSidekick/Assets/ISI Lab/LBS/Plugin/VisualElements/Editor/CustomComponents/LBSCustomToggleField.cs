using System.Linq;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.CustomComponents
{
    [UxmlElement]
    public partial class LBSCustomToggleField: BaseField<bool>
    {
        public static readonly string LBSClassName = "lbs-field";
        public static readonly string LBSToggleClass = "lbs-slide-toggle";
        public static readonly string inputUssClass = "lbs-slide-toggle__input";
        public static readonly string inputKnobUssClass = "lbs-slide-toggle__input-knob";
        public static readonly string inputPressedUssClass = "lbs-slide-toggle__input-checked";

        private VisualElement m_Input;
        private VisualElement m_Knob;
        
        private Color toggleColor = new Color(194/255f, 246/255f, 113/255f );
        
        #region Properties

        public Color ToggleColor 
        {
            set
            {
                toggleColor = value;
            }
            get
            {
                return toggleColor;
            }
        }
        
        #endregion
        
        
        public LBSCustomToggleField(): this("CustomToggleField"){}
        
        
        public LBSCustomToggleField(string _label) : base(_label, null)
        {
            RemoveFromClassList(ussClassName);
            AddToClassList(LBSToggleClass);
            AddToClassList(LBSClassName);
            
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClass);
            
            m_Knob = new();
            m_Knob.AddToClassList(inputKnobUssClass);
            m_Input.Add(m_Knob);
            
            
            RegisterCallback<ClickEvent>(_evt => OnClick(_evt));
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));
        }
        static void OnClick(ClickEvent _evt)
        {
            LBSCustomToggleField toggle = _evt.currentTarget as LBSCustomToggleField;
            if (toggle != null) toggle.ToggleValue();
            _evt.StopPropagation();
        }

        static void OnSubmit(NavigationSubmitEvent _evt)
        {
            LBSCustomToggleField toggle = _evt.currentTarget as LBSCustomToggleField;
            if (toggle != null) toggle.ToggleValue();
            _evt.StopPropagation();
        }

        static void OnKeydownEvent(KeyDownEvent _evt)
        {
            LBSCustomToggleField toggle = _evt.currentTarget as LBSCustomToggleField;
            if (toggle?.panel?.contextType == ContextType.Editor) return;

            if (_evt.keyCode == KeyCode.Return || _evt.keyCode == KeyCode.KeypadEnter || _evt.keyCode == KeyCode.Space)
            {
                toggle?.ToggleValue();
                _evt.StopPropagation();
            }
            
            _evt.StopPropagation();  
        }

        private void ToggleValue()
        {
            value = !value;
        }

        public override void SetValueWithoutNotify(bool _value)
        {
            base.SetValueWithoutNotify(_value);
            m_Input.EnableInClassList(inputPressedUssClass, _value);
        }
    }
}
