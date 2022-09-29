using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{

    public class DeleteManipulator : MouseManipulator
    {
        private LBSGraphRCController controller;

        public DeleteManipulator(LBSGraphRCController controller)
        {
            activators.Add(new ManipulatorActivationFilter {button = MouseButton.LeftMouse});
            this.controller = controller;
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
            var t = e.target as LBSGraphElement;
            if (t == null)
                return;

            // Node
            var node = e.target as LBSNodeView; 
            if (node != null)
            {
                controller.RemoveNode(node.Data);
                //var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
                //graph.RemoveNode(node.Data.Label);
                //t.Clear();
                return;
            }

            // Edge
            var edge = e.target as LBSEdgeView;
            if(edge != null)
            {
                controller.RemoveEdge(edge.Data);
                //var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
                //graph.RemoveEdge(edge.Data);
                //t.Clear();
                return;
            }


        }
    }
}