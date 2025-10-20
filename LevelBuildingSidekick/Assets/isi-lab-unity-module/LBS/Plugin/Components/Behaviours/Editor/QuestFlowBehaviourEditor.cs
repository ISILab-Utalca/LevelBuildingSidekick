using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine.UIElements;
// ReSharper disable All

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestFlowBehaviour", typeof(QuestFlowBehaviour))]
    public class QuestFlowBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private AddGraphNode addNode;
        private RemoveGraphNode removeNode;
        private ConnectQuestNodes connectNodes;
        private RemoveQuestConnection removeConnection;

        #region VIEW FIELDS
        private QuestHistoryPanel questHistoryPanel;
        private DropdownField grammarDropdown;
        private VisualElement actionPallete;
        private QuestFlowBehaviour behaviour;
        #endregion
        
        #endregion
        
        #region CONSTRUCTORS
        public QuestFlowBehaviourEditor(object target) : base(target)
        {
            Clear();
            SetInfo(target);
        }
        #endregion
        
        #region METHODS
        public override void SetInfo(object paramTarget)
        {
            Clear();
            behaviour = paramTarget as QuestFlowBehaviour;
            CreateVisualElement();
            if (behaviour == null) return;
            questHistoryPanel?.SetInfo(behaviour);
        }
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            questHistoryPanel = new QuestHistoryPanel();
            Add(questHistoryPanel);
            return this;
        }

        public void SetTools(ToolKit toolkit)
        { 
            // No tools 
        }
        #endregion
    }
}