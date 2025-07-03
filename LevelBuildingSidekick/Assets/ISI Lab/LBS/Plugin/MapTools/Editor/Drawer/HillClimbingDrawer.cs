using System;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Modules;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(HillClimbingAssistant))]
    public class HillClimbingDrawer : Drawer
    {
        private readonly Vector2 _nodeSize = new(100, 100);

        private readonly Dictionary<Zone, LBSNodeView> _nodeRefs = new();
        private readonly HashSet<object> _keyRefs = new();

        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // Set target Assistant
            var assistant = target as HillClimbingAssistant;
            if (assistant == null) return;

            // Get modules
            var consts = assistant.ConstrainsZonesMod.Constraints;

            // Get new tiles
            assistant.ReloadPrevData();
            view.ClearLayerView(assistant.OwnerLayer);
            
            PaintNewTiles(assistant, view, consts, teselationSize);
            UpdateLoadedTiles(view, assistant, consts, teselationSize);
        }

        private void PaintNewTiles(HillClimbingAssistant assistant, MainView view,
            List<ConstraintPair> consts, Vector2 teselationSize)
        {
            var newTiles = assistant.RetrieveNewTiles();
            
            // Draw new Nodes
            foreach (var o in newTiles)
            {
                if (o is not Zone zone) continue;
                
                var nView = CreateNode(assistant, zone);
                view.AddElement(assistant.OwnerLayer, zone, nView);
                
                if (!_nodeRefs.TryAdd(zone, nView))
                {
                    _nodeRefs[zone] = nView;
                }
                _keyRefs.Add(zone);

                // Constrains
                foreach (var pair in consts)
                {
                    if (!pair.Zone.Equals(zone)) continue;
                    
                    
                    var vws = CreateFeedBackAreas(nView, pair, teselationSize);
                    var ve = new Empty();
                    foreach (var v in vws)
                    {
                        ve.Add(v);
                    }

                    view.AddElement(assistant.OwnerLayer, pair, ve);
                    assistant.SaveConstraintKey(zone,pair);
                    _keyRefs.Add(pair);
                    break;
                }
            }
            
            // Draw new Edges
            foreach (var o in newTiles)
            {
                if (o is not ZoneEdge edge) continue;
                
                // Get view nodes
                var n1 = _nodeRefs[edge.First];
                var n2 = _nodeRefs[edge.Second];

                // Create EdgeView
                var eView = new LBSEdgeView(edge, n1, n2, 4, 4);
                
                view.AddElement(assistant.OwnerLayer, edge, eView);
                _keyRefs.Add(edge);
            }
        }
        
        private void UpdateLoadedTiles(MainView view, HillClimbingAssistant assistant, List<ConstraintPair> consts, Vector2 teselationSize)
        {
            // Update visuals
            foreach (var key in _keyRefs.ToList())
            {
                var elements = view.GetElements(assistant.OwnerLayer, key);
                
                // Remove lost references
                if (elements == null)
                {
                    _keyRefs.Remove(key);
                    continue;
                }

                // Update visuals
                foreach (var element in elements.Where(element => element != null))
                {
                    switch (key)
                    {
                        case Zone zone:
                            var node = element as LBSNodeView;
                            if (node == null) break;
                            UpdateNode(ref node, assistant, zone);
                            break;
                        
                        case ConstraintPair keyPair:
                            var feedback = element as Empty;
                            if (feedback == null) break;

                            foreach (var pair in consts.Where(pair => keyPair.Zone.Equals(pair.Zone)))
                            {
                                UpdateFeedbackArea(ref feedback, pair, teselationSize, assistant.visibleConstraints);
                                break;
                            }
                            break;
                        
                        case ZoneEdge zEdge:
                            var edgeView = element as LBSEdgeView;
                            UpdateEdge(ref edgeView, _nodeRefs[zEdge.First], _nodeRefs[zEdge.Second]);
                            break;
                        
                        default:
                            Debug.LogWarning("HillClimbingDrawer error: _keyRefs contains unsupported element type " + key);
                            break;
                    }
                    element.layer = assistant.OwnerLayer.index;
                }
            }
        }

        public override void ShowVisuals(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not HillClimbingAssistant assistant) return;
            
            foreach (object tile in _keyRefs)
            {
                foreach (var graphElement in view.GetElements(assistant.OwnerLayer, tile).Where(graphElement => graphElement != null))
                {
                    graphElement.style.display = DisplayStyle.Flex;
                }
            }
        }
        public override void HideVisuals(object target, MainView view, Vector2 teselationSize)
        {
            if (target is not HillClimbingAssistant assistant) return;
            
            foreach (object tile in _keyRefs)
            {
                if (tile == null) continue;

                var elements = view.GetElements(assistant.OwnerLayer, tile);
                foreach (var graphElement in elements)
                {
                    graphElement.style.display = DisplayStyle.None;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="center_old"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private List<DottedAreaFeedback> CreateFeedBackAreas(LBSNodeView nodeView, ConstraintPair pair, Vector2 size)
        {
            var settings = LBSSettings.Instance;
            var tileSize = settings.general.TileSize;

            List<DottedAreaFeedback> cViews = new();

            var constr = pair.Constraint;

            // Get points from first dotted area
            var maxV1 = new Vector2(-constr.maxWidth / 2f, -constr.maxHeight / 2f);
            var maxV2 = new Vector2(constr.maxWidth / 2f, constr.maxHeight / 2f);

            // Create first dotted area
            var c1 = new DottedAreaFeedback();

            // Get center position
            var center = nodeView.GetPosition().center;

            // Set values to first doted area
            c1.SetPosition(new Rect(center, new Vector2(10, 10)));
            c1.ActualizePositions((maxV1 * size * tileSize).ToInt(), (maxV2 * size * tileSize).ToInt());
            c1.SetColor(Color.red);

            // Get points from second dotted area
            var minV1 = new Vector2(-constr.minWidth / 2f, -constr.minHeight / 2f);
            var minV2 = new Vector2(constr.minWidth / 2f, constr.minHeight / 2f);

            // Create second dotted area
            var c2 = new DottedAreaFeedback();

            // Set value to second dotted area
            c2.SetPosition(new Rect(center, new Vector2(10, 10)));
            c2.ActualizePositions((minV1 * size * tileSize).ToInt(), (minV2 * size * tileSize).ToInt());
            c2.SetColor(Color.blue);

            // add constraint to list
            cViews.Add(c1);
            cViews.Add(c2);

            return cViews;
        }

        private void UpdateFeedbackArea(ref Empty feedbackArea, ConstraintPair pair, Vector2 size, bool visible)
        {
            if (feedbackArea == null) return;
            
            feedbackArea.visible = visible;
            if (!visible) return;
            
            var settings = LBSSettings.Instance;
            var tileSize = settings.general.TileSize;
            var constr = pair.Constraint;

            var areas = feedbackArea.Children().ToArray();
            var a1 = (DottedAreaFeedback) areas[0];
            var a2 = (DottedAreaFeedback) areas[1];
            _nodeRefs.TryGetValue(pair.Zone, out var nodeView);
            
            if(a1 == null || a2 == null || nodeView == null) return;
            var center = nodeView.GetPosition().center;
            
            // -------- First dotted area --------
            // Get points 
            var maxV1 = new Vector2(-constr.maxWidth / 2f, -constr.maxHeight / 2f);
            var maxV2 = new Vector2(constr.maxWidth / 2f, constr.maxHeight / 2f);
            
            // Set values
            a1.SetPosition(new Rect(center, new Vector2(10, 10)));
            a1.ActualizePositions((maxV1 * size * tileSize).ToInt(), (maxV2 * size * tileSize).ToInt());
            a1.SetColor(Color.red);
            
            // -------- Second dotted area --------
            // Get points from second dotted area
            var minV1 = new Vector2(-constr.minWidth / 2f, -constr.minHeight / 2f);
            var minV2 = new Vector2(constr.minWidth / 2f, constr.minHeight / 2f);

            // Set value to second dotted area
            a2.SetPosition(new Rect(center, new Vector2(10, 10)));
            a2.ActualizePositions((minV1 * size * tileSize).ToInt(), (minV2 * size * tileSize).ToInt());
            a2.SetColor(Color.blue);
        }

        private LBSNodeView CreateNode(HillClimbingAssistant assistant, Zone zone)
        {
            // Create node view
            var nView = new LBSNodeView();

            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();

            // Set position
            var pos = new Vector2(
                bound.center.x * _nodeSize.x - _nodeSize.x / 2f,
                -(bound.center.y * _nodeSize.y - _nodeSize.y / 2f));

            // Set view values
            nView.SetPosition(new Rect(pos, _nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            return nView;
        }

        private void UpdateNode(ref LBSNodeView nView, HillClimbingAssistant assistant, Zone zone)
        {
            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();

            // Set position
            var pos = new Vector2(
                bound.center.x * _nodeSize.x - _nodeSize.x / 2f,
                -(bound.center.y * _nodeSize.y - _nodeSize.y / 2f));

            // Set view values
            nView.SetPosition(new Rect(pos, _nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);
        }

        private void UpdateEdge(ref LBSEdgeView edgeView, LBSNodeView node1, LBSNodeView node2)
        {
            var sPos1 = new Vector2Int((int)node1.GetPosition().center.x, (int)node1.GetPosition().center.y);
            var sPos2 = new Vector2Int((int)node2.GetPosition().center.x, (int)node2.GetPosition().center.y);
            edgeView.ActualizePositions(sPos1, sPos2);
        }
    }
}