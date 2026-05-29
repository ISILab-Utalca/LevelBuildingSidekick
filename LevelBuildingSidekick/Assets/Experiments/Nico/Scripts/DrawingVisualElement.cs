using UnityEngine;
using Unity.UIElements;
using UnityEngine.UIElements;

[UxmlElement]
public partial class DrawingVisualElement: VisualElement
{
    
    
    public DrawingVisualElement(): base()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    public void OnGenerateVisualContent (MeshGenerationContext ctx)
    {
       Vector2 u_size = new Vector2((float)this.style.height, this.style.width);
    }
}
