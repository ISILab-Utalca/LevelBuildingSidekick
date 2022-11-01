using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionBtn : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ActionBtn, VisualElement.UxmlTraits> { }

    public Label nameAction;
   
    public ActionBtn()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ActionBtnUXML");
        visualTree.CloneTree(this);

        nameAction = this.Q<Label>(name: "Action"); 
    }

    public ActionBtn(string labelText)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ActionBtnUXML");
        visualTree.CloneTree(this);

        nameAction = this.Q<Label>(name: "Action");
        
        nameAction.text = labelText;
        
    }
}
