using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewID", menuName = "ISILab/Identifier")]
[System.Serializable]
public class LBSIdentifier : ScriptableObject
{
    // Start is called before the first frame update
    [SerializeField]
    protected string label;
    public string Label
    {
        get => label;
        set => this.label = value;
    }

    [SerializeField]
    protected Texture2D icon;
    public Texture2D Icon
    {
        get => icon;
        set => icon = value;
    }

    [SerializeField]
    protected Color color;
    public Color Color
    {
        get => color;
        set => color = value;
    }
}
