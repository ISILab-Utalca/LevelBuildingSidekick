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
        public new class UxmlFactory : UxmlFactory<LBSNodeView, VisualElement.UxmlTraits> { }

        private static VisualTreeAsset visualTree;
        private static StyleSheet styleSheet;

        public LBSNodeData Data;

        public delegate void NodeEvent(LBSNodeData data);
        public NodeEvent OnStartDragEdge;
        public NodeEvent OnEndDragEdge;
        public Action OnMoving;

        private float cellSize;

        public LBSNodeView(): base() 
        {
        }

        public LBSNodeView(LBSNodeData node, LBSGraphView root, float cellSize = 1) : base(root)
        {
            Data = node;
            this.cellSize = cellSize;
            Vector2 size = new Vector2(Data.Width, Data.Height) * cellSize;
            SetPosition(new Rect(Data.Position, size));


            this.style.minHeight = this.style.maxHeight = size.y;
            this.style.minWidth = this.style.maxWidth = size.x;

            var label = this.Q<Label>(name: "Label");
            label.text = Data.Label;

            capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
             
        }

        public override void LoadVisual()
        {
            if (!styleSheet)
            {
                styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
            }
            styleSheets.Add(styleSheet);

            if (!visualTree)
            {
                visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("NodeUxml");
            }
            visualTree.CloneTree(this);
        }

        public void Delete()
        {
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