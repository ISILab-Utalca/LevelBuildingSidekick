using ISILab.LBS.VisualElements.Editor;
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
        // for actions, and ors,
        private readonly Dictionary<GraphNode, QuestGraphNodeView> _actionViews = new();
        
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
                behaviour.ActionToSet = string.Empty;
                QuestActionView.Deselect();

            };
            
            _actionViews.Clear();
            
            LoadAllTiles(graph, behaviour, view);
            
            /* Unused drawing system
             
             // TODO: Does this drawer actually needs an update in its visualElements? I don't understand it enough to tell.
            
            view.ClearLayerContainer(behaviour.OwnerLayer, true);
            PaintNewTiles(quest, behaviour, nodeViews, view);
            view.ClearLayerComponentView(behaviour.OwnerLayer, behaviour);
            view.ClearLayerComponentView(behaviour.OwnerLayer, behaviour.Graph);

         
            */
            
            if (!Loaded)
            {
                LoadAllTiles(graph, behaviour, view);
                Loaded = true;
            }
        }

        private void LoadAllTiles(QuestGraph questGraph, QuestBehaviour behaviour, MainView view)
        {
            QuestActionView.Deselect();
            
            foreach (var node in questGraph.GraphNodes)
            {
                if (!_actionViews.TryGetValue(node, out var nodeView) || nodeView == null)
                {
                    nodeView = node switch
                    {
                        // make a quest action visual element
                        QuestNode qn => CreateActionView(qn),
                        // make a branch visual element
                        OrNode or AndNode => CreateBranchView(node),
                        _ => null
                    };

                    _actionViews[node] = nodeView;
                }
                
                if (Equals(LBSMainWindow.Instance._selectedLayer, behaviour.OwnerLayer))
                {
                    if (behaviour.Graph.SelectedQuestNode is not null)
                    {
                        if (_actionViews[node] is QuestActionView questActionView)
                        {
                            // to find the highlighted element is within the active quest layer
                            questActionView.IsSelected(node == behaviour.Graph.SelectedQuestNode);
                        }
                        
                    }

                }
                
                // if not successfully created
                if(nodeView is null) continue;
                
                nodeView.style.display = (DisplayStyle)(behaviour.OwnerLayer.IsVisible ? 0 : 1);
               // view.AddElementToLayerContainer(questGraph.OwnerLayer, node, nodeView);
                behaviour.Keys.Add(node);
            }

            foreach (var edge in questGraph.QuestEdges)
            {
                if (!_actionViews.TryGetValue(edge.To, out var n2) || n2 == null) continue;
                foreach (var from in edge.From)
                {
                    if (!_actionViews.TryGetValue(from, out var n1) || n1 == null) continue;
                    
                    var edgeView = CreateEdgeView(questGraph, edge, n1, n2);
                    view.AddElementToLayerContainer(questGraph.OwnerLayer, edge, edgeView);
                    edgeView.layer = n1.layer + 1;
                    behaviour.Keys.Add(edge);
                }
            }

            foreach (var entry in _actionViews)
            {
                view.AddElementToLayerContainer(questGraph.OwnerLayer, entry.Key, entry.Value);
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

        private LBSQuestEdgeView CreateEdgeView(QuestGraph graph, QuestEdge edge, QuestGraphNodeView n1, QuestGraphNodeView n2)
        {
            foreach (var from in edge.From)
            {
                n1.DisplayGrammarState(from);
            }

            n2.DisplayGrammarState(edge.To);
            
            return new LBSQuestEdgeView(graph, edge, n1, n2, 4, 4);
        }
        
        private QuestActionView CreateActionView(QuestNode node)
        {
            var nodeView = new QuestActionView(node);
            return nodeView;
        }
        

        private QuestBranchView CreateBranchView(GraphNode node)
        {
            var nodeView = new QuestBranchView(node);
            return nodeView;
        }
    }
}