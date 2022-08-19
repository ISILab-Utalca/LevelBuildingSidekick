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

        public List<LBSNodeData> Nodes
        {
            get
            {
                var graph = (Data as LBSGraphData);
                if (graph.nodes == null)
                {
                    graph.nodes = new List<LBSNodeData>();
                }
                return graph.nodes;
            }
        }
        public List<LBSEdgeData> Edges
        {
            get
            {
                var graph = (Data as LBSGraphData);
                if (graph.edges == null)
                {
                    graph.edges = new List<LBSEdgeData>();
                }
                return graph.edges;
            }
        }

        private Controller _SelectedItem;

        public int CellSize
        {
            get
            {
                return (Data as LBSGraphData).cellSize;
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

        public LBSGraphController(LBSGraphView view) : base(LBSController.CurrentLevel.GetRepresentation<LBSGraphData>())
        {
            View = view;
        }

        public override void LoadData()
        {
            
        }

        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
        }

       


        // Node methods
        internal void RemoveNode(LBSNodeData node)
        {
            for(int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Contains(node.label))
                {
                    RemoveEdge(Edges[i]);
                }
            }
            Nodes.Remove(node);
        }

        internal void AddNode(LBSNodeData nodeData)
        {
            var index = Nodes.Count;
            while (Nodes.Any(n => n.label == nodeData.label))
            {
                index++;
                nodeData.label = "Node: " + index;
            }
            nodeData.Exist = v => !Nodes.Any(n => n.label == v);
            Nodes.Add(nodeData);
        }

        internal LBSNodeData NewNode(Vector2 position)
        {
            var graph = Data as LBSGraphData;
            LBSNodeData node = new LBSNodeData("Node: " + graph.nodes.Count, position, CellSize);
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

        public LBSEdgeData GetEdge(LBSNodeData n1, LBSNodeData n2)
        {
            foreach (LBSEdgeData e in Edges)
            {
                if (e.Contains(n1.label) && e.Contains(n2.label))
                {
                    return e;
                }
            }
            return null;
        } //(?)

        internal bool AddEdge(LBSEdgeData edgeData)
        {
            if(Edges.Any(e => e.Contains(edgeData.FirstNodeLabel) && e.Contains(edgeData.SecondNodeLabel)))
            {
                return false;
            }
            Edges.Add(edgeData);
            return true;
        }

        internal void RemoveEdge(LBSEdgeData edge)
        {
            Edges.Remove(edge);
        }


        public override void Update()
        {

        }
    }

}
