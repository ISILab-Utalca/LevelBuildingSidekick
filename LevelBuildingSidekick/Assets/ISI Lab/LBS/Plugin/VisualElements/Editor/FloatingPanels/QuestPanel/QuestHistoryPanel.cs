using ISILab.LBS.Editor.Windows;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class QuestHistoryPanel : VisualElement
    {
        #region VIEW ELEMENTS
        private ListView _questList;
        private QuestFlowBehaviour _questBehaviour;
        #endregion

        #region FIELDS

        private readonly List<QuestEntry> _questEntries = new();
        private bool _isRefreshing;
        private bool _isSubscribed;

        #endregion

        #region PROPERTIES
        private QuestGraph QuestGraphs { get; set; } = new();

        #endregion

        #region METHODS

        public void SetInfo(QuestFlowBehaviour target)
        {
            Clear();

            if (target == null) return;

            _questBehaviour = target;
            CreateVisualElement();
        }

        private void CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestHistoryPanel");
            visualTree.CloneTree(this);

            _questEntries.Clear();

            if (_questBehaviour == null) return;

            QuestGraphs = _questBehaviour.Graph;

            _questList = this.Q<ListView>("QuestList");
            _questList.reorderable = true;

            _questList.makeItem = () => new QuestEntry();

            _questList.bindItem = (element, index) =>
            {
                if (index < 0 || index >= QuestGraphs.QuestNodes.Count)
                    return;

                if (element is not QuestEntry questEntryVe)
                    return;

                var quest = QuestGraphs.QuestNodes[index];
                questEntryVe.SetData(quest);
                _questEntries.Add(questEntryVe);

                questEntryVe.RemoveNode = () =>
                {
                    QuestGraphs.RemoveQuestNode(quest);
                    QuestGraphs.UpdateFlow.Invoke();
                    // No direct Refresh(); UpdateFlow will trigger it.
                };

                questEntryVe.GoToNode = () => GoToNode(quest);
            };

            _questList.itemsRemoved += (_) =>
            {
                Refresh();
            };
            
            _questList.itemIndexChanged += (_, _) =>
            {
                QuestGraphs?.Reorder();
                // Let UpdateFlow trigger Refresh
            };

            _questList.itemsSource = QuestGraphs.QuestNodes;

            // Avoid double subscription
            if (!_isSubscribed)
            {
                QuestGraphs.UpdateFlow += Refresh;
                _isSubscribed = true;
            }

            Refresh();
        }

        private void GoToNode(QuestNode node)
        {
            if (node == null) return;
            QuestGraphs?.GoToNode?.Invoke(node);
        }

        private void UpdateVeQuestEntries()
        {
            foreach (var qe in _questEntries)
            {
                qe?.Update();
            }
        }

        private void Refresh()
        {
            if (_isRefreshing) return;
            _isRefreshing = true;

            _questEntries.Clear();

            _questList?.Rebuild();
            QuestGraphs?.UpdateQuestNodes();
            UpdateVeQuestEntries();

            DrawManager.Instance.RedrawLayer(QuestGraphs?.OwnerLayer, MainView.Instance);
            LBSMainWindow.OnWindowRepaint?.Invoke();

            MarkDirtyRepaint();

            _isRefreshing = false;
        }

        #endregion
    }
}
