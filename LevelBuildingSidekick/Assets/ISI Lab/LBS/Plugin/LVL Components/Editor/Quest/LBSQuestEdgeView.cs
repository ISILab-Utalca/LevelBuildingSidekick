using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Modules;

namespace ISILab.LBS.VisualElements
{
    // Class that draws the quest edges.
    public class LBSQuestEdgeView : GraphElement
    {
        private Vector2Int pos1, pos2;

        private QuestEdge data;

        public LBSQuestEdgeView(QuestEdge data, QuestNodeView node1, QuestNodeView node2, int l, int stroke)
        {
            // Set Data
            this.data = data;

            // Set first node
            node1.OnMoving += (rect) =>
            {
                SetPosition(new Rect(pos1, new Vector2(10, 10)));
                ActualizePositions(rect.center.ToInt(), pos2);
            };

            // Set second node
            node2.OnMoving += (rect) =>
            {
                ActualizePositions(pos1, rect.center.ToInt());
            };

            var sPos1 = new Vector2Int((int)node1.GetPosition().center.x, (int)node1.GetPosition().center.y);
            var sPos2 = new Vector2Int((int)node2.GetPosition().center.x, (int)node2.GetPosition().center.y);
            ActualizePositions(sPos1, sPos2);

            SetPosition(new Rect(pos1, new Vector2(10, 10)));
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            // line
            painter.DrawDottedLine(Vector2.zero, pos2 - pos1, Color.white, 3f, 5f);
               
            // arrow
            var pos = pos2 - pos1;
            Vector2 direction = new Vector2(pos.x, pos.y).normalized;
            Vector2 midpoint = (pos2 - pos1) / 2;
            
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            
            float arrowLength = 25f;
            float arrowWidth = 15f; 

            // Define the arrow points relative to the midpoint
            List<Vector2> arrowPoints = new List<Vector2>()
            {
                
                midpoint - direction * arrowLength + perpendicular * arrowWidth, // Left base
                midpoint,                                                       // Arrow tip
                midpoint - direction * arrowLength - perpendicular * arrowWidth  // Right base
            };
            painter.DrawPolygon(arrowPoints, Color.white, Color.white, 0f);
            
        }

        private void ActualizePositions(Vector2Int pos1, Vector2Int pos2)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
            MarkDirtyRepaint();
        }
    }
}