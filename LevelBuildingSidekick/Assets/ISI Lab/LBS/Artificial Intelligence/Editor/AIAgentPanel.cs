using LBS.AI;
using LBS.Assisstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class AIAgentPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<AIAgentPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region VIEW FIELDS
    private Label label;
    private Button details;
    private Button execute;
    #endregion

    #region FIELDS
    private LBSAssistant agent;
    #endregion

    #region CONSTRUCTORS
    public AIAgentPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIAgentPanel"); // Editor
        visualTree.CloneTree(this);

        label = this.Q<Label>(name: "Name");
        details = this.Q<Button>(name: "Details");
        execute = this.Q<Button>(name: "Execute");
    }
    #endregion
}
