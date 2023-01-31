using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static void SetMargins(this VisualElement element, int value)
    {
        element.style.marginBottom = value;
        element.style.marginLeft = value;
        element.style.marginRight = value;
        element.style.marginTop = value;
    }

    public static void SetPaddings(this VisualElement element, int value)
    {
        element.style.paddingBottom = value;
        element.style.paddingLeft = value;
        element.style.paddingRight = value;
        element.style.paddingTop = value;
    }

    public static void SetBorder(this VisualElement element, Color color, int value)
    {
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
        element.style.borderRightColor = color;
        element.style.borderTopColor = color;

        element.style.borderBottomWidth = value;
        element.style.borderLeftWidth = value;
        element.style.borderRightWidth = value;
        element.style.borderBottomWidth = value;
    }
}
