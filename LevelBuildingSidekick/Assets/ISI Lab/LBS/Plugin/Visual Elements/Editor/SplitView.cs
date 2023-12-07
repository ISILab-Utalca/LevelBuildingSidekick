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
    }

    public void OnAttach(AttachToPanelEvent evt)
    {

    }

}
