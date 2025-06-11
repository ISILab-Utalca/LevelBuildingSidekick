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
            { typeof(DataExplore), typeof(QuestNode_Explore) },
            { typeof(DataKill), typeof(QuestNodeKill) },
            { typeof(DataStealth), typeof(QuestNodeStealth) },
            { typeof(DataTake), typeof(QuestNodeTake) },
            { typeof(DataRead), typeof(QuestNodeRead) },
            { typeof(DataExchange), typeof(QuestNodeExchange) },
            { typeof(DataGive), typeof(QuestNodeGive) },
            { typeof(DataReport), typeof(QuestNodeReport) },
            { typeof(DataGather), typeof(QuestNodeGather) },
            { typeof(DataSpy), typeof(QuestNodeSpy) },
            { typeof(DataCapture), typeof(QuestNode_Capture) },
            { typeof(DataListen), typeof(QuestNodeListen) }
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
        private FloatField _size;
    


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
            _targetPosition.vector2IntField.RegisterValueChangedCallback(evt =>
            {
                SetNodeDataPosition(evt.newValue);
            });
            _targetPosition._onClicked = () =>
            {  
                var pickerManipulator = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                if (pickerManipulator == null) return;
                
                pickerManipulator.ActiveData = _behaviour.SelectedQuestNode.NodeData;
                if(pickerManipulator.ActiveData == null) return;
                
                pickerManipulator.OnBundlePicked = (_, _, pos) =>
                {
                    // Update the bundle data
                    _behaviour.SelectedQuestNode.NodeData.Position = pos;
                    // Refresh UI
                    _targetPosition.SetTarget(pos);
                };

            };
            #endregion
            
            _size = this.Q<FloatField>("Size");
            _size.RegisterValueChangedCallback(evt => SetNodeDataSize(evt.newValue));
            
            // No node when instanced
            _noNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            
            return this;
        }

      
        private void SetNodeDataPosition(Vector2Int newValue)
        {
            BaseQuestNodeData nodeData = GetSelectedNode().NodeData;
            if (nodeData is null) return;
            nodeData.Position = newValue;
            DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer, MainView.Instance);
        }
        private void SetNodeDataSize(float newValue)
        {
            BaseQuestNodeData nodeData = GetSelectedNode().NodeData;
            if (nodeData is null) return;
            nodeData.Size = newValue;
            DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer, MainView.Instance);
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
            _size.style.display = validNode ? DisplayStyle.Flex : DisplayStyle.None;
            
            _instancedContent.Clear();
            
            if (!validNode )  return;
            
            _paramActionLabel.text = node.QuestAction;
            _nodeIDLabel.text = node.ID;

            var dataType = node.NodeData.GetType();
            
            if (TypeToPanelMap.TryGetValue(dataType, out Type visualElementType))
            {
                if (visualElementType != null)
                {
                    var instance = Activator.CreateInstance(visualElementType) as NodeEditor;
                    if (instance != null)
                    {
                        _instancedContent.Add(instance);
                        instance.SetNodeData(node.NodeData); // bindings per editor type
                        SetBaseDataValues(node.NodeData); // for trigger position and size
                        DrawManager.Instance.RedrawLayer(_behaviour.OwnerLayer, MainView.Instance); // Must draw in case changes were made
                    }
                }
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
            
            _targetPosition.vector2IntField.value = data.Position;
            _size.value = data.Size;
        }

        private QuestNode GetSelectedNode()
        {
            return _behaviour.SelectedQuestNode;
        }

        #endregion
        
    }
}