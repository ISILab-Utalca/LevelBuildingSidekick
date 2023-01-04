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

        /// <summary>
        /// Default constructor of LBSNodeView.
        /// </summary>
        public LBSNodeView(): base() 
        {
        }

        /// <summary>
        /// Constructor for the LBSNodeView class, which creates a visual
        /// representation of a node data and adds it to the given root graph view. [duda]
        /// </summary>
        /// <param name="node"> Node data to be represented by the node view. </param>
        /// <param name="root"> Root graph view to which the node view will be added. </param>
        /// <param name="cellSize"> Size of the cell in which the node is represented, in pixels. </param>
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

        /// <summary>
        /// Loads the visual tree and styles for this node view.
        /// </summary>
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

        /// <summary>
        /// Delete the graph element.
        /// </summary>
        public void Delete()
        {
            this.OnDelete();
        }

        /// <summary>
        /// Handles a mouse down event. If the right mouse button is pressed, 
        /// invokes the OnStartDragEdge event.
        /// </summary>
        /// <param name="evt"> MouseDownEvent to handle.</param>
        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                OnStartDragEdge?.Invoke(Data);
            }
        }

        /// <summary>
        /// Handles a mouse up event. If the right mouse button is released, 
        /// invokes the OnEndDragEdge event.
        /// </summary>
        /// <param name="evt"> MouseUpEvent to handle. </param>
        private void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button == 1)
            {
                OnEndDragEdge?.Invoke(Data);
            }
        }

        /// <summary>
        /// Sets the data element of this graph as the active object in the
        /// selection.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();

            var il = Reflection.MakeGenericScriptable(Data);
            Selection.SetActiveObjectWithContext(il, il);
        }

        /// <summary>
        /// Set a new position by parameter.
        /// </summary>
        /// <param name="newPos"> New position given.</param>
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