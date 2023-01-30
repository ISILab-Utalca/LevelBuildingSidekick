using Commons.Optimization.Evaluator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class ClassDropDown : DropdownField
{
    public Type Type;
    bool FilterAbstract;

    private List<Type> types;

    public ClassDropDown(Type type, bool filterAbstract)
    {
        Type = type;
        FilterAbstract = filterAbstract;

        UpdateOptions();
        RegisterCallback<MouseDownEvent>((e) => UpdateOptions());
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
