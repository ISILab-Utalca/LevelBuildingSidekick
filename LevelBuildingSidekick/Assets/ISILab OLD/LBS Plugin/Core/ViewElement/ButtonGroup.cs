using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public interface IGrupable
    {
        public void AddEvent(Action action);
        public void SetActive(bool value);
    }

    public class ButtonGroup : VisualElement
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
            group.ForEach(b => b.AddEvent(() => Active(b)));

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
                //if (current == active)
                //    return;

                group.ForEach(b => b.SetActive(false));
                current = active;
                active.SetActive(true);
            }
        }

        public void Remove(IGrupable btn)
        {
            group.Remove(btn);
        }

        public void AddMember(IGrupable btn)
        {
            group.Add(btn);
        }

    }
}