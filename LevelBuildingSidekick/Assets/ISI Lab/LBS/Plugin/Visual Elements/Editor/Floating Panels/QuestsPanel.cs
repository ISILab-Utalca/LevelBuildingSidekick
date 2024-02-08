using ISILab.Commons.Utility.Editor;
using LBS.Components;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    public class QuestsPanel : VisualElement
    {
        public LBSLevelData data;

        private ListView list;
        private TextField nameField;
        private DropdownField typeDropdown;

        #region EVENTS
        public event Action<LBSLayer> OnAddQuest;
        public event Action<LBSQuestGraph> OnRemoveQuest;
        public event Action<LBSLayer> OnSelectQuest;
        public event Action<LBSLayer> OnDoubleSelectQuest;
        public event Action<LBSLayer> OnQuestVisibilityChange;
        #endregion

        public QuestsPanel() { }

        public QuestsPanel(LBSLevelData data)
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestsPanel");
            visualTree.CloneTree(this);

            this.data = data;

            // LayerList
            list = this.Q<ListView>("List");

            list.itemsSource = this.data.Quests;

            Func<VisualElement> makeItem = () =>
            {
                return new QuestView();
            };

            list.bindItem += (item, index) =>
            {
                if (index >= this.data.Quests.Count)
                    return;

                var view = (item as QuestView);
                var quest = this.data.Quests[index];
                view.SetInfo(quest);
                view.OnVisibilityChange += () => { OnQuestVisibilityChange(quest); };
            };

            list.fixedItemHeight = 20;
            list.itemsSource = data.Quests;
            list.makeItem += makeItem;
            list.itemsChosen += OnItemChosen;
            list.selectionChanged += OnSelectionChange;

            // NameField
            nameField = this.Q<TextField>("NameField");

            // AddLayerButton
            var addLayerBtn = this.Q<Button>("AddLayerButton");
            addLayerBtn.clicked += AddQuest;

            // RemoveSelectedButton
            var RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
            RemoveSelectedBtn.clicked += RemoveSelectedQuest;
        }

        private void RemoveSelectedQuest()
        {
            if (data.Quests.Count <= 0)
                return;

            var index = list.selectedIndex;
            if (index < 0)
                return;

            var answer = EditorUtility.DisplayDialog("Caution",
            "You are about to delete a layer. If you proceed with this action, all of its" +
            " content will be permanently removed, and you won't be able to recover it. Are" +
            " you sure you want to continue?", "Continue", "Cancel");

            if (!answer)
                return;

            var quest = data.RemoveQuestAt(index);
            OnRemoveQuest?.Invoke(quest);

            list.Rebuild();

            DrawManager.ReDraw();
        }

        private void AddQuest()
        {
            var name = nameField.text;

            int i = 1;
            while (data.Quests.Any(l => l.Name.Equals(name)))
            {
                name = nameField.text + " " + i;
                i++;
            }

            var q = data.AddQuest(name);
            list.selectedIndex = 0;
            list.Rebuild();

            OnAddQuest?.Invoke(q);

            DrawManager.ReDraw();
        }

        // Simple Click over element
        private void OnSelectionChange(IEnumerable<object> objs)
        {
            if (objs.Count() <= 0)
                return;

            var selected = objs.ToList()[0] as LBSLayer;
            OnSelectQuest?.Invoke(selected);
        }

        // Double Click over element
        private void OnItemChosen(IEnumerable<object> objs)
        {
            if (objs.Count() <= 0)
                return;

            var selected = objs.ToList()[0] as LBSLayer;
            OnDoubleSelectQuest?.Invoke(selected);
        }

        public void ResetSelection()
        {
            list.ClearSelection();
        }

    }
}
