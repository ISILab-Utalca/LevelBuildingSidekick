using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor.Search;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
// ReSharper disable All

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestFlowBehaviour", typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourEditor : LBSCustomEditor, IToolProvider
    {

        /// <summary>
        /// Displays the action string
        /// </summary>
        private Label ActionLabel;
        /// <summary>
        /// To identify which node has been clicked 
        /// </summary>
        private Label NodeIDLabel;
        
        
        /// <summary>
        /// Vector to translate from graph to world position in scene for a generated
        /// </summary>
        private Vector2IntField Vector2Location;  
        
        /// <summary>
        /// References a bundle who's prefab type then is used in the quest generated on scene 
        /// </summary>
        private ObjectField TargetField;
        private IntegerField TargetCount;
        private Button TargetCountIncrease;
        private Button TargetCountDecrease;
        
        /// <summary>
        /// contains the visual element to access target field
        /// </summary>
        private VisualElement ObjectFieldVe;
        /// <summary>
        /// contains the visual element to access vector field
        /// </summary>
        private VisualElement Vector2DVe;
        /// <summary>
        /// Display to indicate no Node from the graph has been selected
        /// </summary>
        private VisualElement NoNodeSelectedPanel;
        
        private QuestNodeBehaviour behaviour;
        
        public QuestNodeBehaviourEditor(object target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
            UpdatePanel();
        }
        public override void SetInfo(object target)
        {
            behaviour = target as QuestNodeBehaviour;
        }
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeBehaviourEditor");
            visualTree.CloneTree(this);
            
            ActionLabel = this.Q<Label>("ParamAction");

            ObjectFieldVe = this.Q<VisualElement>("ObjectFieldVe");
            TargetField = this.Q<ObjectField>("TargetField");
            TargetCount = this.Q<IntegerField>("TargetCount");
            TargetCountIncrease = this.Q<Button>("TargetCountIncrease");
            TargetCountDecrease = this.Q<Button>("TargetCountDecrease");

            Vector2DVe = this.Q<VisualElement>("Vector2DVe");
            Vector2Location = this.Q<Vector2IntField>("Vector2Location");

            NoNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");

            ObjectFieldVe.style.display = DisplayStyle.None;    
            Vector2DVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            
            return this;
        }

        public void SetTools(ToolKit toolkit)
        { 
            // Suscribe select tool to display a node's data
            var SelectTool = toolkit.TryGetTool("Select");
        }

        private void UpdatePanel()
        {
            
        }

        private QuestNode GetSelectedNode()
        {
            return behaviour.SelectedQuestNode;
        }
    }
}