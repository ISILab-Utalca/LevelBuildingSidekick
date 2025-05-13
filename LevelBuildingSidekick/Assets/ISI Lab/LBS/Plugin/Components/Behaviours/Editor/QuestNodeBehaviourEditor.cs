


// ReSharper disable All

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS;
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
        /// contains the visual element to access target field
        /// </summary>
        private VisualElement TargetBundleVe;
            /// <summary>
            /// References a bundle who's prefab type then is used in the quest generated on scene 
            /// </summary>
            private ObjectField TargetBundle;
            private IntegerField TargetCount;
            private Button TargetCountIncrease;
            private Button TargetCountDecrease;
            
        /// <summary>
        /// contains the visual element to access vector field
        /// </summary>
        private VisualElement PositionVe;
            /// <summary>
            /// Vector to translate from graph to world position in scene for a generated
            /// </summary>
            private Vector2IntField Vector2Pos;
            
        /// <summary>
        /// contains the visual element to the time constraint variables
        /// </summary>
        private VisualElement ConstraintVe;
            private FloatField MaxDistance;
            private FloatField StayTime;
            
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
            behaviour.OnQuestDataChanged += SetPanelValuesWithNodeData;
        }
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeBehaviourEditor");
            visualTree.CloneTree(this);
            
            ActionLabel = this.Q<Label>("ParamAction");
            NodeIDLabel = this.Q<Label>("ParamID");
            
            TargetBundleVe = this.Q<VisualElement>("ObjectFieldVe");
            TargetBundle = this.Q<ObjectField>("TargetFieldBundle");
            TargetBundle.RegisterValueChangedCallback(evt =>
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
            
            
            PositionVe = this.Q<VisualElement>("Vector2DVe");
            Vector2Pos = this.Q<Vector2IntField>("Vector2LocationInput");
            Vector2Pos.RegisterValueChangedCallback(evt => SetVector2IntValue(evt.newValue));
            
            
            ConstraintVe = this.Q<VisualElement>("ConstraintVe");
            MaxDistance = this.Q<FloatField>("MaxDistanceInput");
            MaxDistance.RegisterValueChangedCallback(evt => SetMaxDistance(evt.newValue));
            StayTime = this.Q<FloatField>("StayTimeInput");
            StayTime.RegisterValueChangedCallback(evt => SetStayTime(evt.newValue));

           
            NoNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");

            TargetBundleVe.style.display = DisplayStyle.None;    
            PositionVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            

            
            return this;
        }

        private void SetStayTime(float evtNewValue)
        {
            
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;
        }

        private void SetMaxDistance(float evtNewValue)
        {
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;
            
        }

        private void SetTargetValue(Bundle newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if(nd is null)  return;
            var bundleGuid = LBSAssetMacro.GetGuidFromAsset(newValue);

            if (!nd.HasBundle()) return;
            nd.Bundle.bundleGuid = bundleGuid;
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
            if (nd is null) return;
            if (!nd.HasPosition()) return;

            nd.Position.position = newValue;
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        public void SetTools(ToolKit toolkit)
        { 
            Texture2D icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Icon=ColorPicker");
            var questPicker = new QuestPicker();
            var t1 = new LBSTool(icon, "Pick population element",
                "Pick the foremost population element from any layer within the graph." +
                " The picked bundle is assigned to the selected behaviour node", questPicker);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t1.Init(behaviour?.OwnerLayer, target);
            
            toolkit.AddTool(t1);
            
        }

        private void UpdatePanel(QuestNode node)
        {
            TargetBundleVe.style.display = DisplayStyle.None;    
            PositionVe.style.display = DisplayStyle.None;    
            ConstraintVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.None;    
            
            if (node is null || 
                node.NodeData is null )
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
            if (node is null) return;
            var nd = node.NodeData;
            if (nd is null) return;
            
            TargetCount.value = nd.Num;

            if (nd.HasPosition())
            {
                PositionVe.style.display = DisplayStyle.Flex;
                Vector2Pos.value = nd.Position.position;
            }

            if (nd.HasBundle())
            {
                TargetBundleVe.style.display = DisplayStyle.Flex;
                TargetBundle.value = LBSAssetMacro.LoadAssetByGuid<Bundle>(
                    nd.Bundle.bundleGuid);
            }

            if (nd.HasConstraint())
            {
                PositionVe.style.display = DisplayStyle.Flex;
                Vector2Pos.value = nd.Position.position;
            }
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        private QuestNode GetSelectedNode()
        {
            return behaviour.SelectedQuestNode;
        }
        
    }
}