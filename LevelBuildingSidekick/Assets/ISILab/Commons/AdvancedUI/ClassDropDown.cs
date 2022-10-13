using Commons.Optimization.Evaluator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class ClassDropDown //this could inherit from DropDown (!!!) Si, porvafor
{
    public DropdownField Dropdown;
    public Type Type;
    bool FilterAbstract;

    private List<Type> _types;

    public ClassDropDown(DropdownField dropdown, Type type, bool filterAbstract)
    {
        Dropdown = dropdown;
        Type = type;
        FilterAbstract = filterAbstract;

        UpdateOptions();
        Dropdown.RegisterCallback<MouseDownEvent>((e) => UpdateOptions());
    }

    void UpdateOptions()
    {
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

        var options = types.Select(t => {
            var value = t.Name;
            return value;
        }).ToList();

        _types = types;
        Dropdown.choices = options;
    }

    public object GetChoiceInstance()
    {
        object obj = null;
        var dv = Dropdown.value;
        var dx = Dropdown.choices.IndexOf(dv);
        //var t = Type.GetType(dv,true);
        //obj = Activator.CreateInstance(t);
        //var t = Reflection.GetType(dv);
        var t = _types[dx];
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
