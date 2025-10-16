using ISILab.Extensions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents
{
    public class LBSShaderedVisualElement : VisualElement
    {
        public Material material;
        public NativeSlice<Vertex> mesh;

        public LBSShaderedVisualElement(Material mat)
        {
            if (mat != null)
            {
                material = mat;
            }
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
        }
    }
}

