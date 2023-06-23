using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LineView : GraphElement
{

    public new class UxmlFactory : UxmlFactory<LineView, UxmlTraits> { }


    public LineView()
    {

    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        // Init
        var paint2D = mgc.painter2D;

    }

}