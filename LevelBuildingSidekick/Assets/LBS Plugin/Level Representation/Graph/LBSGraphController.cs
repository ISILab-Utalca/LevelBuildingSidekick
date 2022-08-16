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
        // la var (edgeThreshold) esto yo lo pondria a como una variable estatica en 
        // otro lado ya que solo se ocupa para seleccionar las edges,
        // tal vez en la data de la herramienta seleccionar
        // (tal vez parametrizado o incluso estatico en algun lado)
        public float edgeThreshold = 10; 
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
        }

        public List<LBSEdgeData> SelectedEdges
        {
            get
            {
                return (View as LBSGraphView).SelectedEdges.Select(v => v.Edge).ToList();
            }
        }

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

        internal void RemoveEdge(LBSEdgeData edge)
        {
            Edges.Remove(edge);
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
        internal bool AddEdge(LBSEdgeData edgeData)
        {
            if(Edges.Any(e => e.Contains(edgeData.FirstNodeLabel) && e.Contains(edgeData.SecondNodeLabel)))
            {
                return false;
            }
            Edges.Add(edgeData);
            return true;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }

}
