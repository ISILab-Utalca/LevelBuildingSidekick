using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using System.Linq;
using Utility;

namespace LevelBuildingSidekick.Graph
{
    public class GraphController : LevelRepresentationController
    {
        private HashSet<NodeController> _Nodes;
        public HashSet<NodeController> Nodes
        {
            get
            {
                if (_Nodes == null)
                {
                    _Nodes = new HashSet<NodeController>();
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
        public int CellSize
        {
            get
            {
                return (Data as GraphData).cellSize;
            }
        }

        public GraphController(Data data) : base(data)
        {
            View = new GraphView(this);
            //scrollPosition = Vector2.zero;
        }

        public override void LoadData()
        {
            base.LoadData();

            var data = Data as GraphData;
            //Debug.Log("!: " + Data);

            //Nodes = new List<NodeController>();
            if (data.nodes == null)
            {
                data.nodes = new List<NodeData>();
            }
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
            if (data.edges == null)
            {
                data.edges = new List<EdgeData>();
            }
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

        public Dictionary<int, Vector2Int> ToMatrixPositions(Vector2Int size)
        {
            //int[,] tileMatrix = new int[size.x, size.y];
            var closer = CloserPosition();
            var farther = FartherPosition();
            //var start = Mathf.Min(closer.x, closer.y);
            //var end = Mathf.Max(farther.x, farther.y);
            var Size = new Vector2Int(Mathf.Min(closer.x, closer.y), Mathf.Max(farther.x, farther.y));
            //float step = Mathf.Max(Size.x / size.x, Size.y / size.y);
            Dictionary<int, Vector2Int> indexes = new Dictionary<int, Vector2Int>();
            foreach (NodeController n in Nodes)
            {
                /*int x = (int)(n.Position.x / step);
                int y = (int)(n.Position.y / step);
                if(x >= size.x || y >= size.y)
                {
                    Debug.LogWarning("Node out of Bounds");
                    continue;
                }*/
                //tileMatrix[x, y] = n.ID;
                if (!indexes.ContainsKey(n.ID))
                {
                    indexes.Add(n.ID, ToMatrixPosition(n.ID, Size, size));
                }
            }
            return indexes;
        }
        public Vector2Int ToMatrixPosition(int ID, Vector2Int size)
        {
            var closer = CloserPosition();
            var farther = FartherPosition();
            //var start = Mathf.Min(closer.x, closer.y);
            //var end = Mathf.Max(farther.x, farther.y);
            var l = Mathf.Max(farther.x, farther.y) - Mathf.Min(closer.x, closer.y);
            //Debug.Log("L: " + l);
            var Size = new Vector2(l, l);
            var node = Nodes.ToList().Find((n) => n.ID == ID);
            var pos = node.Position - closer;
            //Debug.Log("Pos: " + node.Position);
            //Debug.Log();


            return new Vector2Int((int)(0.1f * size.x + pos.x * ((0.8f * size.x) / Size.x)),
                (int)(0.1f * size.y + pos.y * ((0.8f * size.y) / Size.y)));
        }
        public Vector2Int ToMatrixPosition(int ID, Vector2Int graphSize, Vector2Int size)
        {
            //var closer = CloserPosition();
            //var farther = FartherPosition();
            //var start = Mathf.Min(closer.x, closer.y);
            //var end = Mathf.Max(farther.x, farther.y);
            //var Size = new Vector2Int(Mathf.Min(closer.x, closer.y), Mathf.Max(farther.x, farther.y));
            var node = Nodes.ToList().Find((n) => n.ID == ID);

            return new Vector2Int((int)(0.1f * size.x + node.Centroid.x * ((0.8f * size.x) / graphSize.x)),
                (int)(0.1f * size.y + node.Centroid.y * ((0.8f * size.y) / graphSize.y)));
        }
        //Can be optimized
        public bool[,] AdjacencyMatrix()
        {
            var nodes = Nodes.ToList();
            bool[,] adjacency = new bool[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                foreach (NodeController n in nodes[i].neighbors)
                {
                    int index = nodes.FindIndex((node) => node.ID == n.ID);
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
            var nodes = Nodes.ToList();
            List<int>[] adjacencies = new List<int>[Nodes.Count];
            for(int i = 0; i < Nodes.Count; i++)
            {
                adjacencies[i] = new List<int>();
                foreach(NodeController n in nodes[i].neighbors)
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
        public EdgeController GetEdgeAt(Vector2 pos)
        {
            List<Tuple<float, EdgeController>> edges = new List<Tuple<float, EdgeController>>();
            foreach (EdgeController e in Edges)
            {
                float dist = MathTools.PointToLineDistance(pos, e.Node1.GetAnchor(e.Node2.Centroid), e.Node2.GetAnchor(e.Node1.Centroid));
                //Debug.Log("E: " + e + " - N1: " + e.Node1 + " - N2: " + e.Node2 + "D: " + dist);
                if (dist < 30)//threshold should be parametrized
                {
                    edges.Add(new Tuple<float, EdgeController>(dist,e));
                }
                else
                {
                    //  Debug.Log("Dist: " + dist);
                }
            }
            if(edges.Count == 0)
            {
                return null;
            }
            var tuple = edges.OrderBy((t) => t.Item1).First();
            if(tuple == null)
            {
                return null;
            }
            return tuple.Item2;
        }
        internal void RemoveNode(NodeController node)
        {
            for(int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Contains(node))
                {
                    RemoveEdge(Edges[i]);
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
                (node as NodeController).Radius = CellSize / 2;
                (node as NodeController).Exist = (s) => {
                    //Debug.Log("name: " + s);  
                    return Nodes.ToList().Find((n) => n.Label == s) == null; };
                if(Nodes.Add(node as NodeController))
                {
                    (Data as GraphData).nodes.Add(nodeData);
                    return true;
                }
            }
            return false;
        }
        internal bool AddEdge(EdgeData edgeData)
        {
            var edge = Activator.CreateInstance(edgeData.ControllerType, new object[] { edgeData });
            if (edge is EdgeController)
            {
                var e = edge as EdgeController;
                var n1 = Nodes.ToList().Find((n) => n.Data == edgeData.node1);
                var n2 = Nodes.ToList().Find((n) => n.Data == edgeData.node2);
                if(n1 == null || n2 == null)
                {
                    Debug.LogError("Something went wrong");
                    return false;
                }
                n1.neighbors.Add(n2);
                n2.neighbors.Add(n1);
                e.Node1 = n1;
                e.Node2 = n2;
                Edges.Add(e);
                (Data as GraphData).edges.Add(edgeData);
                return true;
            }
            return false;
        }
        public Vector2Int FartherPosition()
        {
            if (Nodes.Count == 0)
            {
                return Vector2Int.zero;
            }
            var x = Nodes.OrderBy((n) =>
            {
                //var offset = CellSize * (n.ProportionType == ProportionType.RATIO ? n.Ratio.x : n.Width.x);
                return n.Position.x; //+ offset;
            }).Last().Position.x;
            var y = Nodes.OrderBy((n) => 
            {
                //var offset = CellSize * (n.ProportionType == ProportionType.RATIO ? n.Ratio.y : n.Width.y);
                return n.Position.y; //+ offset;
            }).Last().Position.y;
            return new Vector2Int(x,y);
        }
        public Vector2Int CloserPosition()
        {
            if (Nodes.Count == 0)
            {
                return Vector2Int.zero;
            }
            var x = Nodes.OrderBy((n) => n.Position.x).First().Position.x;
            var y = Nodes.OrderBy((n) => n.Position.y).First().Position.y;
            return new Vector2Int(x, y);
        }
        public override void Update()
        {
            base.Update();
        }
    }

}
