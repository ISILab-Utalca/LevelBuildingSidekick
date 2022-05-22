using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

public class GraphView : LevelRepresentationView
{
    NodeWindow nodeInspector;
    Vector2 scrollPos;
    public GraphView(Controller controller) : base(controller)
    {
    }

    public override void Draw()
    {
        //Debug.Log("Graph View");
        //scrollPos = EditorGUILayout.BeginScrollView(scrollPos,true,true);
        //GUILayout.BeginArea(new Rect(0, 0, 1000, 1000));
        var controller = Controller as GraphController;

        if(controller.SelectedNode != null)
        {
            if(nodeInspector == null)
            {
                nodeInspector = EditorWindow.GetWindow<NodeWindow>();
            }
            if(nodeInspector.Data == null || !nodeInspector.Data.Equals(controller.SelectedNode.Data))
            {
                nodeInspector.Data = controller.SelectedNode.Data as NodeData;
            }
        }


        foreach(NodeController n in controller.Nodes)
        {
            n.View.Display();
        }
        foreach(EdgeController e in controller.Edges)
        {
            e.View.Display();
        }
        /*
        for(int i = 0; i < 100; i++)
        {
            GUILayout.Label(i.ToString());
        }*/

        base.Draw();
        //GUILayout.EndArea();
        //EditorGUILayout.EndScrollView();
    }
}

