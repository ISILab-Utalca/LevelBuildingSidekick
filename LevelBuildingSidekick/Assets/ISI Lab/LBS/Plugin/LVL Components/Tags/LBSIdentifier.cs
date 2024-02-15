using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewID", menuName = "ISILab/LBS/Identifier")]
[System.Serializable]
public class LBSIdentifier : ScriptableObject
{
    #region FIELDS
    [ReadOnly]
    public string label;
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
    public void Init(string text, Color color, Texture2D icon)
    {
        this.label = text;
        this.color = color;
        this.icon = icon;
    }

    private void OnValidate()
    {
        label = this.name;
    }
    #endregion
}

public class ReadOnlyAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}