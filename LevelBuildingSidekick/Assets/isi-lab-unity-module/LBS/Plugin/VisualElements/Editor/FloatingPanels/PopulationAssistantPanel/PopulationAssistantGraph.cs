using ISILab.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationAssistantGraph : VisualElement
    {
        private float[] _axes;
        private Color?[] _axesColor;
        private float _strokeSize;
        
        private Vector2[] _fullCorners;
        private Vector2[] _partialCorners;

        private Color _mainColor = new Color32(255,166,0,255);
        public Color MainColor
        {
            set{_mainColor = value;}
            get{return _mainColor;}
        }
        
        private Color _secondaryColor = new Color32(151,71,255,255);
        public Color SecondaryColor
        {
            set{_secondaryColor = value;}
            get{return _secondaryColor;}
        }

        //Builder
        public PopulationAssistantGraph(float[] axes, float strokeSize)
        {
            _axes = axes;
            _strokeSize = strokeSize;

            _axesColor = new Color?[axes.Length];
            for (int i = 0; i < axes.Length; i++)
            {
                _axesColor[i] = null;
            }
            
            generateVisualContent += OnGenerateVisualContent;
            RecalculateCorners();
        }

        //Creates vectors for each axis to form polygons, should be called when _axes is modified (not automatically)
        public void RecalculateCorners(float scale = 60.0f)
        {
            float localScale = scale;
            _fullCorners = new Vector2[_axes.Length];
            _partialCorners = new Vector2[_axes.Length];
            
            for(int i = 0; i < _axes.Length; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(360f / _axes.Length * i, Vector3.forward);
                
                //Get full polygon's corners' position
                Vector2 vAux = rotation * Vector2.down * localScale;
                _fullCorners[i] = vAux + vAux/4;
                
                //Get partial polygon's corners' position
                _partialCorners[i] = vAux * _axes[i] + vAux/4;
            }
        }

        //Automatically called everytime it needs to repaint
        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var paint2D = mgc.painter2D;
            //Vector2 offset = Vector2.right * worldBound.width * 0.5f;
            Vector2 offset = new Vector2(worldBound.width * 0.5f, worldBound.yMax*0.125f);
            float scale = Mathf.Min(worldBound.yMax / 150f + 0.25f, worldBound.width / 180f + 0.25f);
            
            //REFERENCE POLYGON
            Color dColor = DarkenColor(_secondaryColor, 0.39f);
            //Polygonal segments
            DrawEmptyPolygon(paint2D, _fullCorners, _secondaryColor, scale, offset, _strokeSize * scale);
            DrawEmptyPolygon(paint2D, _fullCorners, dColor,scale * 4/5, offset, _strokeSize * 0.5f * scale);
            DrawEmptyPolygon(paint2D, _fullCorners, dColor,scale * 3/5, offset, _strokeSize * 0.5f * scale);
            DrawEmptyPolygon(paint2D, _fullCorners, dColor,scale * 2/5, offset, _strokeSize * 0.5f * scale);
            DrawEmptyPolygon(paint2D, _fullCorners, dColor,scale * 1/5, offset, _strokeSize * 0.5f * scale);
            //Axes lines
            paint2D.lineWidth = _strokeSize * 0.5f;
            foreach (Vector2 corner in _fullCorners)
            {
                paint2D.BeginPath();
                paint2D.MoveTo(corner * scale + offset);
                paint2D.LineTo(corner * scale * 0.2f + offset);
                paint2D.ClosePath();
                paint2D.Stroke();
            }
            //Circles to differentiate axes
            for(int i = 0; i < _axes.Length; i++)
            {
                Color c = _axesColor[i] == null ? _secondaryColor : _axesColor[i].Value;
                dColor = DarkenColor(c, 0.39f);
                
                paint2D.DrawCircle(_fullCorners[i] * scale + offset, 5 * scale, dColor);
                paint2D.DrawCircle(_fullCorners[i] * scale + offset, 4 * scale, c);
            }
            
            //PARTIAL POLYGON
            dColor = DarkenColor(_mainColor, 0.1569f);
            //Outline and area
            DrawEmptyPolygon(paint2D, _partialCorners, _mainColor, scale, offset, _strokeSize * scale);
            DrawFillPolygon(paint2D, _partialCorners, dColor, scale, offset);
            //Circles to point position in axis
            dColor = DarkenColor(_mainColor, 0.39f);
            for(int i = 0; i < _axes.Length; i++)
            {
                paint2D.DrawCircle(_partialCorners[i] * scale + offset, 5 * scale, dColor);
                paint2D.DrawCircle(_partialCorners[i] * scale + offset, 4 * scale, _mainColor);
            }
        }

        public bool SetAxisValue(float value, int index)
        {
            if (index >= _axes.Length || index < 0)
            {
                return false;
            }
            
            _axes[index] = value;
            return true;
        }
        public bool SetAxisColor(Color color, int index)
        {
            if (index >= _axes.Length || index < 0)
            {
                return false;
            }
            
            _axesColor[index] = color;
            return true;
        }
        
        #region UTILITIES
        private void DrawFillPolygon(Painter2D paint2D, Vector2[] polygonCorners, Color color, float scale, Vector2 offset)
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
        private void DrawEmptyPolygon(Painter2D paint2D, Vector2[] polygonCorners, Color color, float scale, Vector2 offset, float strokeSize)
        {
            //Draw partial polygon
            paint2D.strokeColor = color;
            paint2D.lineWidth = strokeSize;
            
            paint2D.BeginPath();
            paint2D.MoveTo(polygonCorners[0] * scale + offset);
            for(int i = 1; i < polygonCorners.Length; i++)
            {
                paint2D.LineTo(polygonCorners[i] * scale + offset);
            }
            paint2D.ClosePath();
            paint2D.Stroke();
        }
        private Color DarkenColor(Color color, float transparency)
        {
            return new Color(color.r, color.g, color.b, transparency);
        }
        #endregion
    }
}
