using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;

public class ComplexDropdown<T> : VisualElement where T : LBSSearchAttribute
{
    #region FIELDS

    public T target; // parche (!!!) esto tiene que estar con un tipo T o algo asi
    private List<ComplexDropdownElement> elements;

    private VisualElement content;
    private TextField searchfield;

    #endregion

    #region EVENTS

    public delegate Action dropdownEvent(Type type);
    private dropdownEvent onSelectOption;

    #endregion

    #region PROPERTIES

    public event dropdownEvent OnSelectOption
    {
        add => onSelectOption += value;
        remove => onSelectOption -= value;
    }

    #endregion

    //public new class UxmlFactory : UxmlFactory<ComplexDropdown<T>, VisualElement.UxmlTraits> { }

    public ComplexDropdown()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ComplexDropdown"); // Editor
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");

        searchfield = this.Q<TextField>("Searchfield");
        searchfield.RegisterCallback<ChangeEvent<string>> ((e) => {
            Actualize(e.newValue);
        });

        Init();
    }

    private void Init()
    {
        var tuples = Utility.Reflection.GetClassesWith<T>();

        foreach (var tuple in tuples)
        {
            var att = tuple.Item2.ToList().Find(att => att.GetType().Equals(typeof(T)));

            var element = new ComplexDropdownElement();
            element.SetAction(() => { onSelectOption(tuple.Item1); });
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


