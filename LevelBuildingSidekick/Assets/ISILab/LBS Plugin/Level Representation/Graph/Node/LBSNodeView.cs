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
    public class LBSNodeView : LBSGraphElement
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

        public LBSNodeView(LBSNodeData node, LBSGraphView root) : base(root)
        {
            Data = node;

            SetPosition(new Rect(Data.Position, Vector2.one * 2 * Data.Radius));

            {
                Box b = new Box();
                b.style.maxHeight = b.style.maxWidth = 2 * Data.Radius;
                var l = new Label(node.Label);
                l.pickingMode = PickingMode.Ignore;
                l.focusable = true;
                b.pickingMode = PickingMode.Ignore;
                b.Add(l);
                Add(b);
            }

            capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
            this.style.minHeight = this.style.minWidth = this.style.maxHeight = this.style.maxWidth = 2 * Data.Radius;
            styleSheets.Add(styleSheet);

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
             
        }

        public new void RemoveFromHierarchy()
        {
            Debug.Log("RFH");
            //base.RemoveFromHierarchy();
        }

        public override void HandleEvent(EventBase evt)
        {
            //Debug.Log(evt.GetType());
        }

        public void Delete()
        {
            //rootView.refres
            this.OnDelete();
        }

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

        public override void OnDelete()
        {
            //throw new NotImplementedException();
        }
    }
}