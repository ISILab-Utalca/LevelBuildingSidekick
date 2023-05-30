using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    private LBSCharacteristic characteristic;

    public CharacteristicsBaseView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsBaseView");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        menuBtn = this.Q<Button>("MenuBtn");

        removeBtn = this.Q<Button>("RemoveBtn");
        removeBtn.clicked += () =>
        {
            var bundle = characteristic.Owner;
            bundle.RemoveCharacteristic(characteristic);
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
        this.characteristic = characteristic;

        var cs = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>(); // esto puede ocurrir cuando se recompila en vez de cada vez (!!!)
        
        var relation = cs.Find((t) =>
        {
            return t.Item2.ToList()[0].type == characteristic.GetType();
        });

        var editor = Activator.CreateInstance(relation.Item1) as LBSCustomEditor;
        
        if (editor == null)
            editor = new LBSNullEditor();
        
        //var e = Editor.CreateEditor(characteristic);
        //var ve = e.CreateInspectorGUI();
        //this.content.Add(ve);

        editor.SetInfo(characteristic);
        this.nameLabel.text = relation.Item1.Name;
        this.icon.style.backgroundImage = null; // implementar esto en LBSCustomEditorAttribute (!!!)

        this.content.Add(editor);
    }
}
