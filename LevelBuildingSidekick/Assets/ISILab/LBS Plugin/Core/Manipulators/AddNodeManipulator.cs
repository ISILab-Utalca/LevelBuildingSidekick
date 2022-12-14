using LBS.Graph;
using LBS.Schema;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class AddNodeManipulator : MouseManipulator
    {
        private LBSGraphRCController controller;
        private GenericLBSWindow window;

        public AddNodeManipulator(GenericLBSWindow window,LBSGraphRCController controller)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.controller = controller;
            this.window = window;
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

            var name = "";
            var loop = true;
            var v = 0;
            do
            {
                name = "Node: " + v;

                var nn = graph.GetNode(name);
                if (nn == null)
                    loop = false;
                v++;
            } while (loop);


            var node = new RoomCharacteristicsData(name, pos, 32);
            graph.AddNode(node);
            window.RefreshView();
        }
    }
}