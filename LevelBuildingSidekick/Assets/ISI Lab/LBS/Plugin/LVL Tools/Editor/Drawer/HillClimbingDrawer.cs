using LBS.Components;
using LBS.Settings;
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

            // Set position
            var pos = new Vector2(
                bound.center.x * nodeSize.x -(nodeSize.x /2f), 
                -(bound.center.y * nodeSize.y - (nodeSize.y / 2f)));

            // Set view values
            nView.SetPosition(new Rect(pos, nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            // add node view to list
            nViews.Add(nView);

            // Add to reference dictionary
            viewRefs.Add(zone, nView);

            // Get pair info
            foreach (var pair in consts)
            {
                if(pair.Zone == zone)
                {
                    // Create feedback view
                    cViews.AddRange(CreateFeedBackAreas(viewRefs[zone], pair, teselationSize));
                    break;
                }
            }
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

        if(assistant.visibleConstraints)
            cViews.ForEach(c => view.AddElement(c));

        nViews.ForEach(n => view.AddElement(n));
    }

    public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="center_old"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private List<DottedAreaFeedback> CreateFeedBackAreas(LBSNodeView nodeView, ConstraintPair pair, Vector2 size)
    {
        var settings = LBSSettings.Instance;
        var tileSize = settings.general.TileSize;

        List<DottedAreaFeedback> cViews = new();

        var constr = pair.Constraint;

        // Get points from first dotted area
        var maxV1 = new Vector2(-constr.maxWidth / 2f, -constr.maxHeight / 2f);
        var maxV2 = new Vector2(constr.maxWidth / 2f, constr.maxHeight / 2f);

        // Create first dotted area
        var c1 = new DottedAreaFeedback();

        // Get center position
        var center = nodeView.GetPosition().center;

        // Set values to first doted area
        c1.SetPosition(new Rect(center, new Vector2(10, 10)));
        c1.ActualizePositions((maxV1 * size * tileSize).ToInt(),(maxV2 * size * tileSize).ToInt());
        c1.SetColor(Color.red);

        // Get points from second dotted area
        var minV1 = new Vector2(-constr.minWidth / 2f, -constr.minHeight / 2f);
        var minV2 = new Vector2(constr.minWidth / 2f, constr.minHeight / 2f);

        // Create second dotted area
        var c2 = new DottedAreaFeedback();

        // Set value to second dotted area
        c2.SetPosition(new Rect(center, new Vector2(10, 10)));
        c2.ActualizePositions((minV1 * size * tileSize).ToInt(), (minV2 * size * tileSize).ToInt());
        c2.SetColor(Color.blue);

        // add constraint to list
        cViews.Add(c1);
        cViews.Add(c2);

        nodeView.OnMoving += (rect) =>
        {

        };

        return cViews;
    }
}