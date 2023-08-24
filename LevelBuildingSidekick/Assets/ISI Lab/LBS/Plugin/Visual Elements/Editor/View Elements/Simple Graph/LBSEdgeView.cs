using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSEdgeView<T,U> : GraphElement where T: LBSEdge where U :LBSNode
{
    private Vector2Int pos1, pos2;
    private LBSNodeView_Old<U> node1, node2; // sobra (?)

    private T data;

    public T Data => data;

    public LBSEdgeView(T data, LBSNodeView_Old<U> node1, LBSNodeView_Old<U> node2, int l, int stroke)
    {
        this.data = data;
        this.node1 = node1;
        node1.OnMoving += (pos) => {
            this.SetPosition(new Rect(pos1, new Vector2(10, 10)));
            ActualizePositions(pos, pos2); 
        };
        this.node2 = node2;
        node2.OnMoving += (pos) => { ActualizePositions(pos1, pos); };
        
        var sPos1 = new Vector2Int((int)node1.GetPosition().center.x, (int)node1.GetPosition().center.y);
        var sPos2 = new Vector2Int((int)node2.GetPosition().center.x, (int)node2.GetPosition().center.y);
        ActualizePositions(sPos1, sPos2);

        this.SetPosition(new Rect(pos1, new Vector2(10,10)));
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
