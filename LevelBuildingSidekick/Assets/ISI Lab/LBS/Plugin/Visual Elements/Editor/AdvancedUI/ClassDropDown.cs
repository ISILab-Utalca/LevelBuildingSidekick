using Commons.Optimization.Evaluator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class ClassDropDown : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ClassDropDown, VisualElement.UxmlTraits> { }

    #region FIELDS

    public Label label;
    public DropdownField dropdown;
    Type type;

    bool FilterAbstract;
    private List<Type> types;

    #endregion

    #region PROPERTIES

    public string Label
    {
        get => label.text;
        set => label.text = value;
    }

    public string Value
    {
        get => dropdown.value;
        set
        {
            if (dropdown.choices.Contains(value))
                dropdown.value = value;
        }
    }

    public Type Type
    {
        get => type;
        set
        {
            type = value;
            UpdateOptions();
        }
    }
    #endregion

    public ClassDropDown() : base()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Dropdown");
        visualTree.CloneTree(this);


        label = this.Q<Label>();
        dropdown = this.Q<DropdownField>();

        FilterAbstract = true;

        dropdown.RegisterCallback<MouseDownEvent>((e) => UpdateOptions());

    }

    public ClassDropDown(Type type, bool filterAbstract) : base()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Dropdown");
        visualTree.CloneTree(this);


        label = this.Q<Label>();
        dropdown = this.Q<DropdownField>();

        Type = type;
        FilterAbstract = filterAbstract;

        UpdateOptions();
        dropdown.RegisterCallback<MouseDownEvent>((e) => UpdateOptions());
    }

    void UpdateOptions()
    {
        dropdown.choices.Clear();

        List<Type> types = null;

        if(Type.IsClass)
        {
            types = Utility.Reflection.GetAllSubClassOf(Type).ToList();
        }
        else if(Type.IsInterface)
        {
            types = Utility.Reflection.GetAllImplementationsOf(Type).ToList();
        }

        if (FilterAbstract)
        {
            types = types.Where(t => !t.IsAbstract).ToList();
        }

        var options = types.Select(t => t.Name).ToList();

        this.types = types;
        dropdown.choices = options;
    }

    public object GetChoiceInstance()
    {
        object obj = null;
        var dv = dropdown.value;
        var dx = dropdown.choices.IndexOf(dv);
        var t = types[dx];
        try
        {
            obj = Activator.CreateInstance(t);
        }
        catch
        {
            throw new FormatException(t + " class needs to have an empty constructor.");
        }

        return obj;
    }

}
