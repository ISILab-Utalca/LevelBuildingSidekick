using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
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
            
            Debug.Log("\n --node: " + nd.Owner.ID + "--");

            switch (nd)
            {
                case QuestNodeDataGoto dataGoto:
                {
                    var position = layer.FixedToPosition(dataGoto.position, true);
                    //var position = view.FixPos(location.position);
                    Color tagsColor = new Color(0.93f, 0.81f, 0.42f, 0.65f);
                    var square = new SquareElement(position, new Vector2(50, 50),
                        tagsColor,dataGoto);
                    view.AddElement(behaviour.OwnerLayer, behaviour, square);
                    Debug.Log(dataGoto.position);
                    break;
                }

                case QuestNodeDataKill kill:
                {
                    Debug.Log(kill.bundleGuid + " -> " + kill.Num);
                    break;
                }

                case QuestNodeDataSteal steal:
                {

                    Debug.Log(steal.position);
                    Debug.Log(steal.bundleGuid + " -> " + steal.bundleGuid);
                    break;
                }
            }
                
            
        }
        

        private class SquareElement : GraphElement
        {
            private readonly BaseQuestNodeData _data;
            public SquareElement(Vector2 position, Vector2 size, Color color, BaseQuestNodeData data)
            {
                _data = data;

                // Set the size and position
                style.width = size.x;
                style.height = size.y;
                style.left = position.x;
                style.top = position.y;

                // Set a background color
                style.backgroundColor = color;
                
                RegisterCallback<MouseLeaveEvent>(OnMouseUp);
                RegisterCallback<MouseMoveEvent>(OnMouseMove);
            }
            
            private void OnMouseUp(MouseLeaveEvent e)
            {
                Debug.LogError("UUUUUP");
                // Get the fixed position using your logic
                var moveClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();
                Vector2Int newPosition = _data.Owner.Graph.OwnerLayer.ToFixedPosition(moveClickPosition);
                newPosition.y = -newPosition.y;
                
                // Print the current position
                Debug.Log($"Dragging to: {newPosition}");
                
                // Update the data's position
                switch (_data)
                {
                    case QuestNodeDataGoto dataGoto: 
                        dataGoto.position = newPosition;
                        Debug.Log($"Updated QuestNodeDataGoto position to: {newPosition}");
                        break;

                    case QuestNodeDataSteal steal:
                        steal.position = newPosition;
                        Debug.Log($"Updated QuestNodeDataSteal position to: {newPosition}");
                        break;
                }

            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                // left button pressed
                if (e.pressedButtons != 1) return;
                if (!MainView.Instance.HasManipulator<Select>() ) return;
                
                var grabPosition =  GetPosition().position + e.mouseDelta / MainView.Instance.viewTransform.scale;
                grabPosition *= MainView.Instance.viewport.transform.scale;
                Rect newPos = new Rect(grabPosition.x, grabPosition.y, resolvedStyle.width, resolvedStyle.height);
                SetPosition(newPos);
                
              
            }
        }
    }
}