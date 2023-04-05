using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ButtonGroup : VisualElement // (!!) mejorar clase
    {
        public new class UxmlFactory : UxmlFactory<ButtonGroup, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlColorAttributeDescription m_BaseColor = new UxmlColorAttributeDescription { name = "base-color", defaultValue = Color.blue };
            UxmlColorAttributeDescription m_SelectedColor = new UxmlColorAttributeDescription { name = "selected-color", defaultValue = Color.red };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ButtonGroup btn = ve as ButtonGroup;

                btn.selected = m_SelectedColor.GetValueFromBag(bag, cc);
                btn.color = m_BaseColor.GetValueFromBag(bag, cc);
            }
        };

        public bool allowSwitchOff = false;
        private List<IGrupable> group = new List<IGrupable>();
        private IGrupable current;

        private Color selected = new Color(255f/ 255f, 189f/ 255f, 0);
        private Color color = new Color(0,0,0,0.2f);

        public ButtonGroup()
        {
            Init();
        }

        public void Init()
        {
            // busca todos los botones dentro de si mismo
            group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupable).Select(ve => ve as IGrupable).ToList();

            // les añade el metodo "Active"
            group.ForEach(b => b.AddGroupEvent(() => {
                b.SetColorGroup(color, selected);
                Active(b); 
            }));

            // inicia el grupo con el primero activo
            if (!allowSwitchOff && group.Count > 0)
            {
                current = group[0];
                current.OnFocus();
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
                e.AddGroupEvent(() => Active(e));
            }

            base.Add(element);
        }

        public new void Clear()
        {
            if (current != null)
                current.OnBlur();
            group.Clear();
            base.Clear();
        }
    }
}