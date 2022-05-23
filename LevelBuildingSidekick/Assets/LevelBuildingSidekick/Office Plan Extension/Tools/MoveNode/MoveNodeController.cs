using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNodeController : ToolController
{
    Vector2 lastPos;
    Vector2 currentPos;
    public MoveNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new MoveNodeView(this);
        IsActive = false;    
        OnButtonClick.AddListener(() => { IsActive = !IsActive; }); //Debug.Log(IsActive); });
    }

    public override void Action(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;
        if(graph.SelectedNode == null)
        {
            return;
        }
        if(!graph.SelectedNode.GetRect().Contains(Event.current.mousePosition))
        {
            //IsActive = false;
            return;
        }
        lastPos = currentPos;
        currentPos = Event.current.mousePosition;
        Vector2 delta = currentPos - lastPos;
        graph.SelectedNode.Translate(delta);
    }

    void SelectNode(LevelRepresentationController level)
    {
        GraphController graph = level as GraphController;

        NodeController n = graph.GetNodeAt(Event.current.mousePosition);
        if (n != null)
        {
            graph.SelectedNode = n;
        }
    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
        if (IsActive)
        {
            Event e = Event.current;
            if (e.button == 0 && e.type.Equals(EventType.MouseDown))
            {
                currentPos = e.mousePosition;
                SelectNode(Toolkit.Level);
                //Debug.Log("Down");
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseDrag))
            {
                Action(Toolkit.Level);
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseUp))
            {
                //IsActive = false;
            }
        }
    }
}
