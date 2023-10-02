using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Commons.Optimization.Evaluator;

public class ClassDropDown : DropdownField
{
    public new class UxmlFactory : UxmlFactory<ClassDropDown, UxmlTraits> { }

    public new class UxmlTraits : DropdownField.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "Label", defaultValue = "Class DropDown" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            ClassDropDown field = (ClassDropDown)ve;
            field.label = m_Label.GetValueFromBag(bag, cc);
        }
    }

    #region FIELDS
    Type type;

    bool filterAbstract;
    private List<Type> types;

    #endregion

    public string Value
    {
        get => value;
        set
        {
            if (choices.Contains(value))
                this.value = value;
        }
    }

    public Type TypeValue => types[choices.IndexOf(value)];

    public Type Type
    {
        get => type;
        set
        {
            type = value;
            UpdateOptions();
        }
    }

    public bool FilterAbstract
    {
        get => filterAbstract;
        set
        {
            filterAbstract = value;
            UpdateOptions();
        }
    }

    public ClassDropDown()
    {
        label = "Class DropDown";
    }

    void UpdateOptions()
    {
        choices.Clear();

        List<Type> types = null;

        if (Type.IsClass)
        {
            types = Utility.Reflection.GetAllSubClassOf(Type).ToList();
        }
        else if (Type.IsInterface)
        {
            types = Utility.Reflection.GetAllImplementationsOf(Type).ToList();
        }

        if (filterAbstract)
        {
            types = types.Where(t => !t.IsAbstract).ToList();
        }

        var options = types.Select(t => t.Name).ToList();

        this.types = types;
        choices = options;
    }

    public object GetChoiceInstance()
    {
        object obj = null;
        var dv = value;
        var dx = choices.IndexOf(dv);
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
