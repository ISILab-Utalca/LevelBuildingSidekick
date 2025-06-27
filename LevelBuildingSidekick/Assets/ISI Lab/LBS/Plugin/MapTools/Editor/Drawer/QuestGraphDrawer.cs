using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor.Experimental.GraphView;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestBehaviour))]
    public class QuestGraphDrawer : Drawer
    {
        private bool _loaded = false;
        public QuestGraphDrawer() : base() { }
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var behaviour = target as QuestBehaviour;

            var quest = behaviour?.Graph;
            if (quest == null) return;
            
            var nodeViews = new Dictionary<QuestNode, QuestNodeView>();

            // Paint new Nodes
            foreach (var node in quest.RetrieveNewNodes())
            {
                var nodeView = CreateNodeView(node, quest, behaviour);
                
                nodeViews.Add(node, nodeView);
                // Stores using QuestNode as key
                view.AddElement(quest.OwnerLayer, node, nodeView);
            }
            
            // Paint new Edges
            foreach (var edge in quest.RetrieveNewEdges())
            {
                if (!nodeViews.TryGetValue(edge.First, out var n1) || n1 == null) continue;
                if (!nodeViews.TryGetValue(edge.Second, out var n2) || n2 == null) continue;
                
                var edgeView = CreateEdgeView(edge, n1, n2);
                // Stores using QuestEdge as key
                view.AddElement(quest.OwnerLayer, edge, edgeView);
            }
            
            // Paint everything if loading
            if (!_loaded)
            {
                _loaded = true;
                foreach (var node in quest.QuestNodes)
                {
                    var nodeView = CreateNodeView(node, quest, behaviour);
                
                    nodeViews.Add(node, nodeView);
                    // Stores using QuestNode as key
                    view.AddElement(quest.OwnerLayer, node, nodeView);
                }
                foreach (var edge in quest.QuestEdges)
                {
                    if (!nodeViews.TryGetValue(edge.First, out var n1) || n1 == null) continue;
                    if (!nodeViews.TryGetValue(edge.Second, out var n2) || n2 == null) continue;
                
                    var edgeView = CreateEdgeView(edge, n1, n2);
                    // Stores using QuestEdge as key
                    view.AddElement(quest.OwnerLayer, edge, edgeView);
                }
            }
        }

        public override void HideVisuals(object target, MainView view, Vector2 teselationSize)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowVisuals(object target, MainView view, Vector2 teselationSize)
        {
            throw new System.NotImplementedException();
        }

        private LBSQuestEdgeView CreateEdgeView(QuestEdge edge, QuestNodeView n1, QuestNodeView n2)
        {
            n1.SetBorder(edge.First);
            n2.SetBorder(edge.Second);
            
            return new LBSQuestEdgeView(edge, n1, n2, 4, 4);
        }
        
        private QuestNodeView CreateNodeView(QuestNode node, QuestGraph quest, QuestBehaviour behaviour)
        {
            /*  Start Node is now assigned by the user. Right click on a node to make it root */
            if (node.NodeType == NodeType.start) { }
                
            var nodeView = new QuestNodeView(node);
            var size = LBSSettings.Instance.general.TileSize * quest.NodeSize;

            nodeView.SetPosition(new Rect(node.Position, size));
                
            if (!(node.Target.Rect.width == 0 || node.Target.Rect.height == 0))
            {
                var rectView = new DottedAreaFeedback(); // TODO make this a DottedAreaUnique for quest

                var rectSize = behaviour.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
                var start = new Vector2(node.Target.Rect.min.x, -node.Target.Rect.min.y) * rectSize;
                var end = new Vector2(node.Target.Rect.max.x, -node.Target.Rect.max.y) * rectSize;
                rectView.SetPosition(Rect.zero);
                rectView.ActualizePositions(start.ToInt(), end.ToInt());
                rectView.SetColor(Color.blue);
                    
                nodeView.Add(rectView);
            }
            
            return nodeView;
        }
    }
}