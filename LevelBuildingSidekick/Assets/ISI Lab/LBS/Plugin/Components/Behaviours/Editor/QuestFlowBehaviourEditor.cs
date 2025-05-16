using ISILab.AI.Grammar;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable All

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestFlowBehaviour", typeof(QuestFlowBehaviour))]
    public class QuestFlowBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private AddQuestNode addNode;
        private RemoveQuestNode removeNode;
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
        public override void SetInfo(object target)
        {
            Clear();
            behaviour = target as QuestFlowBehaviour;
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