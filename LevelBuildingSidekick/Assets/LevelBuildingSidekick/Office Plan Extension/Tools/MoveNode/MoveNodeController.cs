using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class MoveNodeController : ToolController
{
    Vector2 lastPos;
    Vector2 currentPos;

    private NodeController currentNode;

    public MoveNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new MoveNodeView(this);
        IsActive = false;
    }

    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;
        if(graph.SelectedNode == null)
        {
            return;
        }

        //if(!graph.SelectedNode.GetRect().Contains(Event.current.mousePosition))
        //{
        //    return;
        //}

        lastPos = currentPos;
        currentPos = Event.current.mousePosition;
        Vector2 delta = currentPos - lastPos;
        graph.SelectedNode.Translate(delta);
    }

    void SelectNode(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        //NodeController currentNode = graph.GetNodeAt(Event.current.mousePosition);
        currentNode = graph.TrySelectAt(CurrentPos) as NodeController;
        if (currentNode != null)
        {
            graph.SelectedNode = currentNode;
        }
    }

    public override void LoadData()
    {
    }

    public override void OnMouseDown(Vector2 position)
    {
        currentPos = position;
        SelectNode(Toolkit.Level);        
    }

    public override void OnMouseDrag(Vector2 position)
    {
        Action(Toolkit.Level);
    }

    public override void OnMouseUp(Vector2 position)
    {
        currentNode = null;
    }

    public override void Update()
    {

    }
}
