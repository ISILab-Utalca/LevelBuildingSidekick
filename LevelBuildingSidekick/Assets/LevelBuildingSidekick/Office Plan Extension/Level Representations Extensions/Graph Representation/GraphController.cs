using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using System.Linq;

namespace LevelBuildingSidekick.Graph
{
    public class GraphController : LevelRepresentationController
    {
        private List<NodeController> _Nodes;
        public List<NodeController> Nodes
        {
            get
            {
                if (_Nodes == null)
                {
                    _Nodes = new List<NodeController>();
                }
                return _Nodes;
            }
        }

        private List<EdgeController> _Edges;
        public List<EdgeController> Edges
        {
            get
            {
                if (_Edges == null)
                {
                    _Edges = new List<EdgeController>();
                }
                return _Edges;
            }
        }

        private Controller _SelectedItem;
        public EdgeController SelectedEdge
        {
            get
            {
                if (_SelectedItem == null || !(_SelectedItem is EdgeController))
                {
                    return null;
                }
                return _SelectedItem as EdgeController;
            }
            set
            {
                _SelectedItem = value;
            }
        }
        public NodeController SelectedNode
        {
            get
            {
                if (_SelectedItem == null || !(_SelectedItem is NodeController))
                {
                    return null;
                }
                return _SelectedItem as NodeController;
            }
            set
            {
                _SelectedItem = value;
            }
        }

        int step;

        public Vector2 scrollPosition { get; set; }

        public GraphController(Data data) : base(data)
        {
            View = new GraphView(this);
            scrollPosition = Vector2.zero;
        }

        public int[,] ToMatrix(out Vector2Int[] indexes)
        {
            Vector2Int s = LBSController.Instance.CurrentLevel.levelSize / step;
            int[,] tileMatrix = new int[s.x, s.y];
            indexes = new Vector2Int[Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                int x = Nodes[i].Position.x / step;
                int y = Nodes[i].Position.y / step;

                tileMatrix[x, y] = i;
                indexes[i] = new Vector2Int(x, y);
            }
            return tileMatrix;
        }

        //Can be optimized
        public bool[,] AdjacencyMatrix()
        {
            bool[,] adjacency = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                foreach (NodeController n in Nodes[i].neighbors)
                {
                    int index = Array.IndexOf(Nodes.ToArray(), n);
                    if (index < 0)
                    {
                        Debug.LogError("Node not on Graph");
                    }
                    adjacency[i, index] = true;
                }
            }
            return adjacency;
        }

        public List<int>[] AdjacencyIndexes()
        {
            List<int>[] adjacencies = new List<int>[Nodes.Count];
            for(int i = 0; i < Nodes.Count; i++)
            {
                adjacencies[i] = new List<int>();
                foreach(NodeController n in Nodes[i].neighbors)
                {
                    int index = Array.IndexOf(Nodes.ToArray(), n);
                    if(index >= 0 && index > i)
                    {
                        adjacencies[i].Add(index);
                    }
                }
            }
            return adjacencies;
        }

        public Tuple<Vector2Int,Vector2Int>[] EdgesAsSegments()
        {
            Tuple<Vector2Int, Vector2Int>[] segments = new Tuple<Vector2Int, Vector2Int>[Edges.Count];
            for(int i = 0; i < Edges.Count; i++)
            {
                var pos1 = Edges[i].Node1.Position;
                var pos2 = Edges[i].Node1.Position;
                segments[i] = new Tuple<Vector2Int, Vector2Int>(pos1, pos2);
            }
            return segments;
        }

        public NodeController GetNodeAt(Vector2 pos)
        {
            foreach (NodeController n in Nodes)
            {
                if (n.GetRect().Contains(pos))
                {
                    return n;
                }
            }
            return null;
        }

        internal void RemoveNode(NodeController node)
        {
            foreach (EdgeController e in Edges)
            {
                if (e.Contains(node))
                {
                    RemoveEdge(e);
                }
            }
            Nodes.Remove(node);
            if (node.Equals(SelectedNode))
            {
                _SelectedItem = null;
            }
            GraphData d = Data as GraphData;
            d.nodes.Remove(node.Data as NodeData);

        }

        internal void RemoveEdge(EdgeController edge)
        {
            Edges.Remove(edge);
            if (edge.Equals(SelectedEdge))
            {
                _SelectedItem = null;
            }
            GraphData d = Data as GraphData;
            d.edges.Remove(edge.Data as EdgeData);
        }

        public override void LoadData()
        {
            base.LoadData();

            var data = Data as GraphData;
            //Debug.Log("!: " + Data);

            //Nodes = new List<NodeController>();

            foreach (NodeData n in data.nodes)
            {
                var node = Activator.CreateInstance(n.ControllerType, new object[] { n });
                if (node is NodeController)
                {
                    Nodes.Add(node as NodeController);
                    //Nodes[^1].Data = n;
                }
            }

            //Edges = new List<EdgeController>();

            foreach (EdgeData e in data.edges)
            {
                var edge = Activator.CreateInstance(e.ControllerType, new object[] { e });
                if (edge is EdgeController)
                {
                    Edges.Add(edge as EdgeController);
                    //Edges[^1].Data = e;
                }
            }
        }

        public EdgeController GetEdge(NodeController n1, NodeController n2)
        {
            foreach (EdgeController e in Edges)
            {
                if (e.DoesConnect(n1, n2))
                {
                    return e;
                }
            }
            return null;
        }

        internal bool AddNode(NodeData nodeData)
        {
            var node = Activator.CreateInstance(nodeData.ControllerType, new object[] { nodeData });
            if (node is NodeController)
            {
                Nodes.Add(node as NodeController);
                (Data as GraphData).nodes.Add(nodeData);
                return true;
            }
            return false;
        }


        internal bool AddEdge(EdgeData edgeData)
        {
            var edge = Activator.CreateInstance(edgeData.ControllerType, new object[] { edgeData });
            if (edge is EdgeController)
            {
                var e = edge as EdgeController;
                Edges.Add(e);
                (Data as GraphData).edges.Add(edgeData);
                return true;
            }
            return false;
        }


        public Vector2 FartherPosition()
        {
            if (Nodes.Count == 0)
            {
                return Vector2.zero;
            }
            var x = Nodes.OrderBy((n) => n.Position.x).Last().Position.x;
            var y = Nodes.OrderBy((n) => n.Position.y).Last().Position.y;
            return new Vector2(x,y);
        }

        public override void Update()
        {
            base.Update();
        }
    }

}
