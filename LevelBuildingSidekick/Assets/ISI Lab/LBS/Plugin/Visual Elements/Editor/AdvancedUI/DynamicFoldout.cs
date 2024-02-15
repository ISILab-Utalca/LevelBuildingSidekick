using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DynamicFoldout : VisualElement
{
    ClassDropDown dropdown;
    VisualElement content;

    private object data;
    
    static readonly VisualTreeAsset visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ClassFoldout");

    public object Data
    {
        get => data;
        set => SetInfo(value);
    }

    public string Label
    {
        get => dropdown.label;
        set => dropdown.label = value;  
    }

    public Action OnChoiceSelection;

    public DynamicFoldout(Type type) 
    {
        visualTree.CloneTree(this);

        var foldout = this.Q<ClassFoldout>();   
        
        dropdown = foldout.Q<ClassDropDown>();
        dropdown.RegisterValueChangedCallback(ApplyChoice);
        dropdown.Type = type;
        content = foldout.Q<VisualElement>(name: "unity-content");
    }

    void UpdateView(Type type, object data)
    {
        var p = LBS_Editor.pairsEditors;
        var veType = LBS_Editor.GetEditor(type);

        if (veType == null)
            return;

        var ve = Activator.CreateInstance(veType, new object[] { data }) as LBSCustomEditor;


        if (!(ve is VisualElement))
        {
            throw new Exception("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
        }

        this.data = data;
        content.Clear();
        content.Add(ve);
    }

    public void SetInfo(object data)
    {
        if (data != null)
        {
            dropdown.value = data.GetType().Name;
            UpdateView(data.GetType(), data);
        }
    }

    public void ApplyChoice(ChangeEvent<string> e)
    {
        var type = Reflection.GetType(e.newValue);

        if (type == null)
        {
            return;
            throw new Exception("[ISI Lab] Class type not found");
        }

        data = dropdown.GetChoiceInstance();
        UpdateView(type, data);
        OnChoiceSelection?.Invoke();
    }
}
