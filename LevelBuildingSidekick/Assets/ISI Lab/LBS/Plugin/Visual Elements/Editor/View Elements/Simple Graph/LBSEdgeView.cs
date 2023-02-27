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
    private LBSNodeView<U> node1, node2;

    private T data;

    public T Data => data;

    public LBSEdgeView(T data, LBSNodeView<U> node1, LBSNodeView<U> node2, int l, int stroke)
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
        var view = this.GetFirstAncestorOfType<MainView>();

        var fPos1 = Vector2.zero;
        var fPos2 = pos2 - pos1;
        //var fPos2 = view.FixPos(pos2) - view.FixPos(pos1);
        painter.DrawLine(fPos1, fPos2, Color.white, Color.white, true, 1);

        var angle = Vector2.SignedAngle(Vector2.right,(fPos2 - fPos1).normalized); // (?) puede que no funcione
        var middle = fPos2.Divided(2f);
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
    public static Vector2Int Multiply(this Vector2Int vec, float value)
    {
        vec.x = (int)(vec.x * value);
        vec.y = (int)(vec.y * value);
        return vec;
    }

    public static Vector2Int Divided(this Vector2Int vec, float value)
    {
        vec.x = (int)(vec.x / value);
        vec.y = (int)(vec.y / value);
        return vec;
    }

    public static Vector2 Divided(this Vector2 vec, float value)
    {
        vec.x = (int)(vec.x / value);
        vec.y = (int)(vec.y / value);
        return vec;
    }
}