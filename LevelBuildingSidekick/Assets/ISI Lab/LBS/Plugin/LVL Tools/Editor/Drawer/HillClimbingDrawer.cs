using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UIElements;

[Drawer(typeof(HillClimbingAssistant))]
public class HillClimbingDrawer : Drawer
{
    private readonly Vector2 nodeSize = new Vector2(100, 100);

    private Dictionary<Zone, LBSNodeView> viewRefs = new();

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // clear preview view refrences
        viewRefs.Clear();

        // Set target Assitant
        var assistant = target as HillClimbingAssistant;

        // Get modules
        var consts = assistant.ConstrainsZonesMod.Constraints;
        var zones = assistant.ZonesWhitTiles;

        List<LBSNodeView> nViews = new();
        List<DottedAreaFeedback> cViews = new();
        foreach (var zone in zones)
        {
            // Create node view
            var nView = new LBSNodeView();

            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();

            // Set view values
            nView.SetPosition(new Rect(bound.center * nodeSize - (nodeSize / 2f), nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            // add node view to list
            nViews.Add(nView);

            /*
            if (consts.Count > 0) // parche
            {
                var pair = consts.First(p => p.zone == zone);
                cViews.AddRange(CreateFeedBackAreas(pair, bound.center, teselationSize));
            }
            */

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

        // print everything
        eViews.ForEach(e => view.AddElement(e));
        cViews.ForEach(c => view.AddElement(c));
        nViews.ForEach(n => view.AddElement(n));
    }

    private List<DottedAreaFeedback> CreateFeedBackAreas(ConstraintPair pair,Vector2 center, Vector2 size)
    {
        List<DottedAreaFeedback> cViews = new();

        var constr = pair.Constraint;

        var maxV1 = new Vector2(-constr.maxHeight / 2f, -constr.maxWidth / 2f);
        var maxV2 = new Vector2(constr.maxHeight / 2f, constr.maxWidth / 2f);
        var c1 = new DottedAreaFeedback();
        c1.ActualizePositions(((maxV1 + center) * (size * 100)).ToInt(),(( maxV2 + center) * (size * 100)).ToInt());
        c1.SetColor(Color.red);

        var minV1 = new Vector2(-constr.minHeight / 2f, -constr.minWidth / 2f);
        var minV2 = new Vector2(constr.minHeight / 2f, constr.minWidth / 2f);
        var c2 = new DottedAreaFeedback();
        c2.ActualizePositions(minV1.ToInt(),minV2.ToInt());

        // add constraint to list
        cViews.Add(c1);
        cViews.Add(c2);

        return cViews;
    }
}