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

public class QuestWindow : GenericLBSWindow, INameable
{
    public VisualElement actionsContent;
    GrammarTree grammarTree;

    //[MenuItem("ISILab/LBS plugin/Quest window", priority = 1)]
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


        grammarTree = GrammarReader.ReadGrammar(Application.dataPath + "/Grammar/FirstGrammar.xml"); //Use actual route (!!!)

        foreach(var p in grammarTree.Productions)
        {
            AddAction("Rule: " + p.Key, p.Value);
        }
        foreach(var t in grammarTree.Terminals)
        {
            AddAction("Terminal: " + t.Key, t.Value);
        }
    }

    public void StartGrammar()
    {

    }

    public void AddAction(string label, GrammarNode grammarElement)
    {
        var act = new ActionButton(label, grammarElement);
        act.ActionBtn.clicked += () =>
        {
            var graph = this.GetController<LBSQuestGraphController>();
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
    }

    public override void OnInitPanel()
    {

    }
}
