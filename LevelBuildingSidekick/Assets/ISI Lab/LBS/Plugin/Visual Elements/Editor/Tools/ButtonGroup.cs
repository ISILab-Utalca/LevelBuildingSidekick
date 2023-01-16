using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonGroup : VisualElement // (!!) mejorar clase
{
    public new class UxmlFactory : UxmlFactory<ButtonGroup, UxmlTraits> { }

    public bool allowSwitchOff = false;
    private List<IGrupable> group = new List<IGrupable>();
    private IGrupable current;

    public ButtonGroup()
    {
        Init();
    }

    public void Init()
    {
        group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupable).Select(ve => ve as IGrupable).ToList();
        group.ForEach(b => b.SetEvent(() => Active(b)));

        if (!allowSwitchOff && group.Count > 0)
        {
            current = group[0];
            Active(current);
        }
    }

    private void Active(IGrupable active)
    {
        if (allowSwitchOff)
        {
            if (current == active)
            {
                current = null;
                active.SetActive(false);
            }
            else
            {
                group.ForEach(b => b.SetActive(false));
                current = active;
                active.SetActive(true);
            }
        }
        else
        {
            group.ForEach(b => b.SetActive(false));
            current = active;
            active.SetActive(true);
        }
    }


    public new void Remove(VisualElement element)
    {
        if (element is IGrupable)
            group.Remove(element as IGrupable);

        base.Remove(element);
    }

    public new void RemoveAt(int index)
    {
        var childs = base.Children().ToList();
        var element = childs[index];
        base.Remove(element);
    }

    public new void Add(VisualElement element)
    {
        if (element is IGrupable)
        {
            var e = element as IGrupable;
            group.Add(e);
            e.SetEvent(() => Active(e));
        }

        base.Add(element);
    }

    public new void Clear()
    {
        group.Clear();
        base.Clear();
    }
}
