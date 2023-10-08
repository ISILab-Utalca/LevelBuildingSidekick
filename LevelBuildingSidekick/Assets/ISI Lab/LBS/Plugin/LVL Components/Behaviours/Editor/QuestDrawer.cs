using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


[Drawer(typeof(QuestBehaviour))]
public class QuestDrawer : Drawer
{
    private Vector2 size = new Vector2(150, 100);

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var quest = target as QuestBehaviour;

        if (quest == null)
        {
            return;
        }

        foreach (var t in quest.GetNodes())
        {
            var v = new QuestNodeView(t);
            var p = new Vector2(t.Node.Position.x, t.Node.Position.y);

            var pos = new Vector2(
                p.x - (size.x / 2f),
                (p.y - (size.y / 2f)));

            v.SetPosition(new Rect(pos, size));

            view.AddElement(v);
        }

        foreach (var t in quest.GetEdges())
        {
            // implemetar (!!!)
        }
    }
}
