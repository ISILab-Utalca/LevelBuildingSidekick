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
}

public interface IRepController
{
    public void PopulateView(MainView view);
    public void SetContextualMenu(MainView view);
    public void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe);
    public object GetData();
}
