using System;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Modules;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestBehaviour))]
    public class QuestGraphDrawer : Drawer
    {
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not QuestBehaviour behaviour) return;
            if (behaviour.OwnerLayer is not { } layer) return;
            
            var graph = behaviour.Graph;
            if (graph == null) return;
            
            layer.OnChange += () =>
            {
                // Reset layer input when changing to another layer
                graph.SelectedQuestNode = null;
                behaviour.ActionToSet = String.Empty;
                QuestNodeView.Deselect();

            };
            
            var nodeViews = new Dictionary<GraphNode, QuestNodeView>();
           //  view.ClearLayerContainer(behaviour.OwnerLayer, true);
            // PaintNewTiles(quest, behaviour, nodeViews, view);
            

           // view.ClearLayerComponentView(behaviour.OwnerLayer, behaviour);
           // view.ClearLayerComponentView(behaviour.OwnerLayer, behaviour.Graph);
            LoadAllTiles(graph, behaviour, nodeViews, view);
 

            // TODO: Does this drawer actually needs an update in its visualElements? I don't understand it enough to tell.
            
            if (!Loaded)
            {
                LoadAllTiles(graph, behaviour, nodeViews, view);
                Loaded = true;
            }
        }

        private void LoadAllTiles(QuestGraph questGraph, QuestBehaviour behaviour, Dictionary<GraphNode, QuestNodeView> nodeViews, MainView view)
        {
            QuestNodeView.Deselect();
            
            foreach (var node in questGraph.GetQuestNodes())
            {
                if (!nodeViews.TryGetValue(node, out var nodeView) || nodeView == null)
                {
                    nodeView = CreateNodeView(node, questGraph);
                    nodeViews[node] = nodeView;
                }
                
                if (Equals(LBSMainWindow.Instance._selectedLayer, behaviour.OwnerLayer))
                {
                    if (behaviour.Graph.SelectedQuestNode is not null)
                    {
                        nodeViews[node].IsSelected(node == behaviour.Graph.SelectedQuestNode);
                    }

                }

                nodeView.style.display = (DisplayStyle)(behaviour.OwnerLayer.IsVisible ? 0 : 1);
                view.AddElementToLayerContainer(questGraph.OwnerLayer, node.ID, nodeView);
                node.NodeViewPosition = nodeView.GetPosition();
                behaviour.Keys.Add(node);
            }

            foreach (var edge in questGraph.QuestEdges)
            {
                if (!nodeViews.TryGetValue(edge.To, out var n2) || n2 == null) continue;
                foreach (var from in edge.From)
                {
                    if (!nodeViews.TryGetValue(from, out var n1) || n1 == null) continue;
                    
                    var edgeView = CreateEdgeView(questGraph, edge, n1, n2);
                    view.AddElementToLayerContainer(questGraph.OwnerLayer, edge, edgeView);
                    edgeView.layer = n1.layer - 1;
                    behaviour.Keys.Add(edge);
                }
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            // Get behaviours
            if (target is not QuestBehaviour behaviour) return;
            
            foreach (object tile in behaviour.Keys)
            {
                var elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile)?.Where(graphElement => graphElement != null);
                if (elements == null) continue;
                foreach (var graphElement in elements)
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

                var elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile)?.Where(graphElement => graphElement != null);
                if(elements == null) continue;
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
        }

        private LBSQuestEdgeView CreateEdgeView(QuestGraph graph, QuestEdge edge, QuestNodeView n1, QuestNodeView n2)
        {
            foreach (var from in edge.From)
            {
                n1.SetBorder(from);
            }

            n2.SetBorder(edge.To);
            
            return new LBSQuestEdgeView(graph, edge, n1, n2, 4, 4);
        }
        
        private QuestNodeView CreateNodeView(QuestNode node, QuestGraph quest)
        {
            /*  Start Node is now assigned by the user. Right click on a node to make it root */
            if (node.NodeType == QuestNode.ENodeType.Start) { }
                
            var nodeView = new QuestNodeView(node);
            var size = LBSSettings.Instance.general.TileSize * quest.NodeSize;

            nodeView.SetPosition(new Rect(node.Position, size));
            node.NodeViewPosition = nodeView.GetPosition();
            
            return nodeView;
        }
    }
}