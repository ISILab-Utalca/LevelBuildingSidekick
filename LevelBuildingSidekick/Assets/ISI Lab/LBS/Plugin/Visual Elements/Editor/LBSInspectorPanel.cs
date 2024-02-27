using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class LBSInspectorPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }
        #endregion

        #region SINGLETON
        private static LBSInspectorPanel instance;
        public static LBSInspectorPanel Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region FIELDS
        private VisualElement content;
        private ButtonGroup tabsGroup;
        private string selectedTab;

        public LBSLocalCurrent data;
        public LBSLocalBehaviours behaviours;
        public LBSLocalAssistants assistants;

        private Dictionary<string, LBSInspector> VEs = new();
        #endregion

        #region EVENTS
        public event Action<string> OnChangeTab;
        #endregion

        #region CONSTRUCTORS
        public LBSInspectorPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSInspectorPanel");
            visualTree.CloneTree(this);

            // Tabs
            tabsGroup = this.Q<ButtonGroup>("SubTabs");
            InitTabs();

            // Content
            content = this.Q<VisualElement>("InspectorContent");

            SetSelectedTab("Current data");

            instance = this;
        }
        #endregion

        #region METHODS
        public void InitTabs()
        {
            this.data = new LBSLocalCurrent();
            AddTab("Current data", data);

            this.behaviours = new LBSLocalBehaviours();
            AddTab("Behaviours", behaviours);

            this.assistants = new LBSLocalAssistants();
            AddTab("Assistants", assistants);

            tabsGroup.OnChangeTab += (tab) =>
            {
                this.ClearContent();
                VEs.TryGetValue(tab, out var inspct);
                this.SetContent(inspct);

                OnChangeTab?.Invoke(tab);
            };
        }

        private void AddTab(string tab, LBSInspector element)
        {
            VEs.Add(tab, element);

            tabsGroup.AddChoice(tab, (btn) =>
            {
                var grupableBtn = btn as GrupalbeButton;
                this.ClearContent();
                VEs.TryGetValue(grupableBtn.text, out var inspct);
                this.SetContent(inspct);
            });
        }

        public void SetSelectedTab(string name)
        {
            this.ClearContent();
            VEs.TryGetValue(name, out var ve);
            this.SetContent(ve);

            OnChangeTab?.Invoke(name);
        }

        private void ClearContent()
        {
            content.Clear();
        }

        private void SetContent(VisualElement inspector)
        {
            if (inspector == null)
                return;

            content.Add(inspector);
            (inspector as LBSInspector).Repaint();
        }

        internal void SetTarget(LBSLayer layer)
        {
            foreach (var ve in VEs)
            {
                var inspector = ve.Value;
                inspector.SetTarget(layer);
            }
        }

        public void Repaint()
        {
            foreach (var ve in VEs)
            {
                var inspector = ve.Value;
                inspector.Repaint();
            }
        }
        #endregion

        #region FUNCTIONS SINGLETON
        public static void ShowInspector(string tab)
        {
            var panel = LBSInspectorPanel.Instance;
            panel.VEs.TryGetValue(tab, out var ve);
            panel.tabsGroup.ChangeActive(tab);
        }

        public static void ReDraw()
        {
            var panel = LBSInspectorPanel.Instance;
            panel.Repaint();
        }
        #endregion
    }
}