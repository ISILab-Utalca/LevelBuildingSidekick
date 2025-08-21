using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveQuestNode : LBSManipulator
    {
        private QuestGraph _questGraph;

        protected override string IconGuid => "ce08b36a396edbf4394f7a4e641f253d";

        public RemoveQuestNode()
        {
            Name = "Remove Quest Node";
            Description = "Click on a quest node to remove it.";
        }
        
        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _questGraph = layer.GetModule<QuestGraph>();
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = _questGraph.GetNodeAtPosition<GraphNode>(endPosition);
            if (node == null) return;
            _questGraph.RemoveQuestNode(node);
            OnManipulationEnd?.Invoke();
        }
    }
}
