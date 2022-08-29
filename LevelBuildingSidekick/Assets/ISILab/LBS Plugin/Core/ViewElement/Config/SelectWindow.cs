using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectWindow : Button
{
    public new class UxmlFactory : UxmlFactory<ConfLoad, VisualElement.UxmlTraits> { }

    public Label windowName;
    public Toggle check;
    private Color color1 = new Color(0.2352f, 0.2352f, 0.2352f);
    private Color color2 = new Color(0.2274f, 0.4745f, 0.7333f);

    public SelectWindow()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("SelectWinUXML");
        visualTree.CloneTree(this);

        windowName = this.Q<Label>(name: "NameWindow");
        check = this.Q<Toggle>(name: "Tcheck");

        windowName.text = "Defalult";
    }
        public SelectWindow(string winName)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("SelectWinUXML");
        visualTree.CloneTree(this);

        windowName = this.Q<Label>(name: "NameWindow");

        windowName.text = winName;
    }

    public void Select(bool value)
    {
        this.style.backgroundColor = value ? color1 : color2;
        check.value = value;
    }
}
