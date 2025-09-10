using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS
{
    public class QuestVisualTree : MonoBehaviour
    {
        #region FIELDS
        [SerializeField]
        private GameObject trackerGo; // Reference to the GameObject holding QuestTracker

        [SerializeField]
        private QuestTracker tracker; // Reference to the QuestTracker component

        private UIDocument _questVisualTree; // UI document for the quest tree
        private TreeView _questTree; // TreeView UI element for displaying quests
        #endregion

        #region PROPERTIES
        public GameObject Go
        {
            get => trackerGo;
            set => trackerGo = value;
        }
        #endregion

 
        #region METHODS
        private void Start()
        {
            InitializeUI();
            SubscribeToTracker();
            UpdateQuest();
        }

        private void InitializeUI()
        {
            // Retrieves the UIDocument and TreeView for quest display
            _questVisualTree = GetComponentInParent<UIDocument>();
            if (_questVisualTree == null)
            {
                Debug.LogWarning("No UIDocument found in parent.");
                return;
            }

            _questTree = _questVisualTree.rootVisualElement.Q<TreeView>("QuestTree");
            if (_questTree == null)
            {
                Debug.LogWarning("No TreeView named 'QuestTree' found in UI.");
                return;
            }

            ConfigureTreeView();
        }

        private void SubscribeToTracker()
        {
            if (trackerGo == null) return;
            
            tracker = trackerGo.GetComponent<QuestTracker>();
            if (tracker != null)
            {
                tracker.OnQuestAdvance += UpdateQuest;
            }
            else
            {
                Debug.LogWarning("QuestTracker component not found on trackerGO.");
            }
        }

        private void ConfigureTreeView()
        {
            // Configures how TreeView items are created and bound
            // Note: VisualElementQuest is assumed to be a custom VisualElement for quests
            _questTree.makeItem = () => new VisualElementQuest();
            _questTree.bindItem = (element, index) =>
            {
                if (element is VisualElementQuest questEntryVe)
                {
                    var item = _questTree.GetItemDataForIndex<QuestObjective>(index);
                    questEntryVe.SetQuest(item);
                }
            };
        }

        private void UpdateQuest()
        {
            if (tracker?.Objectives == null)
            {
                Debug.LogWarning("Tracker or Objectives is null.");
                return;
            }

            var rootItems = BuildTreeItems();
            _questTree.SetRootItems(rootItems);
            _questTree.Rebuild();
            _questTree.ExpandAll();
        }

        private List<TreeViewItemData<QuestObjective>> BuildTreeItems()
        {
            // Builds the hierarchy of TreeView items for root objectives
            var rootItems = new List<TreeViewItemData<QuestObjective>>();
            var objectives = tracker.Objectives;

            foreach (var objective in objectives)
            {
                // Only include objectives that are not part of a branch
                if (objective.Trigger.OwnerBranchNode == null)
                {
                    rootItems.Add(BuildTreeRecursive(objective));
                }
            }

            return rootItems;
        }

        private TreeViewItemData<QuestObjective> BuildTreeRecursive(QuestObjective objective)
        {
            // Recursively builds the TreeView hierarchy for objectives and sub-objectives
            // Note: Uses GetInstanceID for unique IDs to avoid conflicts in TreeView
            var children = new List<TreeViewItemData<QuestObjective>>();

            foreach (var branch in objective.GetBranches())
            {
                var subObjectives = objective.GetSubObjectives(branch);
                if (subObjectives == null) continue;

                foreach (var subTrigger in subObjectives)
                {
                    var subObjective = new QuestObjective(subTrigger);
                    children.Add(BuildTreeRecursive(subObjective));
                }
            }

            return new TreeViewItemData<QuestObjective>(
                objective.Trigger.GetInstanceID(),
                objective,
                children
            );
        }
        #endregion
    }
}