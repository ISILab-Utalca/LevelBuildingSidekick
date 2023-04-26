using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateNewGrammarNode : ManipulateGrammarGraph // where T: LBSNode  // (!) CreateNewNode<T>
{

    private string prefix = "";
    public CreateNewGrammarNode(/*string prefix = "", string postfix = ""*/) : base()
    {
        this.prefix = "";
    }

    protected override void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();
        var pos = mainView.FixPos(e.localMousePosition);
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
        this.OnManipulationEnd?.Invoke();
    }

    protected override void OnMouseMove(MouseMoveEvent e)
    {
        //throw new NotImplementedException();
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        //throw new NotImplementedException();
    }
}
