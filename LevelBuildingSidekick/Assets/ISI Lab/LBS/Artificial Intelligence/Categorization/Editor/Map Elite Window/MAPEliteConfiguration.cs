using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MAPEliteConfiguration : VisualElement
{
    AssistantMapElite assistant;

    DropdownField dropdown;
    Button undoBtn;
    Button continueBtn;
    Button calculateBtn;
    Button editPresset;

    public Action OnCalculate;
    public Action OnContinue;
    public Action OnUndo;

    public Action<string> OnPressetChange;

    private static VisualTreeAsset visualTree;

    public MAPEliteConfiguration()
    {

        visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MAPEliteConfiguration");
        //var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MAPEliteConfiguration");
        visualTree.CloneTree(this);

        undoBtn = this.Q<Button>(name : "UndoBtn");
        undoBtn.clicked += Undo;

        continueBtn = this.Q<Button>(name: "ContinueBtn");
        continueBtn.clicked += Continue;

        calculateBtn = this.Q<Button>(name: "CalculateBtn");
        calculateBtn.clicked += Calculate;

        editPresset = this.Q<Button>(name: "EditPresset");


        var menu = new ContextualMenuManipulator(EditPresset);
        menu.target = editPresset;

        //editPresset.clicked += () => OpenPressetWindow(null);


        dropdown = this.Q<DropdownField>(name: "PressetDropDown");
        dropdown.RegisterValueChangedCallback(e => OnPressetChange?.Invoke(e.newValue));
        UpdateDropdown();

        var s2 = EditorGUIUtility.Load("DefaultCommonDark.uss") as StyleSheet;
        styleSheets.Add(s2);
    }

    public void Calculate()
    {
        OnCalculate?.Invoke();
    }

    public void Continue()
    {
        OnContinue?.Invoke();
    }

    public void Undo()
    {
        OnUndo?.Invoke();
    }

    private void UpdateDropdown()
    {
        var pressets = LBSAssetsStorage.Instance.Get<MAPElitesPresset>();
        if(pressets == null)
        {
            return;
        }
        if(pressets.Count <= 0)
        {
            return;
        }
        var options = pressets.Select(pressets => pressets.name).ToList();
        dropdown.choices = options;
    }

    public MAPElitesPresset GetPresset()
    {
        return LBSAssetsStorage.Instance.Get<MAPElitesPresset>().Find(p => p.name == dropdown.value);
    }

    public void EditPresset(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("New Presset", (a) => { OpenPressetWindow(null);});
        evt.menu.AppendAction("Copy Presset", (a) => { OpenPressetWindow(GetPresset().Clone() as MAPElitesPresset); });
        evt.menu.AppendAction("Edit Presset", (a) => { OpenPressetWindow(GetPresset()); });
    }

    void OpenPressetWindow(MAPElitesPresset preset)
    {
        ME_PressetEditWindow.OpenWindow(preset);
    }
}
