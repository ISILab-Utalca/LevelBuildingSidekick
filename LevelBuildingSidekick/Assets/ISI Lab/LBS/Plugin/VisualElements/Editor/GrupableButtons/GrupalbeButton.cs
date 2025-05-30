using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class GrupalbeButton : Button, IGrupable
    {
       // public new class UxmlFactory : UxmlFactory<GrupalbeButton, UxmlTraits> { }

        public string label;

        private Color color = new Color(1, 0, 0);
        private Color selected = new Color(0, 0, 1);

        private Action OnFocusEvent;
        private Action OnBlurEvent;

        event Action IGrupable.OnFocusEvent
        {
            add => OnFocusEvent += value;
            remove => OnFocusEvent -= value;
        }

        event Action IGrupable.OnBlurEvent
        {
            add => OnBlurEvent += value;
            remove => OnBlurEvent -= value;
        }

        public GrupalbeButton() { }

        public GrupalbeButton(string text)
        {
            this.text = label = text;
        }

        public void AddGroupEvent(Action action)
        {
            clicked += action;
        }

        public void OnBlur()
        {
            style.backgroundColor = color;
            OnBlurEvent?.Invoke();
        }

        public void OnFocus()
        {
            style.backgroundColor = selected;
            OnFocusEvent?.Invoke();
        }

        public void SetColorGroup(Color color, Color selected)
        {
            this.color = color;
            this.selected = selected;
        }

        public string GetLabel()
        {
            return label;
        }
    }
}