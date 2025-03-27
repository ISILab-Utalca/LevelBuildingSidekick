using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestBehaviour))]
    public class QuestGraphDrawer : Drawer
    {
        public QuestGraphDrawer() : base() { }

        //TODO: Falls into a null for a key in a dictionary.
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var behaviour = target as QuestBehaviour;

            var quest = behaviour.Graph;

            var nodeViews = new Dictionary<QuestNode, QuestNodeView>();
            
            foreach (var node in quest.QuestNodes)
            {
                /*  Start Node is now assigned by the user. Right click on a node to make it root */
                if (node.NodeType == NodeType.start)
                {
                    Debug.Log($"Start Node is {node.ID}");
                }

                var nodeView = new QuestNodeView(node);

                var size = LBSSettings.Instance.general.TileSize * quest.NodeSize;

                nodeView.SetPosition(new Rect(node.Position, size));

                nodeViews.Add(node, nodeView);

                if (!(node.Target.Rect.width == 0 || node.Target.Rect.height == 0))
                {
                    var rectView = new DottedAreaFeedback();

                    var rectSize = behaviour.Owner.TileSize * LBSSettings.Instance.general.TileSize;
                    var start = new Vector2(node.Target.Rect.min.x, -node.Target.Rect.min.y) * rectSize;
                    var end = new Vector2(node.Target.Rect.max.x, -node.Target.Rect.max.y) * rectSize;
                    rectView.SetPosition(Rect.zero);
                    rectView.ActualizePositions(start.ToInt(), end.ToInt());
                    rectView.SetColor(Color.blue);

                    view.AddElement(rectView);
                }
            }

            foreach (var edge in quest.QuestEdges)
            {
                Debug.Log("trying to paint edges");
                
                if (!nodeViews.TryGetValue(edge.First, out var n1) || n1 == null) continue;
                if (!nodeViews.TryGetValue(edge.Second, out var n2) || n2 == null) continue;

                n1.SetBorder(edge.First);
                n2.SetBorder(edge.Second);
                
                Debug.Log("edges painted");
                
                var edgeView = new LBSQuestEdgeView(edge, n1, n2, 4, 4);
                view.AddElement(edgeView);
            }

            foreach (var nodeView in nodeViews.Values)
            {
                view.AddElement(nodeView);
            }
        }
    }
}