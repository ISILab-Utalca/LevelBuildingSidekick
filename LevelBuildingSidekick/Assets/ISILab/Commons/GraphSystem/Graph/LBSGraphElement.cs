using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class LBSGraphElement : GraphElement
{

    private LBSGraphView rootView;

    public LBSGraphView Root => rootView;

    /// <summary>
    /// Default constructor for the LBSGraphElement class.
    /// </summary>
    public LBSGraphElement()
    {
        LoadVisual();
    }

    /// <summary>
    /// Constructor for the LBSGraphElement class, which takes a 
    /// root view as a parameter.
    /// </summary>
    /// <param name="rootView"> Root view to which the element will be 
    /// added. </param>
    public LBSGraphElement(LBSGraphView rootView)
    {
        this.rootView = rootView;
        LoadVisual();
    }

    /// <summary>
    /// Loads the visual representation of the LBSGraphElement. Is called when 
    /// the element is created, and create and setting up the visual elements
    /// of the graph view.
    /// </summary>
    public abstract void LoadVisual();

    /// <summary>
    /// Delete the graph element.
    /// </summary>
    public abstract void OnDelete();
}
