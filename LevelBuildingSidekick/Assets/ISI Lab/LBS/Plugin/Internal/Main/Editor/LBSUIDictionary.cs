using ISILab.Commons.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LBSUIDictionary : MonoBehaviour
{
    private static Dictionary<object, VisualElement> visualElements = new Dictionary<object, VisualElement>();

    public static LBSCustomEditor GetVisualElement(object element)
    {
        if (visualElements.ContainsKey(element))
            if(visualElements[element] is LBSCustomEditor)
                return visualElements[element] as LBSCustomEditor;
            else 
                throw new Exception("[ISI Lab] " + visualElements[element].GetType() + " is not a LBSCustomEditor ");

        var type = element.GetType();
        var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
            .Where(t => t.Item2.Any(v => v.type == type)).ToList();

        if (ves.Count() == 0)
        {
            throw new Exception("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
        }

        var ovg = ves.First().Item1;
        var ve = Activator.CreateInstance(ovg, new object[] { element });
        if (!(ve is LBSCustomEditor))
        {
            throw new Exception("[ISI Lab] " + ve.GetType() + " is not a LBSCustomEditor ");
        }

        return ve as LBSCustomEditor;
    }
}
