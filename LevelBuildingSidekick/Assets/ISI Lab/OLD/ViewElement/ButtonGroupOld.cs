using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public interface IGrupableOld
    {
        public void AddEvent(Action action);
        public void SetActive(bool value);
    }

    public class ButtonGroupOld : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ButtonGroupOld, UxmlTraits> { }

        public bool allowSwitchOff = false;
        private List<IGrupableOld> group = new List<IGrupableOld>();
        private IGrupableOld current;

        public ButtonGroupOld()
        {
            Init();
        }

        public void Init()
        {
            group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupableOld).Select(ve => ve as IGrupableOld).ToList();
            group.ForEach(b => b.AddEvent(() => Active(b)));

            if (!allowSwitchOff && group.Count > 0)
            {
                current = group[0];
                Active(current);
            }
        }

        private void Active(IGrupableOld active)
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

        public void Remove(IGrupableOld btn)
        {
            group.Remove(btn);
        }

        public void AddMember(IGrupableOld btn)
        {
            group.Add(btn);
        }

    }
}