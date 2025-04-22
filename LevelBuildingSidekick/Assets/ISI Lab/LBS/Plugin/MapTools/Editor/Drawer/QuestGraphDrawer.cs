using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;
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

            GraphElement allElements = null;
            
            foreach (var node in quest.QuestNodes)
            {
                /*  Start Node is now assigned by the user. Right click on a node to make it root */
                if (node.NodeType == NodeType.start) {}
                
                var nodeView = new QuestNodeView(node);
                var size = LBSSettings.Instance.general.TileSize * quest.NodeSize;

                nodeView.SetPosition(new Rect(node.Position, size));

                nodeViews.Add(node, nodeView);

                if (!(node.Target.Rect.width == 0 || node.Target.Rect.height == 0))
                {
                    var rectView = new DottedAreaFeedback(); // TODO make this a DottedAreaUnique for quest

                    var rectSize = behaviour.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
                    var start = new Vector2(node.Target.Rect.min.x, -node.Target.Rect.min.y) * rectSize;
                    var end = new Vector2(node.Target.Rect.max.x, -node.Target.Rect.max.y) * rectSize;
                    rectView.SetPosition(Rect.zero);
                    rectView.ActualizePositions(start.ToInt(), end.ToInt());
                    rectView.SetColor(Color.blue);
                    
                    view.AddElement(quest.OwnerLayer, this, rectView);
                }
            }
            
            foreach (var edge in quest.QuestEdges)
            {
                if (!nodeViews.TryGetValue(edge.First, out var n1) || n1 == null) continue;
                if (!nodeViews.TryGetValue(edge.Second, out var n2) || n2 == null) continue;

                n1.SetBorder(edge.First);
                n2.SetBorder(edge.Second);
                
                var edgeView = new LBSQuestEdgeView(edge, n1, n2, 4, 4);
                view.AddElement(quest.OwnerLayer, this, edgeView);
            }

            foreach (var nodeView in nodeViews.Values)
            {
               view.AddElement(quest.OwnerLayer, this, nodeView);
            }
        }
    }
}