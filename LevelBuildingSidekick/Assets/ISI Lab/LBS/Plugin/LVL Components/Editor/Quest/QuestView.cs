using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestView : VisualElement
{
    private LBSLayer layer;
    private TextField questName;
    private VisualElement questIcon;
    private Button showButton;
    private Button hideButton;

    public event Action OnVisibilityChange;

    public QuestView() 
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("LayerView"); 
        visualTree.CloneTree(this);

        // LayerName
        this.questName = this.Q<TextField>("Name");
        this.questName.RegisterCallback<ChangeEvent<string>>(e => {
            this.layer.Name = e.newValue;
        });

        // LayerIcon
        this.questIcon = this.Q<VisualElement>("Icon");

        // Show/Hide button
        this.showButton = this.Q<Button>("ShowButton");
        this.showButton.clicked += () => { ShowQuest(true); };
        this.hideButton = this.Q<Button>("HideButton");
        this.hideButton.clicked += () => ShowQuest(false);
    }

    public void SetInfo(LBSLayer layer)
    {
        this.layer = layer;
        questName.value = layer.Name;

        ShowQuest(layer.IsVisible);
    }

    private void ShowQuest(bool value)
    {
        showButton.style.display = (!value) ? DisplayStyle.Flex : DisplayStyle.None;
        hideButton.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

        layer.IsVisible = value;
        OnVisibilityChange?.Invoke();
    }
}
