using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;
using Utility;
using LBS.Graph;
using LBS.Schema;
using System.Reflection.Emit;
using Label = UnityEngine.UIElements.Label;

namespace LBS.Graph
{
    public class LBSNodeView : GraphElement
    {
        #region InspectorDrawer
        private class NodeScriptable : GenericScriptable<LBSNodeData> { };
        [CustomEditor(typeof(GenericScriptable<LBSNodeData>))]
        [CanEditMultipleObjects]
        private class NodeScriptableEditor : GenericScriptableEditor { };
        #endregion

        public LBSNodeData Data;

        public delegate void NodeEvent(LBSNodeData data);
        public NodeEvent OnStartDragEdge;
        public NodeEvent OnEndDragEdge;
        public Action OnMoving;

        public LBSNodeView(LBSNodeData node)
        {
            Data = node;

            SetPosition(new Rect(Data.Position, Vector2.one * 2 * Data.Radius));

            Box b = new Box();
            b.style.minHeight = b.style.minWidth = b.style.maxHeight = b.style.maxWidth = 2 * Data.Radius;
            b.Add(new Label(node.Label));
            
            Add(b);

            VisualElement main = this;

            capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
            styleSheets.Add(styleSheet);

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);

        }

        /*
        public override void HandleEvent(EventBase evt)
        {
            Debug.Log(evt.GetType());
        }*/

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                OnStartDragEdge?.Invoke(Data);
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button == 1)
            {
                OnEndDragEdge?.Invoke(Data);
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();

            var il = Reflection.MakeGenericScriptable(Data);
            Selection.SetActiveObjectWithContext(il, il);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = new Vector2Int((int)newPos.x, (int)newPos.y);
            OnMoving?.Invoke();
        }
    }
}