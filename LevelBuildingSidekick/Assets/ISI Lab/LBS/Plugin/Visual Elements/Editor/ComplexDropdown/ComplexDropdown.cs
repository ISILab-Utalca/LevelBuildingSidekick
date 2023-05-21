using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;
using UnityEditor.UIElements;
using UnityEditor;

public class ComplexDropdown : VisualElement
{
    public class Asdf
    {
        public string name;
        public Texture icon;
        public Type type;

        public Asdf(string name, Texture icon,Type type)
        {
            this.name = name;
            this.icon = icon;
            this.type = type;
        }
    }
    public List<Asdf> elements = new List<Asdf>();

    #region FIELDS
    private Type target = null;

    private ListView content;
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

        content = this.Q<ListView>("Content");
        content.makeItem += () => { return new ComplexDropdownElement(); };
        content.bindItem += (view, index) => 
        {
            var field = view as ComplexDropdownElement;
            var item = content.itemsSource[index];
            field.SetInfo("", null, false);
        };

        searchfield = this.Q<TextField>("Searchfield");
        searchfield.RegisterCallback<ChangeEvent<string>> ((e) => {
            Actualize(e.newValue);
        });

        searchfield.RegisterCallback<FocusEvent>((e) =>
        {
            Actualize("");
        });
    }

    public void Init(Type type)
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
            elements.Add(new Asdf(att.Name, att.Icon, tuple.Item1));
        }

        Actualize("");
    }

    private void Actualize(string text)
    {
        content.Clear();

        var elements = this.elements.Where( e => e.name.Contains(text)).ToList();

        content.itemsSource = elements;
    }
}


