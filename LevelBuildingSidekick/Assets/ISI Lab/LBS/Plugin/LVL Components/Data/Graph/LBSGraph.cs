using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Components.Graph;
using LBS.Settings;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS.Modules
{

    [System.Serializable]
    public class LBSGraph : LBSModule
    {
        [JsonRequired, SerializeReference]
        private List<LBSNode> nodes = new List<LBSNode>();
        [JsonRequired, SerializeReference]
        private List<LBSEdge> edges = new List<LBSEdge>();

        [JsonIgnore]
        public List<LBSNode> Nodes => new List<LBSNode>(nodes);
        [JsonIgnore]
        public List<LBSEdge> Edges => new List<LBSEdge>(edges);


        public event Action<LBSGraph, LBSNode> OnRemoveNode;

        public LBSGraph()
        {
        }

        public LBSGraph(List<LBSNode> nodes, List<LBSEdge> edges)
        {
            this.nodes = nodes;
            this.edges = edges;
        }

        public void AddNode(LBSNode node)
        {
            if (nodes.Contains(node))
                return;

            OnChanged?.Invoke(this, null, new List<object>() { node });
            nodes.Add(node);
        }

        public void RemoveNode(LBSNode node)
        {
            if (!nodes.Contains(node))
                return;

            OnRemoveNode?.Invoke(this, node);
            OnChanged?.Invoke(this, new List<object>() { node }, null);
            nodes.Remove(node);
            var toR = edges.Where(e => e.FirstNode.Equals(node) || e.SecondNode.Equals(node)).ToList();

            OnChanged?.Invoke(this, new List<object>() { toR }, null);
            foreach (var e in toR)
            {
                edges.Remove(e);
            }

        }

        public LBSNode GetNode(Vector2 position)
        {
            foreach (var n in nodes)
            {
                var r = new Rect(n.Position, Owner.TileSize * LBSSettings.Instance.general.TileSize);

                if (r.Contains(position))
                    return n;
            }
            return null;
        }

        public void AddEdge(LBSNode first, LBSNode second)
        {
            if (!nodes.Contains(first) || !nodes.Contains(second))
                return;

            if (edges.Any(e => e.FirstNode.Equals(first) && e.SecondNode.Equals(second) || e.FirstNode.Equals(second) || e.SecondNode.Equals(first)))
                return;

            OnChanged?.Invoke(this, null, new List<object>() { new LBSEdge(first, second) });
            edges.Add(new LBSEdge(first, second));
        }

        public void RemoveEdge(LBSEdge edge)
        {
            if (!edges.Contains(edge))
                return;

            OnChanged?.Invoke(this, new List<object>() { edge }, null);
            edges.Remove(edge);
        }

        public LBSEdge RemoveEdge(Vector2 position, float delta)
        {
            foreach (var e in edges)
            {
                var dist = position.DistanceToLine(e.FirstNode.Position, e.SecondNode.Position);
                if (dist < delta)
                {
                    OnChanged?.Invoke(this, new List<object>() { e }, null);
                    edges.Remove(e);
                    return e;
                }

            }
            return null;
        }

        public override object Clone()
        {
            var nodes = this.nodes.Clone();
            var edges = this.edges.Clone();
            return new LBSGraph(nodes, edges);
        }

        public override void Clear()
        {
            nodes.Clear();
            edges.Clear();
        }

        public override bool IsEmpty()
        {
            return nodes.Count == 0;
        }
    }
}