using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawSimpleGraph : Drawer
{
    public DrawSimpleGraph() : base() { }

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var graph = layer.GetModule<GraphModule<RoomNode>>();

        var nViews = new List<LBSNodeView<RoomNode>>();
        foreach (var node in graph.GetNodes())
        {
            var element = new LBSNodeView<RoomNode>(node, node.Position, new Vector2(80, 80));
            element.label.text = node.ID;
            nViews.Add(element);
            view.AddElement(element);
        }

        foreach (var edge in graph.GetEdges())
        {
            var n1 = nViews.Find(v => v.Data.Equals(edge.FirstNode));
            var n2 = nViews.Find(v => v.Data.Equals(edge.SecondNode));
            var element = new LBSEdgeView<LBSEdge, RoomNode>(edge, n1, n2, 10, 3);
            view.AddElement(element);
        }
    }
}
