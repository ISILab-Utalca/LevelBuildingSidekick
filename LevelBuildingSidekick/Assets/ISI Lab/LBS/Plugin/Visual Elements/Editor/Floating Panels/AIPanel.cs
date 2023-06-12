using LBS.AI;
using LBS.Components;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    VisualElement container;

    public Action OnFinish;

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
            agent.OnTermination = OnFinish;
            agent.Init(ref layer);
            container.Add(GetAgentPanel(agent));
        }
    }

    private VisualElement GetAgentPanel(LBSAIAgent agent)
    {
        var candidates = Utility.Reflection.GetClassesWith<CustomVisualElementAttribute>();
        if (candidates.Count <= 0)
            return new Label("[ISI Lab] " + agent.GetType() + " does not have an associated VisualElement ");
        var ves = candidates.Where(t => t.Item2.Any(v => v.type == agent.GetType()));
        if(ves.Count() <= 0)
            return new Label("[ISI Lab] " + agent.GetType() + " does not have an associated VisualElement ");
        var ve = Activator.CreateInstance(ves.First().Item1, new object[] { agent });
        if (!(ve is VisualElement))
            return new Label("[ISI Lab] " + ve.GetType().GetType() + " is not a VisualElement ");
        return ve as VisualElement;

    }

}
