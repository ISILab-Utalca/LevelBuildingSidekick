using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class PopulationParamsGraph : VisualElement
    {
        [SerializeField] private float[] axes;
        [SerializeField] private Color color;
        [SerializeField] private float size;

        public PopulationParamsGraph(float[] axes, Color color, float size)
        {
            this.axes = axes;
            this.color = color;
            this.size = size;
            
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            Debug.Log(this.transform.position);
            var paint2D = mgc.painter2D;
            
            paint2D.fillColor = color;
            paint2D.BeginPath();
            paint2D.Arc(this.transform.position, 50.0f, 0.0f, 360.0f);
            paint2D.Fill();

        }
    }
}
