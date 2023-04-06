using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;

public class ComplexDropdown : VisualElement
{
    #region FIELDS

    private Type target = null;
    private List<ComplexDropdownElement> elements = new List<ComplexDropdownElement>();

    private VisualElement content;
    private TextField searchfield;

    #endregion

    #region PROPERTIES

    public Type Target
    {
        get => target;
        set => target = value;
    }

    #endregion

    #region EVENTS

    public delegate Action dropdownEvent(Type type);
    private dropdownEvent onSelectOption;

    public event dropdownEvent OnSelectOption
    {
        add => onSelectOption += value;
        remove => onSelectOption -= value;
    }

    #endregion

    public new class UxmlFactory : UxmlFactory<ComplexDropdown, VisualElement.UxmlTraits> { }

    public ComplexDropdown()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ComplexDropdown"); // Editor
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        searchfield = this.Q<TextField>("Searchfield");
        searchfield.RegisterCallback<ChangeEvent<string>> ((e) => {
            Actualize(e.newValue);
        });

        Init(target);
    }

    private void Init(Type type)
    {
        if (type == null)
            return;

        var tuples = Utility.Reflection.GetClassesWith(type);
        elements.Clear();
        foreach (var tuple in tuples)
        {
            var atts = tuple.Item2.ToList();

            if (atts.Count <= 0)
                continue;

            var att = atts[0] as LBSSearchAttribute;
            //var att = tuple.Item2.ToList().Find(at => at.GetType().Equals(target)) as LBSSearchAttribute;

            var element = new ComplexDropdownElement();
            element.SetAction(() => { onSelectOption?.Invoke(tuple.Item1); });
            element.SetInfo(att.Name, att.Icon);
            elements.Add(element);
        }

        Actualize("");
    }

    private void Actualize(string text)
    {
        content.Clear();

        var elms = elements.Where( e => e.name.Contains(text)).ToList();
        elms.ForEach(e => content.Add(e));
    }
}


