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
        private readonly Vector2 nodeSize = new(100, 100);

        private readonly Dictionary<Zone, LBSNodeView> _nodeRefs = new();
        private readonly List<object> _keyRefs = new();

        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            // clear preview view reference
            _nodeRefs.Clear();

            // Set target Assistant
            var assistant = target as HillClimbingAssistant;

            // Get modules
            var consts = assistant.ConstrainsZonesMod.Constraints;

            List<(object, Empty)> cViews = new();

            assistant.ReloadPrevData();
            var newTiles = assistant.RetrieveNewTiles();

            // New Nodes
            foreach (Zone zone in newTiles)
            {
                if(zone == null) continue;
                
                var nView = CreateNode(assistant, zone);
                view.AddElement(assistant.OwnerLayer, zone, nView);

                _nodeRefs.Add(zone, nView);
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
                    _keyRefs.Add(pair);
                    break;
                }
            }
            
            // New Edges
            foreach (ZoneEdge edge in newTiles)
            {
                if (edge == null) continue;
                
                // Get view nodes
                var n1 = _nodeRefs[edge.First];
                var n2 = _nodeRefs[edge.Second];

                // Create EdgeView
                var eView = new LBSEdgeView(edge, n1, n2, 4, 4);
                
                view.AddElement(assistant.OwnerLayer, edge, eView);
                _keyRefs.Add(edge);
            }
            
            // Update visuals
            foreach (var key in _keyRefs)
            {
                var elements = view.GetElements(assistant.OwnerLayer, key);

                foreach (var element in elements)
                {
                    switch (key)
                    {
                        case Zone zone:
                            var node = element as LBSNodeView;
                            if (node == null) break;
                            
                            UpdateNode(node, assistant, zone);
                            break;
                        
                        case ConstraintPair pair:
                            var feedback = element as Empty;
                            if (feedback == null) break;
                            
                            //UpdateFeedbackArea(feedback, assistant, zone);
                            break;
                        /*
                        case Zonae zone:
                            var em = element as Empty;
                            UpdateNode(anode, assistant, zone);
                            break;//*/
                    }
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

        private Empty UpdateFeedbackArea(Empty feedbackArea, HillClimbingAssistant assistant, ConstraintPair pair, Vector2 size)
        {
            if (feedbackArea == null) return null;
            
            var settings = LBSSettings.Instance;
            var tileSize = settings.general.TileSize;
            var constr = pair.Constraint;
/*

            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();
            
            // Set position
            var pos = new Vector2(
                bound.center.x * nodeSize.x - nodeSize.x / 2f,
                -(bound.center.y * nodeSize.y - nodeSize.y / 2f));

            // Set view values
            nView.SetPosition(new Rect(pos, nodeSize));
            // Get center position
            var center = nodeView.GetPosition().center;//*/

            var areas = feedbackArea.Children().ToArray();
            var a1 = (DottedAreaFeedback) areas[0];
            var a2 = (DottedAreaFeedback) areas[1];
            
            if(a1 == null || a2 == null) return null;
            
            // First dotted area
            // Get points 
            var maxV1 = new Vector2(-constr.maxWidth / 2f, -constr.maxHeight / 2f);
            var maxV2 = new Vector2(constr.maxWidth / 2f, constr.maxHeight / 2f);
            
            // Set values
           // a1.SetPosition(new Rect(center, new Vector2(10, 10)));
            a1.ActualizePositions((maxV1 * size * tileSize).ToInt(), (maxV2 * size * tileSize).ToInt());
            a1.SetColor(Color.red);
            
            
            // Second dotted area
            // Get points from second dotted area
            var minV1 = new Vector2(-constr.minWidth / 2f, -constr.minHeight / 2f);
            var minV2 = new Vector2(constr.minWidth / 2f, constr.minHeight / 2f);

            // Set value to second dotted area
           // a2.SetPosition(new Rect(center, new Vector2(10, 10)));
            a2.ActualizePositions((minV1 * size * tileSize).ToInt(), (minV2 * size * tileSize).ToInt());
            a2.SetColor(Color.blue);

            return feedbackArea;
        }

        private LBSNodeView CreateNode(HillClimbingAssistant assistant, Zone zone)
        {
            // Create node view
            var nView = new LBSNodeView();

            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();

            // Set position
            var pos = new Vector2(
                bound.center.x * nodeSize.x - nodeSize.x / 2f,
                -(bound.center.y * nodeSize.y - nodeSize.y / 2f));

            // Set view values
            nView.SetPosition(new Rect(pos, nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            return nView;
        }

        private LBSNodeView UpdateNode(LBSNodeView nView, HillClimbingAssistant assistant, Zone zone)
        {
            var tiles = assistant.GetTiles(zone);
            var bound = tiles.GetBounds();

            // Set position
            var pos = new Vector2(
                bound.center.x * nodeSize.x - nodeSize.x / 2f,
                -(bound.center.y * nodeSize.y - nodeSize.y / 2f));

            // Set view values
            nView.SetPosition(new Rect(pos, nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            return nView;
        }

        private LBSEdgeView UpdateEdge(LBSEdgeView edgeView, LBSNodeView node1, LBSNodeView node2)
        {
            var sPos1 = new Vector2Int((int)node1.GetPosition().center.x, (int)node1.GetPosition().center.y);
            var sPos2 = new Vector2Int((int)node2.GetPosition().center.x, (int)node2.GetPosition().center.y);
            edgeView.ActualizePositions(sPos1, sPos2);

            return edgeView;
        }
    }
}