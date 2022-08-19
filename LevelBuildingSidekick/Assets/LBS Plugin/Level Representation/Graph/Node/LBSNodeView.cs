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

namespace LevelBuildingSidekick.Graph
{
    public class LBSNodeView : GraphElement
    {
        public Texture2D circle;

        public LBSNodeData Data;

        public delegate void NodeEvent(LBSNodeData data);
        public NodeEvent OnStartDragEdge;
        public NodeEvent OnEndDragEdge;

        public LBSNodeView(LBSNodeData node)
        {
            Data = node;

            SetPosition(new Rect(Data.Position, Vector2.one * 2 * Data.Radius));

            Box b = new Box();
            b.style.minHeight = b.style.minWidth = b.style.maxHeight = b.style.maxWidth = 2 * Data.Radius;
            b.Add(new Label(node.label));
            
            Add(b);

            VisualElement main = this;

            capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
            styleSheets.Add(styleSheet);

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Debug.Log("A");
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            Debug.Log("B");
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            //Debug.Log("type: "+ evt.GetType());
            if (evt is MouseDownEvent)
            {
                Debug.Log("A");
                var e = (MouseDownEvent)evt;
                if(e.button == 1)
                {
                    Debug.Log("B");
                    OnStartDragEdge?.Invoke(Data);
                }
            }
            else if(evt is MouseUpEvent)
            {
                Debug.Log("C");
                var e = (MouseUpEvent)evt;
                if (e.button == 1)
                {
                    Debug.Log("D");
                    OnEndDragEdge?.Invoke(Data);

                }
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Data.UnitySelect();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.Position = new Vector2Int((int)newPos.x, (int)newPos.y);
        }
    }
}