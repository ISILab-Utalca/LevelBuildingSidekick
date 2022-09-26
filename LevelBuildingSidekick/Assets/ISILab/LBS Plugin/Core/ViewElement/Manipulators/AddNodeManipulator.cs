using LBS.Graph;
using LBS.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class AddNodeManipulator : MouseManipulator
    {
        private LBSGraphView view;

        public AddNodeManipulator(LBSGraphView view)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.view = view;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            var pos = e.localMousePosition;
            var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            var node = new RoomCharacteristicsData("Node: " + graph.NodeCount(), pos, 32);
            graph.AddNode(node);

            //view.ggw.RefreshView();

        }
    }
}