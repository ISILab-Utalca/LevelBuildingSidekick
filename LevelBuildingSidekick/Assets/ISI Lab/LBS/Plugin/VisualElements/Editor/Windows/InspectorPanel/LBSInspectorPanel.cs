using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Template;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSInspectorPanel : VisualElement
    {
        #region FACTORY
        //public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }
        #endregion

        #region SINGLETON
        private static LBSInspectorPanel instance;
        public static LBSInspectorPanel Instance => instance;

        #endregion

        #region FIELDS
        private VisualElement content;
        private ButtonGroup tabsGroup;
        private string selectedTab;

        public LBSLocalCurrent data;
        public LBSLocalBehaviours behaviours;
        public LBSLocalAssistants assistants;

        public static string DataTab = "Layer data";
        public static string BehavioursTab = "Behaviours";
        public static string AssistantsTab = "Assistants";

        private static string recentTab;
        
        private Dictionary<string, LBSInspector> VEs = new();
        #endregion

        #region CONSTRUCTORS
        public LBSInspectorPanel()
        {
            instance = this;
            
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSInspectorPanel");
            visualTree.CloneTree(this);
            
            tabsGroup = this.Q<ButtonGroup>("SubTabs");
            content = this.Q<VisualElement>("InspectorContent");
        }
        #endregion

        #region METHODS
        
        /// <summary>
        /// Call after constructor in LBSMainWindow
        /// </summary>
        /// <param name="layers"></param>
        public void InitTabs(ref List<LayerTemplate> layers)
        {
            var layersList = layers.Select(t => t.layer).ToList();
            
            data = new LBSLocalCurrent();
            data.InitCustomEditors(ref layersList);
            AddTab(DataTab, data);

            behaviours = new LBSLocalBehaviours();
            behaviours.InitCustomEditors(ref layersList);
            AddTab(BehavioursTab, behaviours);

            assistants = new LBSLocalAssistants();
            assistants.InitCustomEditors(ref layersList);
            AddTab(AssistantsTab, assistants);
            
            tabsGroup.OnChangeTab += SetSelectedTab; 
            SetSelectedTab(DataTab);
        }

        private void AddTab(string tab, LBSInspector element)
        {
            VEs.Add(tab, element);
            SetContent(element);
   
            tabsGroup.AddChoice(tab, btn =>
            {
                var grupableBtn = btn as GrupalbeButton;
                ClearContent();
                VEs.TryGetValue(grupableBtn.text, out var inspct);
                SetContent(inspct);
            });
            
        }

        public void SetSelectedTab(string name)
        {
            ClearContent();

            if (VEs == null) return;
            if (string.IsNullOrEmpty(name)) return;
            if (!VEs.TryGetValue(name, out var ve))  return;
            if (ve == null) return;
            
            SetContent(ve);
        }


        private void ClearContent()
        {
            content.Clear();
        }

        private void SetContent(VisualElement inspector)
        {
            if (inspector == null) return;
            content?.Add(inspector);
        }

        internal void SetTarget(LBSLayer layer)
        {
            ToolKit.Instance.Clear();
            // updates the inspector panel locals and tools
            foreach (KeyValuePair<string, LBSInspector> ve in VEs)
            {
                // Update the inspector with the new layer data
                var inspector = ve.Value;
                inspector.SetTarget(layer);
            }
        }

        public void Repaint()
        {
            ToolKit.Instance.Clear();
            var currentManipulator = ToolKit.Instance.GetActiveManipulatorInstance();
            Type manipulatorClass = null;
            if (currentManipulator is not null)
            {
                manipulatorClass = currentManipulator.GetType();
            }
            foreach (var ve in VEs)
            {
                var inspector = ve.Value;
                inspector.Repaint();
            }
            ToolKit.Instance.SetSeparators();
            if(manipulatorClass is not null) ToolKit.Instance.SetActive(manipulatorClass);
        }
        #endregion

        #region FUNCTIONS SINGLETON
        private static void ShowInspector(string tab)
        {
            if (recentTab == tab) return;
            recentTab = tab; 
            
            var panel = Instance;
            panel.VEs.TryGetValue(tab, out var ve);
            if(ve == null) return;
           
            // Avoid reopening the same tab constantly
            ve.style.display = DisplayStyle.Flex;
            panel.tabsGroup.ChangeActive(tab);
            LBSMainWindow.InspectorToggleButtonChange(tab);
        }

        public static void ActivateBehaviourTab() { ShowInspector(BehavioursTab); }
        public static void ActivateAssistantTab() { ShowInspector(AssistantsTab); }
        public static void ActivateDataTab() { ShowInspector(DataTab); }
        public static void ReDraw()
        {
            Instance.Repaint();
        }
        #endregion

        /// <summary>
        ///  Called when reloading from the lbsmainwindow
        /// </summary>
        /// <param name="levelData"></param>
        /// <param name="mainView"></param>
        public void CreateContainers(LBSLevelData levelData, MainView mainView)
        {
            foreach (var layer in levelData.Layers)
            {
                if(layer == null) continue;
                mainView.AddContainer(layer);
            }
           
        }
    }
}