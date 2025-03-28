using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Generators;
using ISILab.LBS.Modules;
using Unity.Properties;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class QuestHistoryPanel : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private ListView questList;
        private QuestFlowBehaviour questBehaviour;
        #endregion

        #region FIELDS
        private QuestGraph questGraph = new ();
        private List<QuestEntry> questEntries = new ();
    
        #endregion

        #region PROPERTIES

        QuestGraph QuestGraphs
        {
            get => questGraph;
            set => questGraph = value;
        }
        #endregion

        #region EVENTS

        #endregion
        
        #region CONSTRUCTORS
        public QuestHistoryPanel()
        {

        }
        
        #endregion

        #region METHODS

      
        
        public void SetInfo(QuestFlowBehaviour target)
        {
            Clear();
            if (target == null) return;
            questBehaviour = target;
            CreateVisualElement();
        }
        
        private void CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestHistoryPanel");
            visualTree.CloneTree(this);

            questEntries.Clear();

            if (questBehaviour == null) return;

            questGraph = questBehaviour.Graph;
            foreach (var unused in questBehaviour.Graph.QuestNodes)
            {
                AddEntry();
            }
            
            questList = this.Q<ListView>("QuestList");
            questList.reorderable = true;
            
            questList.makeItem = () => new QuestEntry(); 
            questList.bindItem = (element, index) =>
            {
                var questEntryVe = element as QuestEntry;
                if (questEntryVe == null) return;

                var quest = questGraph.QuestNodes[index]; 
               
                questEntryVe.SetData(quest);
                questEntries.Add(questEntryVe);
                
                questEntryVe.RemoveNode = null;
                questEntryVe.RemoveNode += () =>
                {
                    questGraph.RemoveQuestNode(questGraph.QuestNodes[index]);
                    Refresh();
                };

                questEntryVe.GoToNode = null;
                questEntryVe.GoToNode += GoToNode;
            };
            
            questList.itemIndexChanged += (_, _) =>
            {
                Refresh();
                questGraph?.Reorder();
                UpdateVeQuestEntries();
                //Refresh();
            };
            
            questList.itemsSource = questGraph.QuestNodes;

            Refresh();
        }

        // should pass the preset as parameter
        private void AddEntry()
        {
            var questEntryVe = new QuestEntry();
            questEntries.Add(questEntryVe);
        }

        private void GoToNode()
        { 
            Debug.Log("Go To Node in graph!!!");
        }
        
        private void UpdateVeQuestEntries()
        {
            foreach (var qe in questEntries)
            {
                if(qe==null) continue;
                qe.Update();
            }
            LBSMainWindow.OnWindowRepaint?.Invoke();
            DrawManager.ReDraw();
        }
        
        public void Refresh()
        {
            questList?.Rebuild();
            
            questGraph.UpdateQuestNodes();
            
            UpdateVeQuestEntries();
            
            MarkDirtyRepaint();
        }
        
        #endregion
       
    }
}