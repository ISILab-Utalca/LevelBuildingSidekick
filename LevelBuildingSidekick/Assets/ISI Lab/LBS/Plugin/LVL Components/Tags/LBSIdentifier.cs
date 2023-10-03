using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewID", menuName = "ISILab/LBS/Identifier")]
[System.Serializable]
public class LBSIdentifier : ScriptableObject
{
    #region FIELDS
    [SerializeField]
    protected string label;
    [SerializeField]
    protected Texture2D icon;
    [SerializeField]
    protected Color color;
    #endregion

    #region PROPERTIES
    public string Label
    {
        get => label;
        set
        {
            if (label == value)
                return;

            this.label = value;
            OnChangeText?.Invoke(this);
        }
    }

    public Texture2D Icon
    {
        get => icon;
        set
        {
            if (icon == value)
                return;

            icon = value;
            OnChangeIcon?.Invoke(this);
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            if (color == value)
                return;

            color = value;
            OnChangeColor?.Invoke(this);
        }
    }
    #endregion

    #region EVENTS
    public delegate void TagEvent(LBSIdentifier tag);
    public TagEvent OnChangeText;
    public TagEvent OnChangeColor;
    public TagEvent OnChangeIcon;
    #endregion

    #region METHODS
    public void Init(string text,Color color,Texture2D icon)
    {
        this.label = text;
        this.color = color;
        this.icon = icon;
    }
    #endregion
}
