using Commons.Optimization.Evaluator;
using LBS.ElementView;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LBS.Windows;
using LBS;
using LBS.Graph;
using LBS.VisualElements;

public class QuestWindow : GenericLBSWindow, INameable
{
    public VisualElement actionsContent;
    public VisualElement openNodes;

    [MenuItem("ISILab/LBS plugin/Quest window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<QuestWindow>();
        window.titleContent = new GUIContent(window.GetName());
    }

    public override void CreateGUI()
    {
        OnLoadControllers();
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestWindowUXML");
        visualTree.CloneTree(root);

        actionsContent = root.Q<VisualElement>(name: "Content");
        mainView = root.Q<MainView>(name: "QuestGraph");
        openNodes = root.Q<VisualElement>(name: "OpenNodes");

        var grammarTree = GetController<LBSQuestGraphController>().GrammarTree;
        foreach (var p in grammarTree.Actions)
        {
            AddAction("Rule: " + p.Key, p.Value);
        }
        foreach (var t in grammarTree.Terminals)
        {
            AddAction("Terminal: " + t.Key, t.Value);
        }
    }

    public void StartGrammar()
    {

    }

    public void AddAction(string label, GrammarNode grammarElement)
    {
        var graph = this.GetController<LBSQuestGraphController>();
        var act = new ActionButton(label, grammarElement);
        act.ActionBtn.clicked += () =>
        {
            var node = graph.NewNode(Vector2.zero, act.grammarElement);
            graph.AddNode(node);
            graph.AddNodeView(node);
        };
        actionsContent.Add(act);
        
    }

    public string GetName()
    {
        return "Quest window";
    }

    public override void OnLoadControllers()
    {
        var data = LBSController.CurrentLevel.data;
        var quests = data.GetRepresentation<LBSGraphData>("QuestGraph");
        var graph = new LBSQuestGraphController(mainView, quests);
        controllers.Add(graph);
        CurrentController = graph;
        RefreshOpenNodes();
    }

    public override void OnInitPanel()
    {

    }

    public void RefreshOpenNodes()
    {
        openNodes.Clear();

        var controller = GetController<LBSQuestGraphController>();
        if (controller == null)
            return;
        var nodes = controller.openNodes;


        for (int i = 0; i < nodes.Count; i++)
        {
            var button = new Button() { text = ">>" + nodes[i].GrammarKey };
            int index = i;
            button.clicked += () =>
            {
                controller.CloseUntill(index);
                RefreshOpenNodes();
                OnFocus();
            };
            openNodes.Add(button);
        }


    }


    public void UpdateActions()
    {
    }
}
