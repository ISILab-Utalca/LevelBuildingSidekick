using LBS.Graph;
using LBS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ExpandManipulator : MouseManipulator
{
    private LBSGraphController controller;
    private GenericLBSWindow window;
    public ExpandManipulator(GenericLBSWindow window, LBSGraphController controller)
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        this.controller = controller;
        this.window = window;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        var t = e.target as LBSGraphElement;
        //var t = (T)e.target; 
        if (t == null)
            return;

        // Node
        var node = e.target as LBSNodeView;
        if (node == null)
        {
            return;
        }

        var data = node.Data as QuestGraphNode;
        if (data == null)
        {
            return;
        }

        var questGraph = controller as LBSQuestGraphController;
        if (questGraph == null)
        {
            return;
        }

        var grammarElement = questGraph.GetGrammarElement(data.GrammarKey);
        if (grammarElement == null)
        {
            return;
        }

        var expansions = grammarElement.GetExpansionsText();


        var menu = DynamicContextMenu.Instance;

        var actions = new List<Tuple<string, Action>>();
        for (int i = 0; i < expansions.Count; i++)
        {
            int counter = i;
            var a = new Action(() =>
            {
                window.MainView.Remove(menu);
                data.Expand(counter, questGraph.GrammarTree);
            });

            actions.Add(new Tuple<string, Action>(expansions[i], a));
        }

        menu.Update(actions, node.GetPosition().position);

        window.MainView.Add(menu);
    }
}
