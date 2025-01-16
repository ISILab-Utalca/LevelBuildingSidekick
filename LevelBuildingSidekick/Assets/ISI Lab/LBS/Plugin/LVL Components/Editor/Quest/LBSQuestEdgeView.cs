
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

        private int pos_offset = 16;

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

            var sPos1 = new Vector2Int((int)node1.GetPosition().xMax - pos_offset, (int)node1.GetPosition().center.y);
            var sPos2 = new Vector2Int((int)node2.GetPosition().xMin + pos_offset, (int)node2.GetPosition().center.y);
            ActualizePositions(sPos1, sPos2);

            SetPosition(new Rect(pos1, new Vector2(10, 10)));
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            // line
            painter.DrawDottedLine(Vector2.zero , pos2 - pos1, Color.white, 3f, 5f);
            

            // arrow
            Vector2Int pos = pos2 - pos1;
            Vector2 midpoint = (pos2 - pos1) / 2;
            Vector2 direction = new Vector2(pos.x, pos.y).normalized;
            
            // points
            //painter.DrawCircle(Vector2.zero + direction * 24f, 2f, Color.white);
            //painter.DrawCircle(pos - direction * 24f, 2f, Color.white);

            
            painter.DrawEquilateralArrow(midpoint, direction, 15f, Color.white);
            
        }

        private void ActualizePositions(Vector2Int pos1, Vector2Int pos2)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
            MarkDirtyRepaint();
        }
    }
}