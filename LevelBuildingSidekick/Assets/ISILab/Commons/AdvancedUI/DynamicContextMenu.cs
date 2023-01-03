using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DynamicContextMenu : GraphElement
{
    VisualElement Menu;

    static DynamicContextMenu _Instance;
    public static DynamicContextMenu Instance
    {
        get 
        {
            if (_Instance == null)
                _Instance = new DynamicContextMenu();
            return _Instance;
        }
    }

    public DynamicContextMenu() 
    {
        Menu = new VisualElement();
        Menu.pickingMode = PickingMode.Ignore;
    }

    public DynamicContextMenu(List<Tuple<string, Action>> actions, Vector2 position)
    {
        Update(actions, position);
    }

    public void Update(List<Tuple<string, Action>> actions, Vector2 position)
    {
        var max = actions.Max(t => t.Item1.Length);
        Menu.Clear();
        SetPosition(new Rect(position + Vector2.right*60, new Vector2(max*8, actions.Count * 20)));
        foreach (var action in actions)
        {
            var func = new GenericMenu.MenuFunction(action.Item2);
            Menu.Add(new Button(action.Item2) { text = action.Item1});
        }
        Add(Menu);
    }

    public void Destroy()
    {
        Menu.Clear();
        SetPosition(new Rect(Vector2.zero, Vector2.zero));
    }
}
