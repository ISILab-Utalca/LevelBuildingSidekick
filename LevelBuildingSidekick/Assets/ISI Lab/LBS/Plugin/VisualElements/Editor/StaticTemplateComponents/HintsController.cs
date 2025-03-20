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
        private Tab buttonTooltip;
        private Tab buttonTutorial;
        
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
            
            buttonTooltip = this.Q<Tab>("ButtonTooltip");
            buttonTooltip.RegisterCallback<ClickEvent>(evt =>
            {
                SetupTabButton(buttonTooltip, buttonTutorial, tooltipTab, tutorialTab);
            });
            
            buttonTutorial = this.Q<Tab>("ButtonTutorial");
            buttonTutorial.RegisterCallback<ClickEvent>(evt =>
            {
                SetupTabButton(buttonTutorial, buttonTooltip, tutorialTab, tooltipTab);
            });
            
            
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
            ChangeTab(button1);
            SetupTabButton(buttonTooltip, buttonTutorial, tooltipTab, tutorialTab);
            
        }
        
        #endregion

        #region METHODS
        private void ChangeTab(RadioButton selectedButton)
        {
            foreach (var entry in tabs)
            {
                entry.Value.style.display = entry.Key == selectedButton ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
        
        private void SetupTabButton(Tab button, Tab otherButton, VisualElement activeTab, VisualElement inactiveTab)
        {
            button.RegisterCallback<ClickEvent>(evt =>
            {
                if(!button.ClassListContains(activeClass)) button.AddToClassList(activeClass);
                if(otherButton.ClassListContains(activeClass)) otherButton.RemoveFromClassList(activeClass);

                activeTab.style.display = DisplayStyle.Flex;
                inactiveTab.style.display = DisplayStyle.None;
            });
        }
        
        #endregion
       
    }
}