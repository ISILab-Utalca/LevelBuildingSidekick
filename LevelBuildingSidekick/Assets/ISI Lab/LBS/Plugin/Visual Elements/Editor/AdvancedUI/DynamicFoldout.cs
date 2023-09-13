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
    
    static readonly VisualTreeAsset visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ClassFoldout");

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
        //dropdown.RegisterValueChangedCallback(ApplyChoice);
        content = foldout.Q<VisualElement>(name: "unity-content");
    }

    void UpdateView(Type type, object data)
    {
        var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>().Where(t => t.Item2.Any(v => v.type == type));

        if (ves.Count() == 0)
        {
            content.Clear();
            content.Add(new Label("[ISI Lab] No class marked as CustomVisualElement found for type: " + type));
            return;
            //throw new Exception("[ISI Lab] No class marked as CustomVisualElement found for type: " + type);
        }

        var ve = Activator.CreateInstance(ves.First().Item1, new object[] { data}) as LBSCustomEditor;//, new object[] { dropdown.GetChoiceInstance()});
        //ve.SetInfo(data);

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
        var type = Utility.Reflection.GetType(e.newValue);

        if (type == null)
        {
            return;
            throw new Exception("[ISI Lab] Class type not found");
        }
        /*
        if(data != null && data.GetType() == type)
        {
            Debug.Log("Nothing happens");
            return;
        }
        else
        {
            Debug.Log(data + " - " + type);
            
        }*/

        data = dropdown.GetChoiceInstance();
        UpdateView(type, data);
        OnChoiceSelection?.Invoke();
    }
}
