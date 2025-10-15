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
            Painter2D painter = ctx.painter2D;
            // Custom 2D drawing — or use ctx.DrawMesh for 3D-style rendering
            
            Rect rect = contentRect;
            // Use a mesh with a material that uses your shader
        }
    }
}

