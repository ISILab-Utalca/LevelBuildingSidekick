
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
            QuestNode from,
            QuestNode to,
            List<QuestEdge> existingEdges,
            QuestNode root,
            QuestGraph graphContext,
            out string message,
            out LogType logType)
        {
            if (from == null || to == null)
            {
                message = "Must select two nodes";
                logType = LogType.Error;
                return false;
            }

            if (from == to)
            {
                message = "Cannot connect a node to itself";
                logType = LogType.Error;
                return false;
            }

            if (to.Equals(root))
            {
                message = "The start node cannot be the second element of a connection";
                logType = LogType.Error;
                return false;
            }

            if (!from.IsValidFrom())
            {
                message = "The first node is already connected";
                logType = LogType.Error;
                return false;
            }

            var reverseEdge = new QuestEdge(to, from);
            var edge = new QuestEdge(from, to);

            if (existingEdges.Contains(edge))
            {
                message = "The connection already exists";
                logType = LogType.Error;
                return false;
            }

            if (existingEdges.Contains(reverseEdge))
            {
                message = "The reverse connection already exists";
                logType = LogType.Error;
                return false;
            }

            if (IsLooped(edge, existingEdges))
            {
                message = "Invalid connection, loop detected";
                logType = LogType.Error;
                return false;
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

            var visited = new HashSet<QuestNode>();
            var queue = new Queue<QuestNode>();
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

                if (ReferenceEquals(current, edge.From))
                    return true;

                if (!visited.Add(current)) continue;

                foreach (var e in questEdges)
                {
                    if (e.From == current && e.To != null && !visited.Contains(e.To))
                        queue.Enqueue(e.To);
                }
            }

            return false;
        }

        /// <summary>
        /// Validates whether the given node has the correct connections based on its type.
        /// </summary>
        public static bool HasRequiredConnection(QuestNode node, List<QuestEdge> questEdges, QuestNode root)
        {
            if (node == null) return false;

            bool hasNext = questEdges.Any(e => e.From == node);
            bool hasPrev = questEdges.Any(e => e.To == node);

            return node.NodeType switch
            {
                NodeType.Start => hasNext && !hasPrev,
                NodeType.Goal => hasPrev && !hasNext,
                _ => hasPrev && hasNext
            };
        }
    }
}
