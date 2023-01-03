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
        style.alignContent = Align.FlexStart;
        Menu.style.alignContent = Align.Stretch;
        style.overflow = Overflow.Visible;
    }

    public DynamicContextMenu(List<Tuple<string, Action>> actions, Vector2 position)
    {
        Update(actions, position);
    }

    public void Update(List<Tuple<string, Action>> actions, Vector2 position)
    {
        var max = actions.Max(t => t.Item1.Length);
        Menu.Clear();
        float width = max * 8;
        List<Button> buttons = new List<Button>();
        foreach (var action in actions)
        {
            var func = new GenericMenu.MenuFunction(action.Item2);
            var button = new Button(action.Item2) { text = action.Item1 };
            //buttons.Add(button);
            Menu.Add(button);
        }
        //width = buttons.Max(b => b.contentRect.width);
        SetPosition(new Rect(position, new Vector2(0, 0)));
        Add(Menu);
    }

    public void Destroy()
    {
        Menu.Clear();
        SetPosition(new Rect(Vector2.zero, Vector2.zero));
    }
}
