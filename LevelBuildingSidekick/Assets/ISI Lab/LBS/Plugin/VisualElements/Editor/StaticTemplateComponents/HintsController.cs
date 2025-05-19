using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Generators;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    public class HintsController : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        // visual element's "tab" is not appropriate for the design, so just using its header with style change
        private VisualElement buttonTooltip;
        private VisualElement buttonTutorial;
        
        private VisualElement tooltipTab;
        private VisualElement tutorialTab;
        
        #endregion

        #region FIELDS

        private const string activeClass = "lbs-tutorial-tab-button-act";
        Dictionary<RadioButton, VisualElement> tabs = new();
        
        #endregion

        #region PROPERTIES
        
        #endregion

        #region CONSTRUCTORS
        public HintsController()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("HintsController");
            visualTree.CloneTree(this);
            
            var temp1 = this.Q<Tab>("ButtonTooltip");
            if (temp1 != null)
            {
                buttonTooltip = temp1.hierarchy.Children().First();
                temp1.RegisterCallback<ClickEvent>(evt =>
                {
                    SetupTabButton(buttonTooltip, buttonTutorial, tooltipTab, tutorialTab);
                });
            }
          
            
            var temp2 = this.Q<Tab>("ButtonTutorial");
            if (temp2 != null)
            {
                buttonTutorial = temp2.hierarchy.Children().First();
                temp2.RegisterCallback<ClickEvent>(evt =>
                {
                    SetupTabButton(buttonTutorial, buttonTooltip, tutorialTab, tooltipTab);
                });
            }
          
            
            
            tooltipTab = this.Q<VisualElement>("TooltipTab");
            tutorialTab = this.Q<VisualElement>("TutorialTab");
            
            var tooltipPage1 = this.Q<VisualElement>("TooltipPage1");
            var tooltipPage2 = this.Q<VisualElement>("TooltipPage2");
            var tooltipPage3 = this.Q<VisualElement>("TooltipPage3");
            var tooltipPage4 = this.Q<VisualElement>("TooltipPage4");
            
            var button1 = this.Q<RadioButton>("rb1");
            var button2 = this.Q<RadioButton>("rb2");
            var button3 = this.Q<RadioButton>("rb3");
            var button4 = this.Q<RadioButton>("rb4");
            
            tabs.Add(button1, tooltipPage1);
            tabs.Add(button2, tooltipPage2);
            tabs.Add(button3, tooltipPage3);
            tabs.Add(button4, tooltipPage4);

            foreach (var entry in tabs)
            {
                entry.Key.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue) ChangeTab(entry.Key);
                });
            }
            
            // By default on open
  
            SetupTabButton(buttonTooltip, buttonTutorial, tooltipTab, tutorialTab);
            ChangeTab(button1);
        }
        
        #endregion

        #region METHODS
        private void ChangeTab(RadioButton selectedButton)
        {
            foreach (var entry in tabs)
            {
                entry.Key.value = entry.Key == selectedButton ? true : false;
                entry.Value.style.display = entry.Key == selectedButton ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        
        private void SetupTabButton(VisualElement button, VisualElement otherButton, VisualElement activeTab, VisualElement inactiveTab)
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                var parent = button.hierarchy.parent;
                var parentOther = otherButton.hierarchy.parent;
                
                if(parent!=null && !parent.ClassListContains(activeClass)) parent.AddToClassList(activeClass);
                if(parentOther!=null && parentOther.ClassListContains(activeClass)) parentOther.RemoveFromClassList(activeClass);

                activeTab.style.display = DisplayStyle.Flex;
                inactiveTab.style.display = DisplayStyle.None;
            });
        }
        
        #endregion
       
    }
}