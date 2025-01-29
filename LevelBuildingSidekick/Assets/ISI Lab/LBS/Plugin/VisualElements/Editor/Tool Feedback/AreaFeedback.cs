using ISILab.Extensions;
using ISILab.LBS.VisualElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class AreaFeedback : Feedback
    {
        
        #region Static fields
        private static float borderThickness = 1f;
        private static float fillOpacity = 0.33f;
        private static readonly Color Colordefault = new (0.5f, 0.7f, 0.98f, 1);
        private static readonly Color ColorDelete = Color.red;


        #endregion
        
        public AreaFeedback(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }

        public AreaFeedback() : base()
        {
            EndOffset = TeselationSize;
        }

        protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            var ColorFill = delete ?  ColorDelete : Colordefault;
            ColorFill.a = fillOpacity;
            var fillColor = currentColor * ColorFill;

            var points = new List<Vector2>()
        {
            new Vector2(startPosition.x, startPosition.y),
            new Vector2(startPosition.x, endPosition.y),
            new Vector2(endPosition.x, endPosition.y),
            new Vector2(endPosition.x, startPosition.y),
        };
            painter.DrawPolygon(points, fillColor, ColorFill, borderThickness, true);
        }

        public override void ActualizePositions(Vector2Int p1, Vector2Int p2)
        {
            startPosition = new Vector2Int(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
            endPosition = new Vector2Int(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));

            if (fixToTeselation)
            {
                startPosition = CalcFixTeselation(startPosition);
                endPosition = CalcFixTeselation(endPosition);

                startPosition = startPosition * TeselationSize + StartOffset;
                endPosition = endPosition * TeselationSize + TeselationSize;
            }

            MarkDirtyRepaint();
        }
    }
}