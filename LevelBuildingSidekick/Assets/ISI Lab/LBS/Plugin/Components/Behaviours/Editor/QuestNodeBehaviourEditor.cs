


// ReSharper disable All

using System;
using System.Linq;
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
        #region FIELDS
        private QuestNodeBehaviour behaviour;

        #region VIEW FIELDS
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
        /// contains the visual elements for the other target
        /// </summary>
        private VisualElement OtherTargetBundleVe;
            private ObjectField OtherTargetBundle;
            private IntegerField OtherTargetCount;
            private Button OtherTargetCountIncrease;
            private Button OtherTargetCountDecrease;
            
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
            private IntegerField AreaSize;
            private FloatField StayTime;
            
        /// <summary>
        /// Display to indicate no Node from the graph has been selected
        /// </summary>
        private VisualElement NoNodeSelectedPanel;
        
        private VeQuestTilePicker PickerTarget;
        private Button PickerLocation;
        #endregion
        
        #endregion
        
        #region CONSTRUCTORS
        public QuestNodeBehaviourEditor(object target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
            UpdatePanel(null);
        }
        #endregion
        
        #region METHODS
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
            
            #region Target
            TargetBundleVe = this.Q<VisualElement>("ObjectFieldVe");
            TargetBundle = this.Q<ObjectField>("TargetFieldBundle");
            TargetBundle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is Bundle bundle) SetTargetValue(bundle);
            });
            
            TargetCount = this.Q<IntegerField>("TargetCount");
            TargetCount.RegisterValueChangedCallback(evt => SetCounter(evt.newValue));

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
            #endregion
            
            #region OtherTarget
            OtherTargetBundleVe = this.Q<VisualElement>("OtherTargetFieldVe");
            OtherTargetBundle = this.Q<ObjectField>("OtherTargetFieldBundle");
            OtherTargetBundle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is Bundle bundle) SetOtherTargetValue(bundle);
            });
            
            OtherTargetCount = this.Q<IntegerField>("OtherTargetCount");
            OtherTargetCount.RegisterValueChangedCallback(evt => SetIntValue(evt.newValue, false));

            OtherTargetCountIncrease = this.Q<Button>("OtherTargetCountIncrease");
            OtherTargetCountDecrease = this.Q<Button>("OtherTargetCountDecrease");
            OtherTargetCountIncrease.clicked += () =>
            {
                OtherTargetCount.value = OtherTargetCount.value + 1;
            };
            OtherTargetCountDecrease.clicked += () =>
            {
                OtherTargetCount.value = OtherTargetCount.value - 1;
            };
            #endregion
            
            #region Position
            PositionVe = this.Q<VisualElement>("Vector2DVe");
            Vector2Pos = this.Q<Vector2IntField>("Vector2LocationInput");
            Vector2Pos.RegisterValueChangedCallback(evt => SetVector2IntValue(evt.newValue));
            #endregion
            
            #region Constraint
            ConstraintVe = this.Q<VisualElement>("ConstraintVe");
            AreaSize = this.Q<IntegerField>("AreaSizeInput");
            AreaSize.RegisterValueChangedCallback(evt => SetMaxDistance(evt.newValue));
            StayTime = this.Q<FloatField>("StayTimeInput");
            StayTime.RegisterValueChangedCallback(evt => SetStayTime(evt.newValue));
            #endregion
           
            #region Pickers
            PickerTarget = this.Q<VeQuestTilePicker>("PickerTarget");
            PickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                qp.activeData = behaviour.SelectedQuestNode.NodeData;
                qp.OnBundlePicked = (string guid) =>
                {
                    ///behaviour.SelectedQuestNode.NodeData.bundleGuid = guid;
                };
            };
            
            PickerLocation = this.Q<Button>("PickerLocation");
            PickerLocation.clicked += () => {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                qp.activeData = behaviour.SelectedQuestNode.NodeData;
            };
            
            #endregion
            
            NoNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");

            OtherTargetBundleVe.style.display = DisplayStyle.None;    
            TargetBundleVe.style.display = DisplayStyle.None;    
            PositionVe.style.display = DisplayStyle.None;    
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            
            return this;
        }

        private void SetStayTime(float evtNewValue)
        {
            throw new NotImplementedException();
        }

        private void SetMaxDistance(int evtNewValue)
        {
            throw new NotImplementedException();
        }

        private void SetIntValue(int evtNewValue, bool b)
        {
            throw new NotImplementedException();
        }

        private void SetOtherTargetValue(Bundle bundle)
        {
            throw new NotImplementedException();
        }

        private void SetTargetValue(Bundle bundle)
        {
            throw new NotImplementedException();
        }

        private void SetCounter(int evtNewValue)
        {
            throw new NotImplementedException();
        }

        private void SetVector2IntValue(Vector2Int newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if (nd is null) return;
            nd.position = newValue;
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        public void SetTools(ToolKit toolkit)
        { 
            var questPicker = new QuestPicker();
            var t1 = new LBSTool(questPicker);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            toolkit.ActivateTool(t1,behaviour?.OwnerLayer, target);
            
        }

        private void UpdatePanel(QuestNode node)
        {
            TargetBundleVe.style.display = DisplayStyle.None;    
            OtherTargetBundleVe.style.display = DisplayStyle.None;
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
            
            PositionVe.style.display = DisplayStyle.Flex;
            Vector2Pos.value = nd.position;

            
            /*
            if (nd.HasBundle())
            {
                TargetBundleVe.style.display = DisplayStyle.Flex;
                TargetBundle.value = LBSAssetMacro.LoadAssetByGuid<Bundle>(
                    nd.Bundle.FirstOrDefault().bundleGuid);
                TargetCount.value = nd.Bundle.FirstOrDefault().num;
                
                if (nd.Bundle.Count > 1)
                {
                    OtherTargetBundleVe.style.display = DisplayStyle.Flex;
                    OtherTargetBundle.value = LBSAssetMacro.LoadAssetByGuid<Bundle>(
                        nd.Bundle[1].bundleGuid);
                    OtherTargetCount.value = nd.Bundle[1].num;
                }
            }

            if (nd.HasConstraint())
            {
                ConstraintVe.style.display = DisplayStyle.Flex;
                AreaSize.value = nd.Constrain.FirstOrDefault().areaSize;
                StayTime.value = nd.Constrain.FirstOrDefault().time;
            }
            */
            
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        private QuestNode GetSelectedNode()
        {
            return behaviour.SelectedQuestNode;
        }
        #endregion
        
    }
}