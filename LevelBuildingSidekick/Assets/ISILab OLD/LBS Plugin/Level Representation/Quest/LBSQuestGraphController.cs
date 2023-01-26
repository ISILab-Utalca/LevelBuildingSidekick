using LBS.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSQuestGraphController : LBSGraphController
{
    public GrammarTree GrammarTree;

    List<QuestGraphNode> quests;

    public List<QuestGraphNode> openNodes; // change name to something better (!!!)

    QuestGraphNode currentQuest;
    public QuestGraphNode CurrentQuest
    {
        get
        {
            if(currentQuest == null)
            {
                if(quests == null)
                {
                    quests = new List<QuestGraphNode>();
                }
                if(quests.Count == 0)
                {
                    quests.Add(new QuestGraphNode("Quest1", Vector2.zero));
                }
                CurrentQuest = quests[0];
            }
            return currentQuest;
        }
        set
        {
            if(currentQuest != value)
            {
                currentQuest = value;
                openNodes.Clear();
                OpenNode(currentQuest);
            }
        }
    }

    public LBSQuestGraphController(LBSGraphView view, GraphicsModule data) : base(view, data)
    {
        GrammarTree = GrammarReader.ReadGrammar(Application.dataPath + "/ISI Lab/LBS/Examples/Grammar/Grammar.xml");
        //GrammarTree = GrammarReader.ReadGrammar(Application.dataPath + "/Grammar/Grammar.xml");
        openNodes = new List<QuestGraphNode>();
        OpenNode(CurrentQuest);
    }

    internal override LBSNodeDataOld NewNode(Vector2 position)
    {
        QuestGraphNode g = new QuestGraphNode("Undefined", position);
        return g;
    }

    public LBSNodeDataOld NewNode(Vector2 position, GrammarNode grammarElement)
    {
        QuestGraphNode g = new QuestGraphNode(grammarElement.ID, position);
        return g;
    }

    internal override void AddNode(LBSNodeDataOld node)
    {
        base.AddNode(node);
        openNodes[^1].Children.Add(node as QuestGraphNode);
    }

    internal GrammarNode GetGrammarElement(string grammarKey)
    {
        return GrammarTree.GetGrammarElement(grammarKey);
    }

    public void OpenNode(QuestGraphNode node)
    {
        if (openNodes.Count == 0)
        {
            openNodes.Add(node);
        }
        else if (openNodes[^1].Children.Contains(node))
        {
            CloseNode(openNodes[^1]);
            openNodes.Add(node);
        }

        UpdateData(node);
    }

    public void UpdateData(QuestGraphNode node)
    {
        var graph = data as GraphicsModule;

        graph.Clear();

        if (node.Children.Count != 0)
        {
            data.AddNode(node.Children[0]);

            for (int i = 1; i < node.Children.Count; i++)
            {
                graph.AddNode(node.Children[i]);
                graph.AddEdge(node.Children[i - 1], node.Children[i]);
            }
        }
    }

    public void CloseNode(QuestGraphNode node)
    {
        node.Children.Clear();

        var nodes = data.GetNodes();

        foreach (var n in nodes)
        {
            if(n is QuestGraphNode)
                node.Children.Add(n as QuestGraphNode);
        }
    }

    internal void CloseUntill(int index)
    {
        while(openNodes.Count > index + 1)
        {
            CloseNode(openNodes[^1]);
            openNodes.Remove(openNodes[^1]);
            UpdateData(openNodes[^1]);
        }

    }
}
