using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveQuestConnection : LBSManipulator
    {
        private QuestGraph _questGraph;
        protected override string IconGuid => "b534f3f3d94bf1349babd81aa035d583";

        public RemoveQuestConnection()
        {
            Name = "Remove Quest Connection";
            Description = "Click a connection line between nodes to remove it.";
        }
        
        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _questGraph = layer.GetModule<QuestGraph>();
        }
        
        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            _questGraph.RemoveEdgeByPosition(endPosition, 50);
            OnManipulationEnd.Invoke();
        }
    }
}