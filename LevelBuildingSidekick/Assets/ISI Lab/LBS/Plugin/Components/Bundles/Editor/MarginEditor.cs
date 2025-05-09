using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.VisualElements;

namespace LBS.Bundles.Editor
{
    [CustomEditor(typeof(MicroGenTool.Margin))]
    public class MarginEditor : UnityEditor.Editor
    {
        private MarginGraph _margin;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customInspector = base.CreateInspectorGUI();
            
            _margin = new MarginGraph();
            customInspector.Add(_margin);
            return customInspector;
        }
    }
}
