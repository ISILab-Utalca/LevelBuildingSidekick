using ISILab.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationParamsGraph : VisualElement
    {
        private float[] axes;
        private Color mainColor;
        private float strokeSize;
        
        Vector2[] fullCorners;
        Vector2[] partialCorners;

        public PopulationParamsGraph(float[] axes, Color color, float strokeSize)
        {
            this.axes = axes;
            this.mainColor = color;
            this.strokeSize = strokeSize;

            generateVisualContent += OnGenerateVisualContent;
            RecalculateCorners();
        }

        void RecalculateCorners()
        {
            float scale = 60;
            fullCorners = new Vector2[axes.Length];
            partialCorners = new Vector2[axes.Length];
            
            for(int i = 0; i < axes.Length; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(360f / axes.Length * i, Vector3.forward);
                
                //Get full polygon's corners' position
                Vector2 vAux = rotation * Vector2.down * scale;
                fullCorners[i] = vAux;
                
                //Get partial polygon's corners' position
                vAux *= axes[i];
                partialCorners[i] = vAux;
            }
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var paint2D = mgc.painter2D;
            Vector2 offset = Vector2.right * worldBound.width * 0.5f;
            float scale = worldBound.width / 180f + 0.5f;
            
            //Full polygon
            DrawFillPolygon(paint2D, fullCorners,Color.grey, scale, offset);
            //Partial polygon
            DrawEmptyPolygon(paint2D, partialCorners, mainColor, scale, offset);
            
            //Circles to point position in axis
            for(int i = 0; i < axes.Length; i++)
            {
                paint2D.DrawCircle(partialCorners[i] * scale + offset, 5 * scale, mainColor);
            }
        }

        void DrawFillPolygon(Painter2D paint2D, Vector2[] polygonCorners, Color color, float scale, Vector2 offset)
        {
            //Draw full polygon
            paint2D.fillColor = color;
            paint2D.BeginPath();
            paint2D.MoveTo(polygonCorners[0] * scale + offset);
            for(int i = 1; i < polygonCorners.Length; i++)
            {
                paint2D.LineTo(polygonCorners[i] * scale + offset);
            }
            paint2D.ClosePath();
            paint2D.Fill();
        }
        void DrawEmptyPolygon(Painter2D paint2D, Vector2[] polygonCorners, Color color, float scale, Vector2 offset)
        {
            //Draw partial polygon
            paint2D.strokeColor = color;
            paint2D.lineWidth = strokeSize * scale;
            paint2D.BeginPath();
            paint2D.MoveTo(polygonCorners[0] * scale + offset);
            for(int i = 1; i < polygonCorners.Length; i++)
            {
                paint2D.LineTo(polygonCorners[i] * scale + offset);
            }
            paint2D.ClosePath();
            paint2D.Stroke();
        }
    }
}
