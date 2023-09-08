using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Drawer(typeof(HillClimbingAssistant))]
public class HillClimbingDrawer : Drawer
{
    private readonly Vector2 nodeSize = new Vector2(100, 100);

    private Dictionary<Zone, LBSNodeView> viewRefs = new();

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // clear preview view refrences
        viewRefs.Clear();

        // Set target Assitant
        var assistant = target as HillClimbingAssistant;

        // Get zones
        var zones = assistant.ZonesWhitTiles;
        List<LBSNodeView> nViews = new();
        foreach (var zone in zones)
        {
            // Create node view
            var nView = new LBSNodeView();

            var bound = assistant.GetTiles(zone).GetBounds();

            // Set view values
            nView.SetPosition(new Rect(bound.center * nodeSize - (nodeSize / 2f), nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            nViews.Add(nView);

            // Add to reference dictionary
            viewRefs.Add(zone, nView);
        }

        // Get edges
        var edges = assistant.GetEdges();
        List<LBSEdgeView> eViews = new();
        foreach (var edge in edges)
        {
            // Get view nodes
            var n1 = viewRefs[edge.First];
            var n2 = viewRefs[edge.Second];

            // Create EdgeView
            var eView = new LBSEdgeView(edge, n1, n2, 4, 4);

            // Add element to the canvas
            view.AddElement(eView);
        }

        eViews.ForEach(e => view.AddElement(e));
        nViews.ForEach(n => view.AddElement(n));
    }
}