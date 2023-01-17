using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
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
            // busca todos los botones dentro de si mismo
            group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupable).Select(ve => ve as IGrupable).ToList();

            // les añade el metodo "Active"
            group.ForEach(b => b.AddEvent(() => {
                    Active(b); 
            }));

            // inicia el grupo con el primero activo
            if (!allowSwitchOff && group.Count > 0)
            {
                current = group[0];
                Active(current);
            }
        }

        private void ChangeActive(IGrupable active)
        {
            current?.OnBlur();
            current = active;
            current.OnFocus();
        }

        private void Active(IGrupable active)
        {
            if(!allowSwitchOff)
            {
                ChangeActive(active);
                return;
            }
            else
            {
                if (current == active)
                {
                    current.OnBlur();
                    current = null;
                }
                else
                {
                    ChangeActive(active);
                    return;
                }
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
                e.AddEvent(() => Active(e));
            }

            base.Add(element);
        }

        public new void Clear()
        {
            group.Clear();
            base.Clear();
        }
    }
}