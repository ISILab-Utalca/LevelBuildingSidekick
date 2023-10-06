using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ButtonGroup : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ButtonGroup, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlColorAttributeDescription m_BaseColor = new UxmlColorAttributeDescription 
            { 
                name = "base-color",
                defaultValue = new Color(72f / 255f, 72f / 255f, 72f / 255f)
            };

            UxmlColorAttributeDescription m_SelectedColor = new UxmlColorAttributeDescription 
            { 
                name = "selected-color", 
                defaultValue = new Color(215f / 255f, 127f / 255f, 45f / 255f)
            };

            UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
            {
                name = "index",
                defaultValue = -1
            };

            UxmlStringAttributeDescription m_choices = new UxmlStringAttributeDescription
            {
                name = "choices",
                defaultValue = ""
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ButtonGroup btn = ve as ButtonGroup;

                btn.SelectedColor = m_SelectedColor.GetValueFromBag(bag, cc);
                btn.BaseColor = m_BaseColor.GetValueFromBag(bag, cc);
                btn.Index = m_Index.GetValueFromBag(bag, cc);
                btn.SetChoices(m_choices.GetValueFromBag(bag, cc));
            }
        };
        #endregion

        #region FIELDS
        public bool allowSwitchOff = false;
        private List<IGrupable> group = new List<IGrupable>();
        private IGrupable current;

        private Color baseColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private Color selectedColor = new Color(215f / 255f, 127f / 255f, 45f / 255f);
        private int index = -1;
        private string choices = "";
        private int choiceCount = 0;
        #endregion

        #region PROPERTIES
        public Color BaseColor
        {
            get => baseColor;
            set => baseColor = value;
        }

        public Color SelectedColor
        {
            get => selectedColor;
            set => selectedColor = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }

        public string Choices
        {
            get => choices;
            set
            {
                choices = value;
                SetChoices(value);
            }
        }

        public int ChoiceCount
        {
            get => choiceCount;
            set => choiceCount = value;
        }
        #endregion

        #region CONSTRUCTORS
        public ButtonGroup()
        {
            Init();
        }
        #endregion

        #region METHODS
        public void Init()
        {
            // busca todos los botones dentro de si mismo
            group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupable).Select(ve => ve as IGrupable).ToList();

            // les añade el metodo "Active"
            group.ForEach(b => b.AddGroupEvent(() => {
                b.SetColorGroup(baseColor, selectedColor);
            }));

            // inicia el grupo con el primero activo
            if (!allowSwitchOff && group.Count > 0)
            {
                current = group[0];
                current.OnFocus();
                Active(current);
            }
        }

        public void SetChoices(string choices)
        {
            var cs = choices.Split(",");
            this.Clear();
            var count = 0;
            foreach (var choice in cs)
            {
                if (choice == "")
                    continue;

                count++;
                var cv = new GrupalbeButton(choice);
                cv.SetColorGroup(baseColor, selectedColor);
                cv.style.flexGrow = 1;
                this.Add(cv);
            }
            ChoiceCount = count;
            this.Init();
        }

        public void ChangeActive(IGrupable active)
        {
            current?.OnBlur();
            current = active;
            current.OnFocus();
        }

        public void ChangeActive(int index)
        {
            current?.OnBlur();
            current = group[index];
            current.OnFocus();
        }

        public void ChangeActive(string label)
        {
            current?.OnBlur();
            current = group.Find(b => b.GetLabel() == label);
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
        #endregion
    }
}