using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleGrupableButton : LBSGrupableButton
{
    public new class UxmlFactory : UxmlFactory<SimpleGrupableButton, VisualElement.UxmlTraits> { }

    public Button button;

    public SimpleGrupableButton()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("SimpleGrupableButton");
        visualTree.CloneTree(this);

        button = this.Q<Button>();
    }

    public override void AddGroupEvent(Action action)
    {
        button.clicked += action;
    }

    public override void OnBlur()
    {
        button.style.backgroundColor = color;
        base.OnBlur();
    }

    public override void OnFocus()
    {
        button.style.backgroundColor = selected;
        base.OnFocus();
    }

    internal void SetName(string v)
    {
        button.text = v;
    }
}
