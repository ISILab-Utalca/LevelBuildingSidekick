using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace ISILab.Commons.VisualElements.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }

        public SplitView()
        {
            var content = this.Q<VisualElement>("unity-content-container");
            content.pickingMode = PickingMode.Ignore;

            var dragLineAnchor = this.Q<VisualElement>("unity-dragline-anchor");
            var dragLine = this.Q<VisualElement>("unity-dragline");
        }
    }
}
