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

namespace LBS.Graph
{
    // Este no deberia ser un controlador general de grafos
    // sino que deberia exitir un controlador para cada uno
    // de los diferentes grafos que podamso crear, asi podemos
    // tener una "GraphView" generica, ademas de una data generica
    // pero podemos guardar los datos especificos y controlarlos
    // de manera especifica (!!!)
    public class LBSGraphRCController : LBSRepController<LBSGraphData>
    {
        public static readonly int cellSize = 32;

        public LBSNodeView first; // cancercito (!!)
        public int CellSize => cellSize;

        public LBSGraphRCController(GraphView view,LBSGraphData data) : base(view, data)
        {

        }

        public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
        {
            var pos = (cmpe.localMousePosition - new Vector2(view.transform.position.x, view.transform.position.y)) / view.scale;

            cmpe.menu.AppendAction("GraphRC/Add Node", (dma) => {
                var n = NewNode(pos);
                AddNodeView(n);
                });
        }

        public override string GetName()
        {
            return "Node Layer";
        }

        public void GenerateSchema() 
        {
            Debug.Log("[Generate Tile map]");
            var g = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>(); // peligroso buscar otra forma (!)
            var tm = new GraphToTileMap().Transform(g);
            LBSController.CurrentLevel.data.AddRepresentation(tm);
        }

        public override void PopulateView(MainView view)
        {
            this.view = view;
            view.DeleteElements(elements);
            data.GetNodes().ForEach(n => AddNodeView(n));
            data.GetEdges().ForEach(e => AddEdgeView(e));
        }

        private void AddEdgeView(LBSEdgeData edge)
        {
            var nodeViews = view.graphElements.ToList().Where(e => e is LBSNodeView).Select(e => e as LBSNodeView).ToList();
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

        public LBSNodeView GetNodeViewBylabel(string label) 
        {
            foreach (var element in view.graphElements)
            {
                if (element is LBSNodeView)
                {
                    var n = (LBSNodeView)element;
                    if (n != null && n.Data.Label == label)
                        return n;
                }
            }
            return null;
        }

        public void StartDragEdge(LBSNodeData data) // pasar a manupilator
        {
            Debug.Log("Start edge");
            first = GetNodeViewBylabel(data.Label);
            //proxyEdge = new LBSProxyEdge(first.GetPosition().position,new Vector2(0,0));
            //AddElement(proxyEdge);
        }

        public void EndDragEdge(LBSNodeData data) // pasar a manipulator
        {
            if (first != null)
            {
                Debug.Log("End Edge");
                var second = GetNodeViewBylabel(data.Label);
                var edge = new LBSEdgeData(first.Data, second.Data);
                var current = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
                current.AddEdge(edge);
                AddEdgeView(edge);
            }

            first = null;
        }

        public void AddNodeView(LBSNodeData data)
        {
            var nodeView = new LBSNodeView(data);
            nodeView.OnStartDragEdge += StartDragEdge;
            nodeView.OnEndDragEdge += EndDragEdge;
            elements.Add(nodeView);
            view.AddElement(nodeView);
        }


        // Node methods
        internal void RemoveNode(LBSNodeData node)
        {
            var graph = data;
            graph.RemoveNode(node);
        }

        internal void AddNode(LBSNodeData node)
        {
            var graph = data;
            graph.AddNode(node);
        }

        internal LBSNodeData NewNode(Vector2 position)
        {
            var graph = data;
            LBSNodeData node = new RoomCharacteristicsData("Node: " + graph.NodeCount(), position, CellSize);
            AddNode(node);
            return node;
        }

        // Edge methods
        internal LBSEdgeData NewEdge(LBSNodeData n1, LBSNodeData n2)
        {
            var graph = data;
            var edge = new LBSEdgeData(n1,n2);
            AddEdge(edge);
            return edge;
        }

        internal void AddEdge(LBSEdgeData edge)
        {
            var graph = data;
            graph.AddEdge(edge);

        }

        internal void RemoveEdge(LBSEdgeData edge)
        {
            var graph = data;
            graph.RemoveEdge(edge);
        }
    }

}
