using Commons.Optimization.Evaluator;
using LBS.ElementView;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class QuestWindow : EditorWindow, INameable
{
    public VisualElement actionsContent;
    GrammarTree grammarTree;
    public MainView questView;
    LBSQuestGraphController graphController;

    //[MenuItem("ISILab/LBS plugin/Quest window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<QuestWindow>();
        window.titleContent = new GUIContent(window.GetName());
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestWindowUXML");
        visualTree.CloneTree(root);

        actionsContent = root.Q<VisualElement>(name: "Content");
        questView = root.Q<MainView>(name: "QuestGraph");
        graphController = questView.GetController<LBSQuestGraphController>();


        grammarTree = GrammarReader.ReadGrammar(Application.dataPath + "/Grammar/FirstGrammar.xml"); //Use actual route (!!!)

        foreach(var p in grammarTree.Productions)
        {
            AddAction("Rule: " + p.Key);
        }
        foreach(var t in grammarTree.Terminals)
        {
            AddAction("Terminal: " + t.Key);
        }
    }

    public void StartGrammar()
    {

    }

    public void AddAction(string action)
    {
        var act = new ActionButton(action);
        act.ActionBtn.clicked += () =>
        {
            questView.Add(new Node());
        };
        actionsContent.Add(act);
        
    }

    public string GetName()
    {
        return "Quest window";
    }
}
