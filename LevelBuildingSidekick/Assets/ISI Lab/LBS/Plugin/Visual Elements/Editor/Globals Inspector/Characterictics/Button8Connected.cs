using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Button8Connected : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<Button8Connected, VisualElement.UxmlTraits> { }
    #endregion

    private Color baseColor = new Color(88 / 255f, 88 / 255f, 88 / 255f, 1f);
    private Color selectedColor = new Color(163 / 255f, 160 / 255f, 251 / 255f, 1f);

    private Button upperLeft;
    private Button upper;
    private Button upperRight;

    private Button left;
    private VisualElement center;
    private Button right;

    private Button bottomLeft;
    private Button bottom;
    private Button bottomRight;

    private Button cxn1;
    private Button cxn4;
    private Button cxn8;

    private VisualElement connection1;
    private VisualElement connection4;
    private VisualElement connection8;

    public Button8Connected()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Button8Connected");
        visualTree.CloneTree(this);

        upperLeft = this.Q<Button>("UpperLeft");
        upper = this.Q<Button>("Upper");
        upperRight = this.Q<Button>("UpperRight");

        left = this.Q<Button>("Left");
        center = this.Q<Button>("Center");
        right = this.Q<Button>("Right");

        bottomLeft = this.Q<Button>("BottomLeft");
        bottom = this.Q<Button>("Bottom");
        bottomRight = this.Q<Button>("BottomRight");

        connection1 = this.Q<VisualElement>("Connection1");
        connection4 = this.Q<VisualElement>("Connection4");
        connection8 = this.Q<VisualElement>("Connection8");

    }

    public void SetWidth(int value = 20)
    {
        upper.style.height = value;
        bottom.style.height = value;
        left.style.width = value;
        right.style.width = value;
    }

    public void SetMargins(int value = 2)
    {
        upperLeft.SetMargins(value);
        upper.SetMargins(value);
        upperRight.SetMargins(value);

        left.SetMargins(value);
        center.SetMargins(value);
        right.SetMargins(value);

        bottomLeft.SetMargins(value);
        bottom.SetMargins(value);
        bottomRight.SetMargins(value);
    }

    public void SetBorderRadius(int value = 3)
    {
        upperLeft.style.borderTopLeftRadius = value;
        upperRight.style.borderTopRightRadius = value;
        bottomLeft.style.borderBottomLeftRadius = value;
        bottomRight.style.borderBottomRightRadius = value;
    }
}
