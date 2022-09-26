using LBS;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSGraphView : GraphView
{
    public List<IRepController> controllersRefs;

    public void ClearView()
    {
        this.graphElements.ForEach(e => this.RemoveElement(e));
    }

    public GenericGraphWindow ggw; // cancercito (!!!)

    public void Refresh()
    {
        this.ClearView();
        Debug.Log("Refresh AAA");
        /*
        controllers.Clear();

        OnLoadControllers();
        InitContextualMenu();
        InitPanel();
        Populate();

        currentController = controllers[0];
        mainView.OnClearSelection = () =>
        {
            // puede que esto generar clases que ya existe y se dupliquen revisar si puede llegar a ser un problema (?)
            var il = Reflection.MakeGenericScriptable(currentController.GetData());
            Selection.SetActiveObjectWithContext(il, il);
        };
        */
    }

    public new void DeleteElements(IEnumerable<GraphElement> elements)
    {
        var elms = elements.Where(e => e is LBSGraphElement).Select(e=> e as LBSGraphElement).ToList();
        elms.ForEach(e => e.OnDelete());
        base.DeleteElements(elements);
    }
}

public abstract class LBSGraphElement : GraphElement
{
    public LBSGraphView rootView;

    public LBSGraphElement(LBSGraphView rootView)
    {
        this.rootView = rootView;
    }

    public abstract void OnDelete();
}