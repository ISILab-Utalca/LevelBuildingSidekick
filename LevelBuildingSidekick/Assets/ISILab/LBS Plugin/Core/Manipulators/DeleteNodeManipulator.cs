using LBS.ElementView;
using LBS.Graph;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class DeleteNodeManipulator : MouseManipulator 
    {
        private LBSGraphRCController controller;
        private GenericGraphWindow window;

        public DeleteNodeManipulator(GenericGraphWindow window,LBSGraphRCController controller)
        {
            activators.Add(new ManipulatorActivationFilter {button = MouseButton.LeftMouse});
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
            var t = e.target as LBSGraphElement;
            //var t = (T)e.target; 
            if (t == null)
                return;

            // Node
            var node = e.target as LBSNodeView; 
            if (node != null)
            {
                controller.RemoveNode(node.Data);
                window.RefreshView();
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
                window.RefreshView();
                //var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
                //graph.RemoveEdge(edge.Data);
                //t.Clear();
                return;
            }


        }
    }
}