using LBS.Components;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QuestsPanel : VisualElement
{

    public LBSQuestManager data;

    private ListView list;
    private TextField nameField;
    private DropdownField typeDropdown;

    #region EVENTS
    public event Action<LBSQuestGraph> OnAddQuest;
    public event Action<LBSQuestGraph> OnRemoveQuest;
    public event Action<LBSQuestGraph> OnSelectQuest; // click simple (!)
    public event Action<LBSQuestGraph> OnDoubleSelectQuest; // doble click (!)
    public event Action<LBSQuestGraph> OnQuestVisibilityChange;
    #endregion

    public QuestsPanel() { }

    public QuestsPanel(LBSQuestManager data)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestsPanel"); // Editor
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

        //DrawManager.ReDraw();
    }

    private void AddQuest()
    {
        var quest = new LBSQuestGraph();
        quest.Name = nameField.text;

        int i = 1;
        while (data.Quests.Any(l => l.Name.Equals(quest.Name)))
        {
            quest.Name = nameField.text + " " + i;
            i++;
        }

        data.AddQuest(quest);
        list.selectedIndex = 0;
        OnAddQuest?.Invoke(quest);
        list.Rebuild();
    }

    // Simple Click over element
    private void OnSelectionChange(IEnumerable<object> objs)
    {
        if (objs.Count() <= 0)
            return;

        var selected = objs.ToList()[0] as LBSQuestGraph;
        OnSelectQuest?.Invoke(selected);
    }

    // Double Click over element
    private void OnItemChosen(IEnumerable<object> objs)
    {
        if (objs.Count() <= 0)
            return;

        var selected = objs.ToList()[0] as LBSQuestGraph;
        OnDoubleSelectQuest?.Invoke(selected);
    }




}
