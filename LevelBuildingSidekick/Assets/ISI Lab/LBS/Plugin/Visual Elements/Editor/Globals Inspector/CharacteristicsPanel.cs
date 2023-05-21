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

    public CharacteristicsPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("CharacteristicsPanel");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        search = this.Q<ComplexDropdown>();
        search.Init(typeof(LBSCharacteristicAttribute));
    }

    public void SetInfo(Bundle target)
    {
        content.Clear();
        var characs = target.Characteristics;

        var cs = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>();
        foreach (var charac in characs)
        {
            var relation = cs.Find(t => t.Item2.ToList()[0].type == charac.GetType());
            var editor = Activator.CreateInstance(cs[0].Item1) as LBSCustomEditor;

            if (editor == null)
                editor = new LBSNullEditor();

            editor.SetInfo(charac);
            content.Add(editor);
        }
    }
}
