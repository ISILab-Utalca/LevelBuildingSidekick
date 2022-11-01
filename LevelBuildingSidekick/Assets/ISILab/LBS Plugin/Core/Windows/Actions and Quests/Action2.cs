using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Action2 : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Action2, VisualElement.UxmlTraits> { }

    public Label nameAction;
   
    public Action2()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Action_window");
        visualTree.CloneTree(this);

        nameAction = this.Q<Label>(name: "Action"); 
    }

    public Action2(string labelText)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Action_window");
        visualTree.CloneTree(this);

        nameAction = this.Q<Label>(name: "Action");
        
        nameAction.text = labelText;
        
    }
}
