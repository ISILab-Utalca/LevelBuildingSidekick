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

    public System.Action OnAIExecute;

    public AIAgentPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIAgentPanel"); // Editor
        visualTree.CloneTree(this);

        label = this.Q<Label>(name: "Name");
        details = this.Q<Button>(name: "Details");
        execute = this.Q<Button>(name: "Execute");
    }

    public AIAgentPanel(LBSAIAgent agent, System.Action OnAIExecute) : this()
    {
        this.agent = agent;
        this.OnAIExecute = OnAIExecute;

        label.text = agent.Name;
        details.clicked += () => Debug.LogWarning("Not Implemented");
        execute.clicked += () =>
        {
            agent.Execute();
            this.OnAIExecute?.Invoke();
        };

    }
}
