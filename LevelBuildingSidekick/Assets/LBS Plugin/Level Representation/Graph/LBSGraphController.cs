using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using LevelBuildingSidekick;
using System;
using System.Linq;
using Utility;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LevelBuildingSidekick.Graph;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphController : Controller
    {
        public int cellSize = 32;

        //public List<LBSNodeData> Nodes
        //{
        //    get
        //    {
        //        var graph = (Data as LBSGraphData);
        //        if (graph.nodes == null)
        //        {
        //            graph.nodes = new List<LBSNodeData>();
        //        }
        //        return graph.nodes;
        //    }
        //}

        //public List<LBSEdgeData> Edges
        //{
        //    get
        //    {
        //        var graph = (Data as LBSGraphData);
        //        if (graph.edges == null)
        //        {
        //            graph.edges = new List<LBSEdgeData>();
        //        }
        //        return graph.edges;
        //    }
        //}

        private Controller _SelectedItem;

        public int CellSize
        {
            get
            {
                return cellSize;
            }
        }

        public List<LBSNodeData> SelectedNodes
        {
            get
            {
                return (View as LBSGraphView).SelectedNodes.Select(v => v.Data).ToList();
            }
        } // no se usa (!!!)

        public List<LBSEdgeData> SelectedEdges
        {
            get
            {
                return null;//(View as LBSGraphView).SelectedEdges.Select(v => v.data ).ToList();
            }
        } // no se usa (!!!)

        public LBSGraphController(LBSGraphView view) : base(LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>())
        {
            View = view;
        }

        public override void LoadData()
        {
            
        }

        public void Clear()
        {
            (Data as LBSGraphData).Clear();
        }

       


        // Node methods


        internal void RemoveNode(LBSNodeData node)
        {
            var graph = (Data as LBSGraphData);
            graph.RemoveNode(node);
        }

        internal void AddNode(LBSNodeData node)
        {
            var graph = (Data as LBSGraphData);
            graph.AddNode(node);
        }

        internal LBSNodeData NewNode(Vector2 position)
        {
            var graph = (Data as LBSGraphData);
            LBSNodeData node = new LBSNodeData("Node: " + graph.NodeCount(), position, CellSize);
            AddNode(node);
            return node;
        }

        // Edge methods
        internal LBSEdgeData NewEdge(LBSNodeData n1, LBSNodeData n2)
        {
            var graph = Data as LBSGraphData;
            var edge = new LBSEdgeData(n1,n2);
            AddEdge(edge);
            return edge;
        }

        //public LBSEdgeData GetEdge(LBSNodeData n1, LBSNodeData n2)
        //{
        //    foreach (LBSEdgeData e in Edges)
        //    {
        //        if (e.Contains(n1.Label) && e.Contains(n2.Label))
        //        {
        //            return e;
        //        }
        //    }
        //    return null;
        //} //(?)

        internal void AddEdge(LBSEdgeData edge)
        {
            var graph = (Data as LBSGraphData);
            graph.AddEdge(edge);

        }

        internal void RemoveEdge(LBSEdgeData edge)
        {
            var graph = (Data as LBSGraphData);
            graph.RemoveEdge(edge);
        }

        //internal bool ContainEdge(LBSEdgeData edge)
        //{
        //    return Edges.Contains(edge);
        //}


        public override void Update()
        {
            
        }
    }

}
