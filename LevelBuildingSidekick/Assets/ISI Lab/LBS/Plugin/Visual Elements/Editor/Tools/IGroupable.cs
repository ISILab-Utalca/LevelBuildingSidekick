using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IGrupable
{
    public void SetActive(bool value);
    public void SetEvent(Action action);
}

public class GrupableButton : VisualElement, IGrupable
{
    public void SetEvent(Action action)
    {
        throw new NotImplementedException();
    }

    public void SetActive(bool value)
    {
        throw new NotImplementedException();
    }
}