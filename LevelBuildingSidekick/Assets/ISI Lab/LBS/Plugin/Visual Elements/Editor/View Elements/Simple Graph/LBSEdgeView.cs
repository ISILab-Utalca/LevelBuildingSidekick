using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSEdgeView<T,U> : GraphElement where T: LBSEdge where U :LBSNode
{
    private Vector2Int pos1, pos2;
    private LBSNodeView<U> node1, node2;

    private T data;

    public T Data => data;

    public LBSEdgeView(T data, LBSNodeView<U> node1, LBSNodeView<U> node2)
    {
        this.data = data;
        this.node1 = node1;
        node1.OnMoving += (pos) => { ActualizePositions(pos, pos2); };
        this.node2 = node2;
        node2.OnMoving += (pos) => { ActualizePositions(pos1, pos); };
        
        var sPos1 = new Vector2Int((int)node1.GetPosition().x, (int)node1.GetPosition().y);
        var sPos2 = new Vector2Int((int)node2.GetPosition().x, (int)node2.GetPosition().y);
        ActualizePositions(sPos1, sPos2);
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        painter.DrawLine(pos1, pos2, Color.white, Color.white, true, 1);
        var angle = Vector2.SignedAngle(Vector2.zero,(pos2 - pos1)); // (?) puede que no funcione
        var middle = (pos1 + pos2).Divided(2f);
        painter.DrawPolygons(middle, 3, Color.white, Color.white, 0, angle, 10);
    }

    private void ActualizePositions(Vector2Int pos1, Vector2Int pos2)
    {
        this.pos1 = pos1;
        this.pos2 = pos2;
        this.MarkDirtyRepaint();
    }
}

public static class extVec
{
    public static Vector2Int Divided(this Vector2Int vec, float value)
    {
        vec.x = (int)(vec.x / value);
        vec.y = (int)(vec.y / value);
        return vec;
    }
}