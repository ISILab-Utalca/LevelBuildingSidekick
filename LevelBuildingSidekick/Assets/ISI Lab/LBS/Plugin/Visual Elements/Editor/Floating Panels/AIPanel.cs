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

        //var assist = layer.Assitant;
        var assistants = layer.Assitants;
        for (int i = 0; i < assistants.Count; i++)
        {
            var ass = assistants[i];
            ass.OnTermination = OnAIExecute;
            ass.OnTermination = OnEndExecute;
            ass.Init(ref layer);
            container.Add(new AIAgentPanel(ref ass));

        }
    }

}
