using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
using UnityEngine;


[Drawer(typeof(QuestBehaviour))]
public class QuestDrawer : Drawer
{
    private Vector2 size = new Vector2(150, 100);

    private Dictionary<LBSNode, QuestNodeView> viewRefs = new();

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // clear preview view refrences
        viewRefs.Clear();

        var quest = target as QuestBehaviour;

        if (quest == null)
        {
            return;
        }

        List<QuestNodeView> nViews = new();
        foreach (var t in quest.GetNodes())
        {
            var v = new QuestNodeView(t);
            var p = new Vector2(t.Node.Position.x, t.Node.Position.y);

            var pos = new Vector2(
                p.x - (size.x / 2f),
                (p.y - (size.y / 2f)));

            v.SetPosition(new Rect(pos, size));

            nViews.Add(v);

            // Add to reference dictionary
            viewRefs.Add(t.Node, v);
        }

        List<EdgeQuestView> eViews = new();
        foreach (var e in quest.GetEdges())
        {
            var n1 = viewRefs[e.First];
            var n2 = viewRefs[e.Second];

            var edge = new EdgeQuestView(e, n1, n2, 4, 4);

            eViews.Add(edge);
        }

        eViews.ForEach(e => view.AddElement(e));

        nViews.ForEach(n => view.AddElement(n));
    }
}
