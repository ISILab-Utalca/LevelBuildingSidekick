using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DoorView, VisualElement.UxmlTraits> { }

    public VisualElement Left;
    public VisualElement Right;
    public VisualElement Top;
    public VisualElement Botton;

    public DoorView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Door");
        visualTree.CloneTree(this);

        Left = this.Q<VisualElement>(name: "Left");
        Right = this.Q<VisualElement>(name: "Right");
        Top = this.Q<VisualElement>(name: "Top");
        Botton = this.Q<VisualElement>(name: "Botton");

    }
    /// <summary>
    /// Terminar Nicolas
    /// </summary>
    /// <param name="one"></param>
    public DoorView(object one)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Door");
        visualTree.CloneTree(this);

        Left = this.Q<VisualElement>(name: "Left");
        Right = this.Q<VisualElement>(name: "Right");
        Top = this.Q<VisualElement>(name: "Top");
        Botton = this.Q<VisualElement>(name: "Botton");
    }


}
