using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ActionButton, VisualElement.UxmlTraits> { }

    public Button ActionBtn;
    public Button AddBtn;

    public GrammarElement grammarElement;

    public ActionButton()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ActionButtonUXML");
        visualTree.CloneTree(this);

        ActionBtn = this.Q<Button>(name: "ActionBtn");
        AddBtn = this.Q<Button>(name: "AddBtn");
    }

    public ActionButton(string label, GrammarElement grammarElement)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ActionButtonUXML");
        visualTree.CloneTree(this);

        ActionBtn = this.Q<Button>(name: "ActionBtn");
        AddBtn = this.Q<Button>(name: "AddBtn");

        this.grammarElement = grammarElement;
        ActionBtn.text = label;
    }
}
