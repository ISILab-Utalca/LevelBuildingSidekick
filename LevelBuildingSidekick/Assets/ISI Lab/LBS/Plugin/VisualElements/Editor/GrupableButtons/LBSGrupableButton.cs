using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public abstract class LBSGrupableButton : VisualElement, IGrupable
    {
        public Color color = new Color(72 / 255f, 72 / 255f, 72 / 255f);
        public Color selected = new Color(161 / 255f, 81 / 255f, 21 / 255f);

        // Event
        public abstract void AddGroupEvent(Action action);

        public LBSGrupableButton() { }

        #region EVENTS 
        // call when the button is selected
        private Action onFocusEvent;
        public event Action OnFocusEvent
        {
            add => onFocusEvent += value;
            remove => onFocusEvent -= value;
        }

        // call when the button is deselected
        private Action onBlurEvent;
        public event Action OnBlurEvent
        {
            add => onBlurEvent += value;
            remove => onBlurEvent -= value;
        }

        #endregion

        public virtual void OnBlur()
        {
            onBlurEvent?.Invoke();
        }

        public virtual void OnFocus()
        {
            onFocusEvent?.Invoke();
        }

        public void SetColorGroup(Color color, Color selected)
        {
            this.color = color;
            this.selected = selected;
        }

        public string GetLabel()
        {
            throw new NotImplementedException();
        }
    }
}