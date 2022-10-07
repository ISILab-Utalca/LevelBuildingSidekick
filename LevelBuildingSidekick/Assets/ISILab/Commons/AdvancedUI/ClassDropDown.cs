using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ClassDropDown //this could inherit from DropDown
{
    public DropdownField Dropdown;
    public Type Type;
    bool FilterAbstract;

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
        IEnumerable<Type> types = null;

        if(Type.IsClass)
        {
            types = Utility.Reflection.GetAllSubClassOf(Type);
        }
        else if(Type.IsInterface)
        {
            types = Utility.Reflection.GetAllImplementationsOf(Type);
        }
        //Utility.Reflection.PrintDerivedTypes(typeof(IOptimizer));
        if (FilterAbstract)
        {
            types = types.Where(t => !t.IsAbstract);
        }

        var options = types.Select(t => t.Name).ToList();

        Dropdown.choices = options;
    }

    public object GetChoiceInstance()
    {
        object obj = null;

        var t = Type.GetType(Dropdown.value);
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
