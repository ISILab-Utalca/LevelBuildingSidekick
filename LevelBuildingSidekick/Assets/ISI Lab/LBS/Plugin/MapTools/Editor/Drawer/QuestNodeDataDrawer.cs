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
        private const float BaseSize = 100f;
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
                  
            var circle = new TriggerElement(position, BaseSize*nd.size, nd);
            view.AddElement(behaviour.OwnerLayer, behaviour, circle);
            
        }
        

        public class TriggerElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;

            public TriggerElement(Vector2 position, float length, BaseQuestNodeData data)
            {
                _data = data;

                // Properly position the element using SetPosition to avoid layout offset
                SetPosition(new Rect(position.x, position.y, length, length));

                // Calculate radius and border thickness
                float radius = length / 2;
                float thickness = length * borderThickness;
                
                //style.borderBottomLeftRadius = radius;
                //style.borderBottomRightRadius = radius;
                //style.borderTopLeftRadius = radius;
                //style.borderTopRightRadius = radius;

                // Set border thickness
                style.borderBottomWidth = thickness;
                style.borderTopWidth = thickness;
                style.borderLeftWidth = thickness;
                style.borderRightWidth = thickness;

                // Color configuration
                Color color = new Color(0.93f, 0.81f, 0.42f, 1f);
                Color backgroundColor = color;
                backgroundColor.a = 0.33f;

                style.backgroundColor = backgroundColor;
                style.borderBottomColor = color;
                style.borderTopColor = color;
                style.borderRightColor = color;
                style.borderLeftColor = color;

                // Optional: if using a child VisualElement for background visuals
                // var visual = new VisualElement();
                // visual.style.flexGrow = 1;
                // visual.style.backgroundColor = backgroundColor;
                // Add(visual);

                RegisterCallback<MouseMoveEvent>(OnMouseMove);
                RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                // Only move when left mouse button is pressed
                if (e.pressedButtons == 0 || e.button != 0) return;
                if (!MainView.Instance.HasManipulator<Select>()) return;

                var currentRect = GetPosition();
                var delta = e.mouseDelta / MainView.Instance.viewTransform.scale;
                var newPos = new Rect(currentRect.x + delta.x, currentRect.y + delta.y, currentRect.width, currentRect.height);
                SetPosition(newPos);

                // Update node data position (convert to grid if needed)
                var gridPos = LBSMainWindow._gridPosition;
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