using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System;

public class CharacteristicsPanel : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CharacteristicsPanel, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private ComplexDropdown search;

    private Bundle target;

    public CharacteristicsPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsPanel");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        search = this.Q<ComplexDropdown>();
        search.Init(typeof(LBSCharacteristicAttribute));
        search.OnSelected = (e) => 
        {
            var x = e as LBSCharacteristic;
            target.AddCharacteristic(x);
            SetInfo(target);
            AssetDatabase.SaveAssets();
        };
    }

    public void SetInfo(Bundle target)
    {
        this.target = target;

        content.Clear();
        var characs = target.Characteristics;

        foreach (var characteristic in characs)
        {
            var view = new CharacteristicsBaseView();
            view.SetContent(characteristic);
            content.Add(view);
        }
    }
}
