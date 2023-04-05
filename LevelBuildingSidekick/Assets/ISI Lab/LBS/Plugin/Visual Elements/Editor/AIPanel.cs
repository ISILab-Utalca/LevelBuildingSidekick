using LBS.Components;
using System;
using UnityEngine.UIElements;

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    VisualElement container;

    public Action OnAIExecute;
    public Action OnEndExecute;

    public AIPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);

        container = this.Q<VisualElement>(name: "Container");
    }

    public void Init(ref LBSLayer layer)
    {
        container.Clear();

        var assist = layer.Assitant;

        for(int i = 0; i < assist.AgentsCount; i++)
        {
            var agent = assist.GetAgent(i);
            agent.OnTermination = OnAIExecute;
            agent.OnTermination = OnEndExecute;
            agent.Init(ref layer);
            container.Add(new AIAgentPanel(ref agent));

        }
    }

}
