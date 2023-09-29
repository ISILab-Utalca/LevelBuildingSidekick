using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using System.Speech.Recognition;
using static UnityEngine.GraphicsBuffer;
using LBS.Settings;

namespace LBS.VisualElements
{
    public class ToolKit : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ToolKit, UxmlTraits> { }

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
                defaultValue = 0
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
                var btn = ve as ToolKit;

                btn.BaseColor = m_BaseColor.GetValueFromBag(bag, cc);
                btn.Index = m_Index.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        #region SINGLETON
        private static ToolKit instance;
        internal static ToolKit Instance 
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region FIELDS
        private List<(LBSTool,ToolButton)> tools = new();
        private (LBSTool,ToolButton) current;

        private Color baseColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private int index = 0;
        private int choiceCount = 0;
        #endregion

        #region FIELDS VIEW
        private VisualElement content;
        #endregion

        #region PROPERTIES
        public Color BaseColor
        {
            get => baseColor;
            set => baseColor = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }

        public int ChoiceCount
        {
            get => choiceCount;
            set => choiceCount = value;
        }
        #endregion

        #region EVENTS
        public event Action OnEndAction;
        public event Action OnStartAction;
        #endregion

        #region CONSTRUCTORS
        public ToolKit()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolKit");
            visualTree.CloneTree(this);

            this.content = this.Q<VisualElement>("Content");

            // Singleton
            if (instance != this)
                instance = this;
        }
        #endregion

        #region METHODS
        public void Init(LBSLayer layer)
        {
            if (tools.Count > 0)
            {
                SetActive(0);
                return;
            }

            // Esto lo esto sacando de LBSlocalBH y LBSLocalAss asi que 
            // mejor que se quede asi por ahora (16/08/23) si despues queda
            // separado el temap de la creacion de isnpectores para los "BH"
            // y los "Ass" entonces lo movere a aqui (!!!)
        }

        public void SetActive(int index)
        {
            if (tools.Count <= 0)
                return;

            this.index = index;

            if(current != default((LBSTool,ToolButton)))
                current.Item2.OnBlur();
            
            current = tools[index];
            current.Item2.OnFocus();

            var m = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(m);
        }

        public void SetActive(string value)
        {
            var index = tools.FindIndex(t => t.Item2.tooltip.Equals(value));
            SetActive(index);
        }

        public void SetActive()
        {
            if (tools.Count <= 0)
                return;
        }

        public void AddSubTools(LBSTool[] tool, int index = -1)
        {
            throw new NotImplementedException();
        }

        public void AddTool(LBSTool tool, int index = -1)
        {
            var button = new ToolButton(tool);
            (LBSTool, ToolButton) t = new(tool, button);
            tool.BindButton(button);

            this.content.Add(button);
            tools.Add(t);

            var i = tools.Count - 1;
            button.AddGroupEvent(() =>
            {
                var index = i;
                SetActive(index);
            });
            button.SetColorGroup(baseColor, LBSSettings.Instance.view.toolkitSelected);

            tool.OnStart += OnStartAction;
            tool.OnEnd += () => { OnEndAction(); };

        }

        public void AddSeparator(int height = 10)
        {
            var separator = new VisualElement();
            separator.style.height = height;
            this.content.Add(separator);
        }

        public new void Clear()
        {
            if (tools.Count <= 0)
                return;

            current.Item2.OnBlur();
            tools.Clear();
            this.content.Clear();
        }
        #endregion
    }
}
