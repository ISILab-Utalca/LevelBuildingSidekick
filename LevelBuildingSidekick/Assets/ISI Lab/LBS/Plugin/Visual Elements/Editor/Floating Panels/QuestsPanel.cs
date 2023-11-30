using LBS.Components;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestsPanel : VisualElement
{

    public LBSLevelData data;

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

    public QuestsPanel(LBSLevelData data)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayersPanel"); // Editor
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
        list.itemsSource = data.Layers;
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
        throw new NotImplementedException();
    }

    private void AddQuest()
    {
        throw new NotImplementedException();
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
