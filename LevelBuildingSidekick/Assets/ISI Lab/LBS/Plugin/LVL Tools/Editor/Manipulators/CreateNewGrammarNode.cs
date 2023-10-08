using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewGrammarNode : LBSManipulator // where T: LBSNode  // (!) CreateNewNode<T>
{

    private string prefix = "";
    public CreateNewGrammarNode(/*string prefix = "", string postfix = ""*/) : base()
    {
        this.prefix = "";
    }

    public override void Init(LBSLayer layer, object behaviour)
    {
        throw new NotImplementedException();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        //throw new NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        /*
        var pos = MainView.FixPos(e.localMousePosition);
        //var pos = e.localMousePosition;

        var name = "";
        var loop = true;
        var v = 0;
        do
        {
            name = prefix + actionToSet.ID + " (" + v + ")";

            loop = module.QuestNodes.Any(n => n.Node.ID.Equals(name));
            v++;
        } while (loop);

        var a = new QuestStep(actionToSet); //var n = Activator.CreateInstance<T>();
        var n = new LBSNode(name, pos);
        module.AddNode(n, a);
        */
    }
}
