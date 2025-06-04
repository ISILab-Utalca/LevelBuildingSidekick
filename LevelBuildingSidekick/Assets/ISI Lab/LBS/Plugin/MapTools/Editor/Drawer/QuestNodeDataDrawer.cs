using System.Linq;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
        // Circle drawn to display an element on the graph
        private const float CircleSize = 100f;
        // Multiplied by the CircleSize (1 == filled circle)
        private const float borderThickness = 0.025f;

        public QuestNodeBehaviourDrawer() : base() { }
        /// <summary>
        /// Draws the information that corresponds to the quest node behavior selected node.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="teselationSize"></param>
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not QuestNodeBehaviour behaviour) return;
            
            var layer = behaviour.OwnerLayer;
            if (layer == null) return;
            
            var qn = behaviour.SelectedQuestNode;
            var nd = qn?.NodeData;
            if (nd == null) return;
            
            //Debug.Log("\n --node: " + nd.Owner.ID + "--");
            /*
             * TODO: Replace this within the switch and pass the visualElement corresponding
             * to the type in the switch. Perhaps use the attribute created for actions}
             * but apply on visual Elements.
             */
            // view.AddElement(behaviour.OwnerLayer, behaviour, type);
            var position = layer.FixedToPosition(nd.position, true);
                  
            var circle = new CircleElement(position, CircleSize, nd);
            view.AddElement(behaviour.OwnerLayer, behaviour, circle);
            
        }
        

        public class CircleElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;

            public CircleElement(Vector2 position, float diameter, BaseQuestNodeData data)
            {
                _data = data;

                // Clone the UXML template
                // Find root: var root 
                style.width = diameter;
                style.height = diameter;
                style.left = position.x;
                style.top = position.y;

                var radius = diameter / 2;
                
                style.borderBottomLeftRadius = radius;
                style.borderBottomRightRadius = radius;
                style.borderTopLeftRadius = radius;
                style.borderTopRightRadius = radius;


                var thickness = diameter * borderThickness;
                style.borderBottomWidth = thickness;
                style.borderTopWidth = thickness;
                style.borderLeftWidth = thickness;
                style.borderRightWidth = thickness;
                
                
                // Add the UXML root to this VisualElement
                // Add(root);
                
                // Should set color per switch data class
                Color color;
                
                // Get references to sub-elements
               // VisualElement centerElement = null; // root.Q<VisualElement>("CenterElement");
               // centerElement.style.backgroundColor = Color.white;
                
                color = new Color(0.93f, 0.81f, 0.42f, 1f);
                Color backgroundColor = color;
                backgroundColor.a = 0.33f;
                
                style.backgroundColor = backgroundColor;
                style.borderBottomColor = color;
                style.borderTopColor = color;
                style.borderRightColor = color;
                style.borderLeftColor = color;
                
                RegisterCallback<MouseMoveEvent>(OnMouseMove);
                RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                // left button pressed
                if (e.pressedButtons == 0 || e.button != 0) return;
                if (!MainView.Instance.HasManipulator<Select>()) return;

                var gridPos = LBSMainWindow._gridPosition;

                var grabPosition = GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
                grabPosition *= MainView.Instance.viewport.transform.scale;
                Rect newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
                SetPosition(newPos);
                _data!.position = gridPos;
                
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                var qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(_data.Owner.Graph.OwnerLayer); 
                qnb.DataChanged(_data.Owner);
            }
        }
    }
}