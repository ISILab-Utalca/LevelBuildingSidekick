using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGrammarGraph : Drawer
{
    public DrawGrammarGraph() : base() { }

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var graph = layer.GetModule<LBSGraph>();

        var nViews = new List<LBSNodeView_Old<LBSNode>>();
        foreach (var node in graph.GetNodes())
        {
            var size = new Vector2(300, 100);
            var element = new LBSNodeView_Old<LBSNode>(node, node.Position - (size / 2f), size);

            element.SetText(node.ID);
            //element.SetColor(node.Room.Color);

            nViews.Add(element);
            view.AddElement(element);
        }

        foreach (var edge in graph.GetEdges())
        {
            var n1 = nViews.Find(v => v.Data.Equals(edge.FirstNode));
            var n2 = nViews.Find(v => v.Data.Equals(edge.SecondNode));
            var element = new LBSEdgeView<LBSEdge, LBSNode>(edge, n1, n2, 10, 3);
            view.AddElement(element);
            element.SendToBack();
        }
    }


    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        throw new System.NotImplementedException();
    }
}
