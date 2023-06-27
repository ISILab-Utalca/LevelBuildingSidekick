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
    public class listElement
    {
        public string name;
        public Texture2D icon;
        public Type type;

        public listElement(string name, Texture2D icon,Type type)
        {
            this.name = name;
            this.icon = icon;
            this.type = type;
        }
    }
    public List<listElement> elements = new List<listElement>();

    #region FIELDS
    private Type targetType = null;

    private ListView content;
    private TextField searchfield;
    #endregion

    #region PROPERTIES
    public Type Target
    {
        get => targetType;
        set => targetType = value;
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

    public Action<object> OnSelected;

    public ComplexDropdown()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ComplexDropdown"); // Editor
        visualTree.CloneTree(this);

        content = this.Q<ListView>("Content");
        content.makeItem = () => { return new ComplexDropdownElement(); };
        content.bindItem = (view, index) => 
        {
            var field = view as ComplexDropdownElement;
            var item = content.itemsSource[index] as listElement;
            field.SetInfo(item.name, item.icon, false);
        };

        content.selectionChanged += (e) =>
        {
            var index = content.selectedIndex;
            var selected = content.itemsSource[index] as listElement;
            try
            {
                OnSelected?.Invoke(Activator.CreateInstance(selected.type));

            }
            catch(Exception ex)
            {
                var x = ex.Message;
                Debug.LogWarning("<color=#ff0000ff><b>[ISI Lab]</b></color>: The '" + selected.type + "' class does not have a base parameterless constructor.");
            }
        };

        searchfield = this.Q<TextField>("Searchfield");
        searchfield.RegisterValueChangedCallback((e) =>
        {
            Actualize(e.newValue);
        });

        searchfield.RegisterCallback<ChangeEvent<string>> ((e) =>
        {
            Actualize(e.newValue);
            content.SetDisplay(true);
        });

        searchfield.RegisterCallback<FocusEvent>((e) =>
        {
            Actualize(searchfield.value);
            content.SetDisplay(true);
        });

        searchfield.RegisterCallback<BlurEvent>((e) =>
        {
            content.SetDisplay(false);
        });

        content.SetDisplay(false);
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
            elements.Add(new listElement(att.Name, att.Icon, tuple.Item1));
        }
    }

    public void Actualize(string text)
    {
        content.ClearClassList();
        content.makeItem = () => { return new ComplexDropdownElement(); };
        content.bindItem = (view, index) =>
        {
            var field = view as ComplexDropdownElement;
            var item = content.itemsSource[index] as listElement;
            field.SetInfo(item.name, item.icon, false);
        };

        var elements = this.elements.Where(e => 
        e.name.ToLower()
        .Contains(text.ToLower()))
        .ToList();

        content.itemsSource = elements;
    }
}


