using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components.TileMap;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestBehaviour))]
    public class QuestGraphDrawer : Drawer
    {
        public QuestGraphDrawer() : base() { }
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var behaviour = target as QuestBehaviour;

            var quest = behaviour?.Graph;
            if (quest == null) return;
            
            var nodeViews = new Dictionary<QuestNode, QuestNodeView>();
           // view.ClearLayerView(behaviour.OwnerLayer, true);
          //  PaintNewTiles(quest, behaviour, nodeViews, view);
            LoadAllTiles(quest, behaviour, nodeViews, view);
           

            // TODO: Does this drawer actually needs an update in its visualElements? I don't understand it enough to tell.
            
            if (!Loaded)
            {
                LoadAllTiles(quest, behaviour, nodeViews, view);
                Loaded = true;
            }
        }

        private void PaintNewTiles(QuestGraph quest, QuestBehaviour behaviour, Dictionary<QuestNode, QuestNodeView> nodeViews, MainView view)
        {
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
        }

        private void LoadAllTiles(QuestGraph quest, QuestBehaviour behaviour, Dictionary<QuestNode, QuestNodeView> nodeViews, MainView view)
        {
            QuestNodeBehaviour qnb = LBSLayerHelper.GetObjectFromLayer<QuestNodeBehaviour>(quest.OwnerLayer);
            foreach (var node in quest.QuestNodes)
            {
                if (!nodeViews.TryGetValue(node, out var nodeView) || nodeView == null)
                {
                    nodeView = CreateNodeView(node, quest, behaviour);
                    nodeViews[node] = nodeView;
                }

                nodeViews[node].IsSelected(false);
                if (qnb.SelectedQuestNode is not null)
                {
                    nodeViews[node].IsSelected(node == qnb.SelectedQuestNode);
                }
                
                view.AddElement(quest.OwnerLayer, node, nodeView);
                behaviour.Keys.Add(node);
            }

            foreach (var edge in quest.QuestEdges)
            {
                if (!nodeViews.TryGetValue(edge.First, out var n1) || n1 == null) continue;
                if (!nodeViews.TryGetValue(edge.Second, out var n2) || n2 == null) continue;

                var edgeView = CreateEdgeView(edge, n1, n2);
                view.AddElement(quest.OwnerLayer, edge, edgeView);
                behaviour.Keys.Add(edge);
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not QuestBehaviour behaviour) return;
            
            foreach (object tile in behaviour.Keys)
            {
                foreach (var graphElement in view.GetElements(behaviour.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.style.display = DisplayStyle.Flex;
                }
            }
        }
        public override void HideVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not QuestBehaviour behaviour) return;
            
            foreach (object tile in behaviour.Keys)
            {
                if (tile == null) continue;

                var elements = view.GetElements(behaviour.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
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
            if (node.NodeType == NodeType.Start) { }
                
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