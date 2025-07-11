using System;
using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using LBS;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestFlowBehaviour", typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private QuestNodeBehaviour _behaviour;
        
        private const float ActionBorderThickness = 1f;
        private const float BackgroundOpacity = 0.25f;
        
        private static readonly Dictionary<Type, Type> TypeToPanelMap = new()
        {
            { typeof(DataExplore), typeof(NodeEditorExplore) },
            { typeof(DataKill), typeof(NodeEditorKill) },
            { typeof(DataStealth), typeof(NodeEditorStealth) },
            { typeof(DataTake), typeof(NodeEditorTake) },
            { typeof(DataRead), typeof(NodeEditorRead) },
            { typeof(DataExchange), typeof(NodeEditorExchange) },
            { typeof(DataGive), typeof(NodeEditorGive) },
            { typeof(DataReport), typeof(NodeEditorReport) },
            { typeof(DataGather), typeof(NodeEditorGather) },
            { typeof(DataSpy), typeof(NodeEditorSpy) },
            { typeof(DataCapture), typeof(NodeEditorCapture) },
            { typeof(DataListen), typeof(NodeEditorListen) }
        };
        
        #endregion
        
        #region VIEW FIELDS
        /// <summary>
        /// Displays the action string
        /// </summary>
        private Label _paramActionLabel;
        /// <summary>
        /// To identify which node has been clicked 
        /// </summary>
        private Label _nodeIDLabel;
        
        private VisualElement _nodePanel;
        private VisualElement _actionPanel;
        private VisualElement _noNodeSelectedPanel;
        private VisualElement _instancedContent;
        
        private VisualElement _actionColor;
        private VisualElement _actionIcon;
        
        private PickerVector2Int _targetPosition;
        private RectField _area;
    


        #endregion
        
        #region CONSTRUCTORS
        public QuestNodeBehaviourEditor(object target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
            OnSelectNode(null);
        }
        #endregion
        
        #region METHODS
        public sealed override void SetInfo(object paramTarget)
        {
            _behaviour = paramTarget as QuestNodeBehaviour;
            _behaviour!.OnQuestNodeSelected += OnSelectNode;
            DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer);
        }
        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeBehaviourEditor");
            visualTree.CloneTree(this);
            
            #region Get VisualElements from UXML
            _nodePanel = this.Q<VisualElement>("ID");
            _actionPanel = this.Q<VisualElement>("Action");
            _noNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");
            
            _instancedContent = this.Q<VisualElement>("InstancedContent");
            
            _actionColor = this.Q<VisualElement>("ActionColor");
            _actionIcon = this.Q<VisualElement>("ActionIcon");
            
            _paramActionLabel = this.Q<Label>("ParamAction");
            _nodeIDLabel = this.Q<Label>("ParamID");
            #endregion
            
            #region Picker Position in Graph
            _targetPosition = this.Q<PickerVector2Int>("TargetPosition");
            _targetPosition.SetInfo("Trigger Position", "The position of the trigger in the graph.");
            _targetPosition.DisplayVectorField(false);
            
            _targetPosition._onClicked = () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;

                pickerManipulator.PickTriggerPosition = true;
                pickerManipulator.ActiveData = _behaviour.SelectedQuestNode.NodeData;
                
                if(pickerManipulator.ActiveData == null) return;
                
                pickerManipulator.OnPositionPicked = (pos) =>
                {
                    var nodeData = _behaviour.SelectedQuestNode.NodeData;
                    nodeData.Area = new Rect(pos.x,pos.y, nodeData.Area.width,nodeData.Area.height);
                    
                    // Refresh UI
                    _targetPosition.SetTarget(pos);
                };

            };
            #endregion
            
            _area = this.Q<RectField>("Area");
            _area.RegisterValueChangedCallback(evt => SetNodeDataArea(evt.newValue));
            
            // No node when instanced
            _noNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            
            return this;
        }
        
        private void SetNodeDataArea(Rect newValue)
        {
            BaseQuestNodeData nodeData = GetSelectedNode().NodeData;
            if (nodeData is null) return;
            
            newValue.x = Mathf.Round(newValue.x);
            newValue.y = Mathf.Round(newValue.y);
            newValue.height = MathF.Abs(newValue.height);
            newValue.width = MathF.Abs(newValue.width);
            
            nodeData.Area = newValue;
        //    DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer, MainView.Instance);
        }

        /// <summary>
        /// By default the quest picker tool sets the Trigger Position of the quest node
        /// </summary>
        /// <param name="toolkit"></param>
        public void SetTools(ToolKit toolkit)
        { 
            var questPicker = new QuestPicker();
            var t1 = new LBSTool(questPicker);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            toolkit.ActivateTool(t1,_behaviour?.OwnerLayer, target);
        }

        private void OnSelectNode(QuestNode node)
        {
            var validNode = node is not null && node.NodeData is not null;
            
            _noNodeSelectedPanel.style.display = validNode ? DisplayStyle.None : DisplayStyle.Flex;  
            _nodePanel.style.display = validNode ? DisplayStyle.Flex : DisplayStyle.None;
            _actionPanel.style.display = validNode ? DisplayStyle.Flex : DisplayStyle.None;
            _targetPosition.style.display = validNode ? DisplayStyle.Flex : DisplayStyle.None;
            _area.style.display = validNode ? DisplayStyle.Flex : DisplayStyle.None;
            
            _instancedContent.Clear();
            
            if (!validNode )  return;
            
            _paramActionLabel.text = node.QuestAction;
            _nodeIDLabel.text = node.ID;

            var dataType = node.NodeData.GetType();
            
            if (TypeToPanelMap.TryGetValue(dataType, out Type visualElementType))
            {
                if (visualElementType == null) return;
                if (Activator.CreateInstance(visualElementType) is not NodeEditor instance) return;
                
                _instancedContent.Add(instance);
                instance.SetNodeData(node.NodeData); // bindings per editor type
                SetBaseDataValues(node.NodeData); // for trigger position and size
          //      DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer, MainView.Instance); // Must draw in case changes were made
            }
            
            // if not in the dictionary just set the default data: For example "GoTo" action
            else
            {
                SetBaseDataValues(node.NodeData);
            }
        }
        
        private void SetBaseDataValues(BaseQuestNodeData data)
        {
            var backgroundColor = data.Color;
            backgroundColor.a = BackgroundOpacity;
            _actionColor.SetBackgroundColor(backgroundColor);
            
            _actionIcon.style.unityBackgroundImageTintColor = data.Color;
            _actionColor.SetBorder(data.Color,ActionBorderThickness);
            
            _area.value = data.Area;
            
        }

        private QuestNode GetSelectedNode()
        {
            return _behaviour.SelectedQuestNode;
        }

        #endregion
        
    }
}