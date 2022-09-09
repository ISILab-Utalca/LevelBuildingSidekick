using LBS.ElementView;
using LevelBuildingSidekick;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSRepController<T> : IRepController where T : LBSRepesentationData // representation controler // : Controller
{
    protected List<GraphElement> elements = new List<GraphElement>();

    protected GraphView view;
    protected T data;

    protected LBSRepController(GraphView view,T data)
    { 
        this.view = view;
        this.data = data;
    }

    public void Init(T data, LBSGraphView view)
    {
        this.data = data;
        this.view = view;
    }

    public void SetContextualMenu(MainView view)
    {
        view.OnBuild += (cmpe) => OnContextualBuid(view,cmpe);
    }

    /// <summary>
    /// Builds the context menu of the window based on the actions you append 
    /// to the contextualMenuPopulateEvent.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="cmpe"></param>
    public abstract void OnContextualBuid( MainView view, ContextualMenuPopulateEvent cmpe);

    /// <summary>
    /// Populates view delivered by parameters with the data information.
    /// </summary>
    /// <param name="view"></param>
    public abstract void PopulateView(MainView view);

    public object GetData()
    {
        return data;
    }

    /// <summary>
    /// Marka los objetos para que no sean interactuables, (los hace trasparentes ademas). el nombre esta raro (!)
    /// </summary>
    /// <param name="v"></param>
    public void ShowView(bool v)
    {
        elements.ForEach(e =>  e.style.opacity = v ? new StyleFloat(0.1f): new StyleFloat(1f));
        // desactiva la interacion con los objetos correspondientes a este controlador
    }
}

public interface IRepController
{
    public void PopulateView(MainView view);
    public void SetContextualMenu(MainView view);
    public void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe);
    public object GetData();
    public void ShowView(bool v);
}
