using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    public class ConnectedLine : Feedback
    {
        public ConnectedLine(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }
        public ConnectedLine() : base()
        {
            this.StartOffset = (TeselationSize.Multiply(0.5f));
            this.EndOffset = (TeselationSize.Multiply(0.5f));
        }

        protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            var line = new List<Vector2>() { startPosition, endPosition };
            painter.DrawPolygon(line, new Color(0, 0, 0, 0), currentColor, 4, false);
            painter.DrawCircle(startPosition, 16, currentColor);
            painter.DrawCircle(endPosition, 16, currentColor);
        }

        public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
        {
            startPosition = p1;
            endPosition = p2;

            if (fixToTeselation)
            {
                startPosition = CalcFixTeselation(startPosition);
                endPosition = CalcFixTeselation(endPosition);


                startPosition = (startPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
                endPosition = (endPosition * TeselationSize) + TeselationSize.Multiply(0.5f);
            }

            this.MarkDirtyRepaint();
        }
    }

    public class ConnectedConrnerLine : Feedback
    {
        public bool LeftSide = false;

        public Vector2Int cornerPoint;

        public bool useVertices = false;

        public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
        {
            startPosition = p1;
            endPosition = p2;

            cornerPoint = LeftSide ?
                    new Vector2Int(startPosition.x, endPosition.y) :
                    new Vector2Int(endPosition.x, startPosition.y);

            if (fixToTeselation)
            {
                Vector2Int offsetValue = TeselationSize.Multiply(0.5f);
                offsetValue = new Vector2Int(offsetValue.x, -offsetValue.y);
                Vector2Int offset = useVertices ? TeselationSize.Multiply(0.5f) : Vector2Int.zero;
                startPosition = CalcFixTeselation(startPosition + offset);
                endPosition = CalcFixTeselation(endPosition + offset);
                cornerPoint = CalcFixTeselation(cornerPoint + offset);
                                                                    
                startPosition = (startPosition * TeselationSize) + TeselationSize.Multiply(0.5f) - offset;
                endPosition = (endPosition * TeselationSize) + TeselationSize.Multiply(0.5f) - offset;
                cornerPoint = (cornerPoint * TeselationSize) + TeselationSize.Multiply(0.5f) - offset;
            }

            this.MarkDirtyRepaint();
        }

        protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            var line = new List<Vector2>() { startPosition, cornerPoint, endPosition };
            painter.DrawPolygon(line, new Color(0, 0, 0, 0), currentColor, 4, false);
            painter.DrawCircle(startPosition, 16, currentColor);
            painter.DrawCircle(endPosition, 16, currentColor);
        }
    }
}