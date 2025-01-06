using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Optimization.Data
{
    public class Graph
    {
        public class Node
        {
            public string name;
            public int id;
            public Vector2 pos;
            public Vector2Int minArea;
            public Vector2Int maxArea;
            public Color color;
        }

        public struct Edge
        {
            public Node n1, n2;
        }

        public string name;

        public List<Node> nodes = new List<Node>();
        public List<Edge> edges = new List<Edge>();

        public Graph(string name)
        {
            this.name = name;
        }

        public Vector2 Center
        {
            get
            {
                var sum = new Vector2(0, 0);

                if (nodes.Count > 0)
                {
                    Debug.Log("There are no nodes in the graph");
                    return sum;
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    sum += nodes[i].pos;
                }

                return sum / nodes.Count;
            }
        }

        public Node GetMaxConection()
        {
            var dict = new Dictionary<Node, int>();

            foreach (var edge in edges)
            {
                if (dict.ContainsKey(edge.n1))
                {
                    dict[edge.n1]++;
                }
                else
                {
                    dict.Add(edge.n1, 1);
                }

                if (dict.ContainsKey(edge.n2))
                {
                    dict[edge.n2]++;
                }
                else
                {
                    dict.Add(edge.n2, 1);
                }
            }

            return dict.OrderByDescending(x => x.Value).First().Key;
        }

        public List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            foreach (var edge in edges)
            {
                if (edge.n1.Equals(node))
                    neighbors.Add(edge.n2);
                if (edge.n2.Equals(node))
                    neighbors.Add(edge.n1);
            }
            return neighbors;
        }
    }
}