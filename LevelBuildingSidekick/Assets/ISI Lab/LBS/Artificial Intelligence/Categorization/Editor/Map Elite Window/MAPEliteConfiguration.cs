using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MAPEliteConfiguration : VisualElement
{
    private AssistantMapElite assistant;

    private DropdownField dropdown;
    private Button undoBtn;
    private Button continueBtn;
    private Button calculateBtn;
    private Button editPresset;

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


        dropdown = this.Q<DropdownField>(name: "PresetDropDown");
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
        var pressets = LBSAssetsStorage.Instance.Get<MAPElitesPreset>();
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

    public MAPElitesPreset GetPresset()
    {
        return LBSAssetsStorage.Instance.Get<MAPElitesPreset>().Find(p => p.name == dropdown.value);
    }

    public void EditPresset(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("New Preset", (a) => { OpenPressetWindow(null);});
        evt.menu.AppendAction("Copy Preset", (a) => { OpenPressetWindow(GetPresset().Clone() as MAPElitesPreset); });
        evt.menu.AppendAction("Edit Preset", (a) => { OpenPressetWindow(GetPresset()); });
    }

    void OpenPressetWindow(MAPElitesPreset preset)
    {
        ME_PresetEditWindow.OpenWindow(preset);
    }
}
