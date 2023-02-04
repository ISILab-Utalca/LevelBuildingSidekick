using LBS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class AIAgentPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIAgentPanel, VisualElement.UxmlTraits> { }

    Label label;
    Button details;
    Button execute;

    LBSAIAgent agent;

    public AIAgentPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIAgentPanel"); // Editor
        visualTree.CloneTree(this);

        label = this.Q<Label>(name: "Name");
        details = this.Q<Button>(name: "Details");
        execute = this.Q<Button>(name: "Execute");
    }

    public AIAgentPanel(ref LBSAIAgent agent) : this()
    {
        this.agent = agent;

        label.text = agent.Name;
        details.clicked += () => Debug.LogWarning("Not Implemented");
        execute.clicked += () =>
        {
            this.agent.Execute();
        };

    }
}
