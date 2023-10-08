using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EdgeQuestView : GraphElement
{
    private Vector2Int pos1, pos2;

    private LBSEdge data;

    public EdgeQuestView(LBSEdge data, QuestNodeView node1, QuestNodeView node2, int l, int stroke)
    {
        // Set Data
        this.data = data;

        // Set first node
        node1.OnMoving += (rect) => {
            this.SetPosition(new Rect(pos1, new Vector2(10, 10)));
            ActualizePositions(rect.center.ToInt(), pos2);
        };

        // Set second node
        node2.OnMoving += (rect) =>
        {
            ActualizePositions(pos1, rect.center.ToInt());
        };

        var sPos1 = new Vector2Int((int)node1.GetPosition().center.x, (int)node1.GetPosition().center.y);
        var sPos2 = new Vector2Int((int)node2.GetPosition().center.x, (int)node2.GetPosition().center.y);
        ActualizePositions(sPos1, sPos2);

        this.SetPosition(new Rect(pos1, new Vector2(10, 10)));
        this.generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        painter.DrawDottedLine(
            Vector2.zero,
            pos2 - pos1,
            Color.white
            );
    }

    private void ActualizePositions(Vector2Int pos1, Vector2Int pos2)
    {
        this.pos1 = pos1;
        this.pos2 = pos2;
        this.MarkDirtyRepaint();
    }
}
