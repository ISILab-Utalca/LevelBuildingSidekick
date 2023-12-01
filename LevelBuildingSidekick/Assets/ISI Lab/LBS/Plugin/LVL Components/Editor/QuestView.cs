using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class QuestView : VisualElement
{
    private LBSQuestGraph quest;
    private TextField questName;
    private VisualElement questIcon;
    private Button showButton;
    private Button hideButton;

    public event Action OnVisibilityChange;

    public QuestView() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayerView"); // Editor
        visualTree.CloneTree(this);

        // LayerName
        this.questName = this.Q<TextField>("Name");
        this.questName.RegisterCallback<ChangeEvent<string>>(e => {
            this.quest.Name = e.newValue;
        });

        // LayerIcon
        this.questIcon = this.Q<VisualElement>("Icon");

        // Show/Hide button
        this.showButton = this.Q<Button>("ShowButton");
        this.showButton.clicked += () => { ShowQuest(true); };
        this.hideButton = this.Q<Button>("HideButton");
        this.hideButton.clicked += () => ShowQuest(false);
    }

    public void SetInfo(LBSQuestGraph questGraph)
    {
        quest = questGraph;
        questName.value = quest.Name;

        ShowQuest(quest.IsVisible);
    }

    private void ShowQuest(bool value)
    {
        showButton.style.display = (!value) ? DisplayStyle.Flex : DisplayStyle.None;
        hideButton.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

        quest.IsVisible = value;
        OnVisibilityChange?.Invoke();
    }
}
