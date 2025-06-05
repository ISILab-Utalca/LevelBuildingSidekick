


// ReSharper disable All

using System;
using System.Collections.Generic;
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
        
        static private Dictionary<Type, Type> typeToPanelMap = new()
        {
            { typeof(DataExplore), typeof(QuestNode_Explore) },
            { typeof(DataKill), typeof(QuestNode_Kill) },
            { typeof(DataStealth), typeof(QuestNode_Stealth) },
            { typeof(DataTake), typeof(QuestNode_Take) },
            { typeof(DataRead), typeof(QuestNode_Read) },
            { typeof(DataExchange), typeof(QuestNode_Exchange) },
            { typeof(DataGive), typeof(QuestNode_Give) },
            { typeof(DataReport), typeof(QuestNode_Report) },
            { typeof(DataGather), typeof(QuestNode_Gather) },
            { typeof(DataSpy), typeof(QuestNode_Spy) },
            { typeof(DataCapture), typeof(QuestNode_Capture) },
            { typeof(DataListen), typeof(QuestNode_Listen) }
        };
        
        #endregion
        
        
        #region VIEW FIELDS
        /// <summary>
        /// Displays the action string
        /// </summary>
        private Label ActionLabel;
        /// <summary>
        /// To identify which node has been clicked 
        /// </summary>
        private Label NodeIDLabel;
        private VisualElement NoNodeSelectedPanel;
        private VisualElement InstancedContent;
        private Vector2IntField TargetPosition;
        private FloatField Size;
        
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
        public override void SetInfo(object target)
        {
            behaviour = target as QuestNodeBehaviour;
            behaviour.OnQuestNodeSelected += OnSelectNode;
        }
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNodeBehaviourEditor");
            visualTree.CloneTree(this);
            
            ActionLabel = this.Q<Label>("ParamAction");
            NodeIDLabel = this.Q<Label>("ParamID");
            NoNodeSelectedPanel = this.Q<VisualElement>("NoNodeSelectedPanel");
            InstancedContent = this.Q<VisualElement>("InstancedContent");
            
            // Generic (Go To) Values
            TargetPosition = this.Q<Vector2IntField>("TargetPosition");
            TargetPosition.RegisterValueChangedCallback(evt =>
            {
                SetNodeDataPosition(evt.newValue);
            });

            Size = this.Q<FloatField>("Size");
            Size.RegisterValueChangedCallback(evt =>
            {
                SetNodeDataSize(evt.newValue);
            });

            
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;    
            
            return this;
        }

      
        private void SetNodeDataPosition(Vector2Int newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if (nd is null) return;
            nd.position = newValue;
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }
        private void SetNodeDataSize(float newValue)
        {
            var nd = GetSelectedNode().NodeData;
            if (nd is null) return;
            nd.size = newValue;
            DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
        }

        public void SetTools(ToolKit toolkit)
        { 
            var questPicker = new QuestPicker();
            var t1 = new LBSTool(questPicker);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            toolkit.ActivateTool(t1,behaviour?.OwnerLayer, target);
            
        }

        private void OnSelectNode(QuestNode node)
        {
     
            NoNodeSelectedPanel.style.display = DisplayStyle.Flex;  
            TargetPosition.style.display = DisplayStyle.None;
            Size.style.display = DisplayStyle.None;
            
            if (node is null || 
                node.NodeData is null )
            {
                  
                return;
            }
            
            NoNodeSelectedPanel.style.display = DisplayStyle.None; 
            InstancedContent.Clear();
            
            ActionLabel.text = node.QuestAction;
            NodeIDLabel.text = node.ID.ToString();

            var dataType = node.NodeData.GetType();
            if (typeToPanelMap.TryGetValue(dataType, out Type visualElementType))
            {
                if (visualElementType != null)
                {
                    var instance = Activator.CreateInstance(visualElementType) as INodeEditor;
                    if (instance != null)
                    {
                        InstancedContent.Add(instance as VisualElement);
                        instance.SetMyData(node.NodeData);
                        SetBaseDataValues(node.NodeData); 
                        DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance); // Must draw in case changes were made
                    }
                }
            }
        }
        

        
        private void SetBaseDataValues(BaseQuestNodeData data)
        {
            TargetPosition.style.display = DisplayStyle.Flex;
            Size.style.display = DisplayStyle.Flex;
            TargetPosition.value = data.position;
            Size.value = data.size;
        }

        private QuestNode GetSelectedNode()
        {
            return behaviour.SelectedQuestNode;
        }

        #endregion
        
    }
}