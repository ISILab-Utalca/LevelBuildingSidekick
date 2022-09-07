using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSGraphView : GraphView
{
    public new void DeleteElements(IEnumerable<GraphElement> elements)
    {
        var elms = elements.Where(e => e is LBSGraphElement).Select(e=> e as LBSGraphElement).ToList();
        elms.ForEach(e => e.OnDelete());
        base.DeleteElements(elements);
    }
}

public abstract class LBSGraphElement : GraphElement
{
    public abstract void OnDelete();
}