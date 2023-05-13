using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;

public class TagView : VisualElement
{
    #region FIELDS
    private LBSIdentifier target;

    private TextField textfield;
    private ColorField colorfield;
    #endregion

    #region CONSTRUCTORS
    public TagView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagView"); // Editor
        visualTree.CloneTree(this);

        // TextField
        textfield = this.Q<TextField>();
        textfield.RegisterCallback<ChangeEvent<string>>(e => TextChange(e.newValue));

        // ColorField
        colorfield = this.Q<ColorField>("ColorField");
        colorfield?.RegisterCallback<ChangeEvent<Color>>(e => ColorChange(e.newValue));
    }
    #endregion

    #region METHODS
    public void SetInfo(LBSIdentifier target)
    {
        this.target = target;

        textfield.value = target.Label;
        textfield.RegisterCallback<BlurEvent>(e => {
            Debug.Log(textfield.value);
            target.Label = textfield.value;
            EditorUtility.SetDirty(target);
        });

        colorfield.value = target.Color;
        colorfield.RegisterCallback<BlurEvent>(e => {
            target.Color = colorfield.value;
            EditorUtility.SetDirty(target);
        });

        target.OnChangeColor += (tag) => colorfield.value = tag.Color;
        target.OnChangeText += (tag) => { textfield.value = tag.Label;  };
        target.OnChangeIcon += (tag) => Debug.Log(tag.Icon);
    }
    #endregion

    #region VIEW METHODS
    public void TextChange(string value)
    {
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), target.name);

        target.Label = value;
        target.name = value;
        EditorUtility.SetDirty(target); // (!) ojo
    }

    public void ColorChange(Color color)
    {
        target.Color = color;
        AssetDatabase.SaveAssets(); // (!) ojo
    }
    #endregion
}
