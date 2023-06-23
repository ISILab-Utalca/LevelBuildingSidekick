using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }

    private int minWidth = 45;

    private VisualElement dragLineAnchor;
    private VisualElement dragLine;

    public SplitView()
    {
        var content = this.Q<VisualElement>("unity-content-container");
        content.pickingMode = PickingMode.Ignore;

        dragLineAnchor = this.Q<VisualElement>("unity-dragline-anchor");
        dragLine = this.Q<VisualElement>("unity-dragline");


        //this.RegisterCallback<AttachToPanelEvent>(OnAttach); // descomentar cuando se fixee el OnChageSize (!)
    }

    public void OnAttach(AttachToPanelEvent evt)
    {
        if(this.childCount >= 2)
        {
            this[1].RegisterCallback<GeometryChangedEvent>(OnChageSize);
        }
    }

    public void OnChageSize(GeometryChangedEvent evt) // el cuerpo de esto esta mal implementado, no hace que el drag line se quede en borde minimo y titila (!)
    {
        if(this[1].resolvedStyle.width <= minWidth)
        {
            this[1].style.width = minWidth;
            
            var dragLinePos = minWidth + dragLineAnchor.worldBound.xMin;
            dragLine.style.left = dragLinePos;
            dragLineAnchor.style.left = dragLinePos;
        }
    }
}
