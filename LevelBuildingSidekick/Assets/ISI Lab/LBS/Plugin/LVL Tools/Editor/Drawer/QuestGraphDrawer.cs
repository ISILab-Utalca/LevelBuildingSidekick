using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Drawer(typeof(GrammarAssistant))]
public class QuestGraphDrawer : Drawer
{
    public QuestGraphDrawer() : base() { }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var assistant = target as GrammarAssistant;

        var quest = assistant.Quest;

        var nodeViews = new Dictionary<QuestNode, QuestNodeView>();


        foreach(var node in quest.QuestNodes)
        {
            if(node.ID == "Start Node")
            {
                var v = new StartQNode();
                v.SetPosition(new Rect(node.Position, LBSSettings.Instance.general.TileSize));
                nodeViews.Add(node, v);
                continue;
            }

            var nodeView = new QuestNodeView(node);

            var size = assistant.Owner.TileSize * LBSSettings.Instance.general.TileSize;

            nodeView.SetPosition(new Rect(node.Position, size));

            nodeViews.Add(node, nodeView);
        }

        foreach (var edge in quest.QuestEdges)
        {
            var n1 = nodeViews[edge.First];
            var n2 = nodeViews[edge.Second];

            var edgeView = new LBSQuestEdgeView(edge, n1, n2, 4, 4);

            view.AddElement(edgeView);
        }

        foreach(var node in nodeViews.Values)
        {
            view.AddElement(node);
        }
    }

    public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
    {
        throw new System.NotImplementedException();
    }
}
