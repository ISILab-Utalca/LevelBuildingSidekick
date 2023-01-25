using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using LBS;
using System;
using System.Linq;
using Utility;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LBS.Graph;
using LBS.Schema;
using LBS.Transformers;
using LBS.ElementView;
using LBS.VisualElements;

namespace LBS.Graph
{
    // Este no deberia ser un controlador general de grafos
    // sino que deberia exitir un controlador para cada uno
    // de los diferentes grafos que podamso crear, asi podemos
    // tener una "GraphView" generica, ademas de una data generica
    // pero podemos guardar los datos especificos y controlarlos
    // de manera especifica (!!!)
    public abstract class LBSGraphController : LBSRepController<LBSGraphData>
    {
        public static readonly int cellSize = 64;

        public LBSNodeViewOld first; // cancercito (!!)
        public int CellSize => cellSize;

        /// <summary>
        /// Constructor for the LBSGraphController class.
        /// </summary>
        /// <param name="view">Graph view that the controller will use.</param>
        /// <param name="data">Graph data that the controller will use.</param>
        public LBSGraphController(LBSGraphView view,LBSGraphData data) : base(view, data)
        {

        }

        /// <summary>
        /// Creates a new node at the given position and adds it to the graph view.
        /// </summary>
        /// <param name="view">The main view in which the node will be added.</param>
        /// <param name="cmpe">The contextual menu populate event that is triggering this method.</param>
        public override void OnContextualBuid(MainViewOld view, ContextualMenuPopulateEvent cmpe)
        {
            var pos = (cmpe.localMousePosition - new Vector2(view.transform.position.x, view.transform.position.y)) / view.scale;

            cmpe.menu.AppendAction("GraphRC/Add Node", (dma) => {
                var n = NewNode(pos);
                AddNodeView(n);
                });
        }

        /// <summary>
        /// Get the name of the layer.
        /// </summary>
        /// <returns> The string "Node Layer". </returns>
        public override string GetName()
        {
            return "Node Layer";
        }

        /// <summary>
        /// Populates the provided MainView with the nodes and edges stored in this layer's data.
        /// Deletes any existing elements in the provided MainView.
        /// </summary>
        /// <param name="view">The MainView to populate</param>
        public override void PopulateView(MainViewOld view)
        {
            this.view = view;
            view.DeleteElements(elements);
            data.GetNodes().ForEach(n => AddNodeView(n));
            data.GetEdges().ForEach(e => AddEdgeView(e));
        }

        /// <summary>
        /// Add an edge view to the view, find the node views that are connected by this edge.
        /// Create the edge view and add the edge view to the list of elements and the view.
        /// </summary>
        /// <param name="edge">Data EdgeView to be added. </param>
        /// <return> Return nothing if one of the nodes views is not found. </return>
        private void AddEdgeView(LBSEdgeDataOld edge)
        {
            var nodeViews = view.graphElements.ToList().Where(e => e is LBSNodeViewOld).Select(e => e as LBSNodeViewOld).ToList();
            var nv1 = nodeViews.Find((n) => {
                return n.Data.Label == edge.FirstNodeLabel;
            });
            var nv2 = nodeViews.Find((n) =>
            {
                return n.Data.Label == edge.SecondNodeLabel;
            });

            if (nv1 == null || nv2 == null)
            {
                Debug.LogWarning("There is no 'NodeView' to which to link this 'EdgeView'.");
                return;
            }

            var edgeView = new LBSDotedEdgeView(nv1, nv2, view);
            nv1.OnMoving += edgeView.ActualizeView;
            nv2.OnMoving += edgeView.ActualizeView;
            elements.Add(edgeView);
            view.AddElement(edgeView);
        }

        /// <summary>
        /// Get the NodeView with from a given label.
        /// </summary>
        /// <param name="label">The label of the node view get.</param>
        /// <returns>The node view with the given label, or null if no such view exists.</returns>
        public virtual LBSNodeViewOld GetNodeViewBylabel(string label)
        {
            foreach (var element in view.graphElements)
            {
                if (element is LBSNodeViewOld)
                {
                    var n = (LBSNodeViewOld)element;
                    if (n != null && n.Data.Label == label)
                        return n;
                }
            }
            return null;
        }

        public void StartDragEdge(LBSNodeDataOld data) // (!!) pasar a manupilator
        {
            Debug.Log("Start edge");
            first = GetNodeViewBylabel(data.Label);
            //proxyEdge = new LBSProxyEdge(first.GetPosition().position,new Vector2(0,0));
            //AddElement(proxyEdge);
        }

        public void EndDragEdge(LBSNodeDataOld data, string label) // (!!) pasar a manipulator
        {
            if (first != null)
            {
                Debug.Log("End Edge");
                var second = GetNodeViewBylabel(data.Label);
                var edge = new LBSEdgeDataOld(first.Data, second.Data);
                //var current = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>(label);
                //current.AddEdge(edge);
                AddEdgeView(edge);
            }

            first = null;
        }

        /// <summary>
        /// Add a new NodeView.
        /// </summary>
        /// <param name="data">Node data to create the new NodeView.</param>
        public void AddNodeView(LBSNodeDataOld data)
        {
            var nodeView = new LBSNodeViewOld(data, view, cellSize);
            nodeView.OnStartDragEdge += StartDragEdge;
            nodeView.OnEndDragEdge += (n) =>  EndDragEdge(n, this.data.Label);
            elements.Add(nodeView);
            view.AddElement(nodeView);
        }

        // Node methods
        /// <summary>
        /// Remove a node from the graph.
        /// </summary>
        /// <param name="node">Node data of the node.</param>
        internal void RemoveNode(LBSNodeDataOld node)
        {
            var graph = data;
            graph.RemoveNode(node);
        }

        /// <summary>
        /// Add a node to the graph.
        /// </summary>
        /// <param name="node">Node data of the new node</param>
        internal virtual void AddNode(LBSNodeDataOld node)
        {
            var graph = data;
            graph.AddNode(node);
        }

        /// <summary>
        /// Create a new node with the given position.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The new node.</returns>
        internal abstract LBSNodeDataOld NewNode(Vector2 position);

        // Edge methods
        /// <summary>
        /// Create and add a new edge by two data nodes.
        /// </summary>
        /// <param name="n1">Data of the first node.</param>
        /// <param name="n2">Data of hte second node.</param>
        /// <returns>Return the new Edge.</returns>
        internal LBSEdgeDataOld NewEdge(LBSNodeDataOld n1, LBSNodeDataOld n2)
        {
            var graph = data;
            var edge = new LBSEdgeDataOld(n1,n2);
            AddEdge(edge);
            return edge;
        }

        /// <summary>
        /// Add a new Edge to the graph.
        /// </summary>
        /// <param name="edge">Edge data to add.</param>
        internal void AddEdge(LBSEdgeDataOld edge)
        {
            var graph = data;
            graph.AddEdge(edge);
        }

        /// <summary>
        /// Remove a Edge of the graph.
        /// </summary>
        /// <param name="edge">Edge data given to remove.</param>
        internal void RemoveEdge(LBSEdgeDataOld edge)
        {
            var graph = data;
            graph.RemoveEdge(edge);
        }
    }

}
