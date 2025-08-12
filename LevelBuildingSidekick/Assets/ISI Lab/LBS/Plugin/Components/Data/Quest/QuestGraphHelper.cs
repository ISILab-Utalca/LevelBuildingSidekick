
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    public static class QuestGraphHelper
    {
        /// <summary>
        /// Validates whether an edge can be added between two nodes.
        /// Returns true if the edge is valid, false otherwise.
        /// </summary>
        public static bool IsValidEdge(
            QuestEdge edge,
            QuestGraph graph,
            out string message,
            out LogType logType)
        {
            if (!edge.From.Any() || edge.To == null)
            {
                message = "Must select two nodes";
                logType = LogType.Error;
                return false;
            }

            if (edge.From.Contains(edge.To))
            {
                message = "Cannot connect a node to itself";
                logType = LogType.Error;
                return false;
            }

            if (edge.To.Equals(graph.Root))
            {
                message = "The start node cannot be the second element of a connection";
                logType = LogType.Error;
                return false;
            }
            

            foreach (var from in edge.From)
            {
                var reverseEdge = new QuestEdge(edge.To, from);
                
                if (graph.QuestEdges.Contains(edge))
                {
                    message = "The connection already exists";
                    logType = LogType.Error;
                    return false;
                }

                if (graph.QuestEdges.Contains(reverseEdge))
                {
                    message = "The reverse connection already exists";
                    logType = LogType.Error;
                    return false;
                }

                if (IsLooped(edge, graph.QuestEdges))
                {
                    message = "Invalid connection, loop detected";
                    logType = LogType.Error;
                    return false;
                }
            }
            
            message = string.Empty;
            logType = LogType.Log;
            return true;
        }
        
        /// <summary>
        /// Checks whether adding the given edge would cause a cycle in the graph.
        /// </summary>
        public static bool IsLooped(QuestEdge edge, List<QuestEdge> questEdges)
        {
            if (edge == null || edge.From == null || edge.To == null)
                return false;

            var visited = new HashSet<GraphNode>();
            var queue = new Queue<GraphNode>();
            queue.Enqueue(edge.To);

            int iteration = 0;
            const int maxIterations = 1000;

            while (queue.Count > 0)
            {
                if (++iteration > maxIterations)
                {
                    Debug.LogError("IsLooped exceeded max iterations; possible graph corruption");
                    return true;
                }

                var current = queue.Dequeue();

                foreach (var from in edge.From)
                {
                    if (ReferenceEquals(current, from))
                        return true;
                }
                
                if (!visited.Add(current)) continue;

                foreach (var e in questEdges)
                {
                    foreach (var from in e.From)
                    {
                        if (from == current && e.To != null && !visited.Contains(e.To))
                            queue.Enqueue(e.To);
                    }
                   
                }
            }

            return false;
        }

        /// <summary>
        /// Validates whether the given node has the correct connections based on its type.
        /// </summary>
        public static bool HasRequiredQuestConnections(QuestNode node, QuestGraph graph)
        {
            if (node == null) return false;

            bool hasNext = graph.GetRoots(node).Any();
            bool hasPrev = graph.GetBranches(node).Any();

            switch (node.NodeType)
            {
                case QuestNode.ENodeType.Start:
                    return hasNext && !hasPrev;
                case QuestNode.ENodeType.Middle:
                    return hasPrev && hasNext;
                case QuestNode.ENodeType.Goal:
                    return hasPrev && !hasNext;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
