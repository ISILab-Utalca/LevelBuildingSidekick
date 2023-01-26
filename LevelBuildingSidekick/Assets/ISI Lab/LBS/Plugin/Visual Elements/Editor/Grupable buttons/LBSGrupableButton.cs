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
    public Action OnFocusEvent; // se llama cuando se selecciona el boton
    public Action OnBlurEvent; // se llama cuando se dejo de seleccionar el boton

    public abstract void AddGroupEvent(Action action);

    public LBSGrupableButton() { }

    public virtual void OnBlur()
    {
        OnBlurEvent?.Invoke();
    }

    public virtual void OnFocus()
    {
        OnFocusEvent?.Invoke();
    }

}
