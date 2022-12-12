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

    /// <summary>
    /// Removes all visual elements contained in this view.
    /// </summary>
    public void ClearView()
    {
        this.graphElements.ForEach(e => this.RemoveElement(e));
    }

    /// <summary>
    /// Removes the visual elements delivered by parameters
    /// that this view contains.
    /// </summary>
    /// <param name="elements"></param>
    public new void DeleteElements(IEnumerable<GraphElement> elements)
    {
        var elms = elements.Where(e => e is LBSGraphElement).Select(e=> e as LBSGraphElement).ToList();
        elms.ForEach(e => e.OnDelete());
        base.DeleteElements(elements);
    }

    public T GetController<T>() where T : IRepController
    {
        foreach(var controller in controllersRefs)
        {
            if(controller is T)
            {
                return (T)controller;
            }
        }
        return default;
    }
}
