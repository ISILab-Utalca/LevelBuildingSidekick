using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewID", menuName = "ISILab/Identifier")]
[System.Serializable]
public class LBSIdentifier : ScriptableObject
{
    [SerializeField]
    protected string label;
    [SerializeField]
    protected Texture2D icon;
    [SerializeField]
    protected Color color;

    public string Label
    {
        get => label;
        set => this.label = value;
    }

    public Texture2D Icon
    {
        get => icon;
        set => icon = value;
    }

    public Color Color
    {
        get => color;
        set => color = value;
    }

    public void Init(string text,Color color,Texture2D icon)
    {
        this.label = text;
        this.color = color;
        this.icon = icon;
    }
}
