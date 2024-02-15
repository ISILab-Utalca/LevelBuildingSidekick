using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;

public class CharacteristicsBaseView : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<CharacteristicsBaseView, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private Button removeBtn;
    private Button menuBtn;
    private Label nameLabel;
    private VisualElement icon;
    private Foldout foldout;

    private LBSCharacteristic target;

    public CharacteristicsBaseView()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsBaseView");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        menuBtn = this.Q<Button>("MenuBtn");

        removeBtn = this.Q<Button>("RemoveBtn");
        removeBtn.clicked += () =>
        {
            var bundle = target.Owner;
            bundle.RemoveCharacteristic(target);
            this.parent.Remove(this);
        };

        nameLabel = this.Q<Label>("Name");

        icon = this.Q<VisualElement>("Icon");

        foldout = this.Q<Foldout>("Foldout");
        this.foldout.RegisterCallback<ChangeEvent<bool>>((e) => 
        { 
            content.SetDisplay(e.newValue);
        });

    }

    public void SetContent(LBSCharacteristic characteristic)
    {
        this.target = characteristic;

        var cs = Reflection.GetClassesWith<LBSCustomEditorAttribute>();

        var relation = cs.Find((t) =>
        {
            return t.Item2.ToList()[0].type == characteristic.GetType();
        });

        LBSCustomEditor editor;
        if (relation == null)
        {
            editor = new LBSNullEditor();
            this.nameLabel.text = characteristic.GetType().Name;
            this.icon.style.backgroundImage = null; // TODO: Implement default icon  
        }
        else
        {
            editor = Activator.CreateInstance(relation.Item1) as LBSCustomEditor;
            this.nameLabel.text = relation.Item2.ToList()[0].name;
            this.icon.style.backgroundImage = null;
        }

        editor.SetInfo(characteristic);

        this.content.Add(editor);
    }
}
