using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSGrupableButton : VisualElement, IGrupable
{
    public Color color = new Color(72 / 255f, 72 / 255f, 72 / 255f);
    public Color selected = new Color(161 / 255f, 81 / 255f, 21 / 255f);

    // Event
    public abstract void AddGroupEvent(Action action);

    public LBSGrupableButton() { }

    #region EVENTS 

    private Action onFocusEvent; // se llama cuando se selecciona el boton
    public event Action OnFocusEvent
    {
        add => onFocusEvent += value;
        remove => onFocusEvent -= value;
    }

    private Action onBlurEvent; // se llama cuando se dejo de seleccionar el boton
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
