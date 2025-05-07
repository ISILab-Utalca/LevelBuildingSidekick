


// ReSharper disable All

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS.Bundles;
using LBS.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
            UpdatePanel(null);
        }
        public override void SetInfo(object target)
        {
            behaviour = target as QuestNodeBehaviour;
            behaviour.OnQuestNodeSelected += UpdatePanel;
        }
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeBehaviourEditor");
            visualTree.CloneTree(this);
            
            ActionLabel = this.Q<Label>("ParamAction");
            NodeIDLabel = this.Q<Label>("ParamID");
            
            ObjectFieldVe = this.Q<VisualElement>("ObjectFieldVe");
            TargetField = this.Q<ObjectField>("TargetFieldBundle");
            TargetField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is Bundle bundle) SetTargetValue(bundle);
            });
            
            TargetCount = this.Q<IntegerField>("TargetCount");
            TargetCount.RegisterValueChangedCallback(evt => SetIntValue(evt.newValue));

            TargetCountIncrease = this.Q<Button>("TargetCountIncrease");
            TargetCountDecrease = this.Q<Button>("TargetCountDecrease");
            TargetCountIncrease.clicked += () =>
            {
                TargetCount.value = TargetCount.value + 1;
            };
            TargetCountDecrease.clicked += () =>
            {
                TargetCount.value = TargetCount.value - 1;
            };
            
            
            Vector2DVe = this.Q<VisualElement>("Vector2DVe");
            Vector2Location = this.Q<Vector2IntField>("Vector2LocationInput");
            Vector2Location.RegisterValueChangedCallback(evt => SetVector2IntValue(evt.newValue));
            
           
            NoNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");

            ObjectFieldVe.style.display = DisplayStyle.None;    
            Vector2DVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            

            
            return this;
        }

        private void SetTargetValue(Bundle newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;
            var bundleGuid = LBSAssetMacro.GetGuidFromAsset(newValue);
            switch (nd)
            {
                case QuestNodeDataKill kill:
                {
                    nd.SetGoal<string>(bundleGuid);
                    break;
                }
                
                case QuestNodeDataSteal steal:
                {
                    var tuple = new Tuple<string, Vector2Int>(bundleGuid, Vector2Location.value);
                    nd.SetGoal<Tuple<string, Vector2Int>>(tuple);
                    break;
                }
            }
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        private void SetIntValue(int newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;
            nd.SetNum(newValue);
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        private void SetVector2IntValue(Vector2Int newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;

            switch (nd)
            {
                case QuestNodeDataGoto locationData:
                {
                    nd.SetGoal<Vector2Int>(newValue);
                    break;
                }
                
                case QuestNodeDataSteal steal:
                {
                    var bundleGuid = LBSAssetMacro.GetGuidFromAsset(TargetField.value);
                    var tuple = new Tuple<string, Vector2Int>(bundleGuid, newValue);
                    nd.SetGoal<Tuple<string, Vector2Int>>(tuple);
                    break;
                }

            }
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        public void SetTools(ToolKit toolkit)
        { 
            /* Although it relies on the OnSelect tool, its functionality is called from QuestNode Mouse Down
                where the SelectedNode is set in QuestNodeBehavior 
            */
        }

        private void UpdatePanel(QuestNode node)
        {
            ObjectFieldVe.style.display = DisplayStyle.None;    
            Vector2DVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.None;    
            
            if (node is null || 
                node.NodeData is null ||
                node.NodeData is  QuestNodeDataEmpty emptyData)
            {
                NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
                return;
            }
            
            ActionLabel.text = node.QuestAction;
            NodeIDLabel.text = node.ID.ToString();
            
            SetPanelValuesWithNodeData(node);
        }

        private void SetPanelValuesWithNodeData(QuestNode node)
        {
            
            TargetCount.value = node.NodeData.Num;
            switch (node.NodeData)
            {
                case QuestNodeDataGoto locationData:
                    Vector2Location.value = locationData.position;
                    Vector2DVe.style.display = DisplayStyle.Flex;
                    break;

                case QuestNodeDataKill killData:
                    ObjectFieldVe.style.display = DisplayStyle.Flex;
                    TargetField.value = LBSAssetMacro.LoadAssetByGuid<Bundle>(
                        killData.bundleGuid);
                    break;
                
                case QuestNodeDataSteal stealData:
                    ObjectFieldVe.style.display = DisplayStyle.Flex;
                    Vector2Location.value = stealData.position;
                    TargetField.value = LBSAssetMacro.LoadAssetByGuid<Bundle>(
                        stealData.bundleGuid);
                    break;
                
            }
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        private QuestNode GetSelectedNode()
        {
            return behaviour.SelectedQuestNode;
        }
        
    }
}