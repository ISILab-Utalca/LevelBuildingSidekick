using ISILab.LBS.Editor.Windows;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor;

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
                questEntryVe.GoToNode += () => GoToNode(questGraph.QuestNodes[index]);
            };
            
            questList.itemIndexChanged += (_, _) =>
            {
                questGraph?.Reorder();
                Refresh();
            };
            
            questList.itemsSource = questGraph.QuestNodes;
            
            questGraph.UpdateFlow -= Refresh;
            questGraph.UpdateFlow += Refresh;
            Refresh();
        }
        
        // should pass the preset as parameter
        private void AddEntry()
        {
            var questEntryVe = new QuestEntry();
            questEntries.Add(questEntryVe);
        }

        private void GoToNode(QuestNode node)
        {
            if(node == null) return;
            questGraph?.GoToNode.Invoke(node);
        }
        
        private void UpdateVeQuestEntries()
        {
            foreach (var qe in questEntries)
            {
                qe?.Update();
            }
        }
        
        public void Refresh()
        {
            questList?.Rebuild();
            
            questGraph.UpdateQuestNodes();
            
            UpdateVeQuestEntries();
            
            DrawManager.Instance.RedrawLayer(questGraph?.OwnerLayer, MainView.Instance);

            LBSMainWindow.OnWindowRepaint?.Invoke();
            
            MarkDirtyRepaint();
        }
        
        #endregion
       
    }
    

}

