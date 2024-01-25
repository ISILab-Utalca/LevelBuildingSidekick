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

    //private VisualElement dragLineAnchor;
    //private VisualElement dragLine;

    public SplitView()
    {
        var content = this.Q<VisualElement>("unity-content-container");
        content.pickingMode = PickingMode.Ignore;

        var dragLineAnchor = this.Q<VisualElement>("unity-dragline-anchor");
        var dragLine = this.Q<VisualElement>("unity-dragline");
    }
}
