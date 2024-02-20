using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
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
            questName = this.Q<TextField>("Name");
            questName.RegisterCallback<ChangeEvent<string>>(e =>
            {
                layer.Name = e.newValue;
            });

            // LayerIcon
            questIcon = this.Q<VisualElement>("Icon");

            questIcon.style.backgroundImage = DirectoryTools.SearchAssetByName<Texture2D>("IconQuestMainWindw");

            // Show/Hide button
            showButton = this.Q<Button>("ShowButton");
            showButton.clicked += () => { ShowQuest(true); };
            hideButton = this.Q<Button>("HideButton");
            hideButton.clicked += () => ShowQuest(false);
        }

        public void SetInfo(LBSLayer layer)
        {
            this.layer = layer;
            questName.value = layer.Name;

            ShowQuest(layer.IsVisible);
        }

        private void ShowQuest(bool value)
        {
            showButton.style.display = !value ? DisplayStyle.Flex : DisplayStyle.None;
            hideButton.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

            layer.IsVisible = value;
            OnVisibilityChange?.Invoke();
        }
    }
}