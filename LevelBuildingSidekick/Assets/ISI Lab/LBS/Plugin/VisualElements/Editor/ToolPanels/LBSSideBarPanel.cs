using System;
using System.Collections.Generic;
using ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager;
using ISILab.Commons.Utility.Editor;
using UnityEngine.UIElements;
using ISILab.LBS.CustomComponents.Events;
using ISILab.LBS.Editor.Windows;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSSideBarPanel: VisualElement
    {
        private Toggle layerToggle;
        private Toggle gen3DToggle;
        private Toggle plusToggle;

        
        private static List<Toggle> inspectorToggleTabs = new();
        public Toggle layerDataTab;
        public Toggle assistantTab;
        public Toggle behaviorTab;
        
        private Toggle tagWindowButton;
        private Toggle bundleWindowButton;
        
        private VisualElement toggleButtonsGroup;
        
        #region EVENTS
        public LBSBoolEvent toggleEvent; //Experimental!
        #endregion
        
        #region ACTION EVENTS
        
        public event Action<ChangeEvent<bool>> OnTogglePressed;
        
        #endregion
        
        public LBSSideBarPanel(): base()
        {
            
            VisualTreeAsset visualTreeAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSSideBarPanel");
            visualTreeAsset.CloneTree(this);
            this.name = "LBSSideBarPanel";
            
            layerToggle = this.Q<Toggle>("LayerToggle");
            gen3DToggle = this.Q<Toggle>("Gen3DToggle");
            plusToggle =  this.Q<Toggle>("PlusToggle");
            
            
            layerDataTab = this.Q<Toggle>("LayerDataButton");
            assistantTab = this.Q<Toggle>("AssistantButton");
            behaviorTab = this.Q<Toggle>("BehaviorButton");
            inspectorToggleTabs.Clear();
            inspectorToggleTabs.Add(layerDataTab);
            inspectorToggleTabs.Add(assistantTab);
            inspectorToggleTabs.Add(behaviorTab);
            
            tagWindowButton = this.Q<Toggle>("TagButton");
            bundleWindowButton = this.Q<Toggle>("BundlesButton");
            
            toggleButtonsGroup = this.Q<VisualElement>("ToggleButtonContainer");

            // gen3DToggle.RegisterValueChangedCallback<bool>( _evt =>
            // {
            //     //toggleEvent = new LBSBoolEvent(_evt.target, _evt.newValue);
            //     //this.SendEvent(toggleEvent);
            //     OnTogglePressed?.Invoke(_evt);
            //     _evt.StopPropagation();
            // });
            
        }

        public void Bind(LBSMainWindow _mainWindow){
            if (_mainWindow != null)
            {
                layerToggle?.SetValueWithoutNotify(true);
                layerToggle?.RegisterCallback<ChangeEvent<bool>>(_evt =>
                {
                    _mainWindow.layerPanel.style.display = _evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                });

                gen3DToggle?.SetValueWithoutNotify(false);
                gen3DToggle?.RegisterCallback<ChangeEvent<bool>>(_evt =>
                {
                    _mainWindow.gen3DPanel.Init(_mainWindow._selectedLayer);
                    _mainWindow.gen3DPanel.style.display = _evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                });

                layerDataTab.RegisterCallback<ClickEvent>(_ => _mainWindow.ChangeInspectorPanelTab(layerDataTab));
                assistantTab.RegisterCallback<ClickEvent>(_ => _mainWindow.ChangeInspectorPanelTab(assistantTab));
                behaviorTab.RegisterCallback<ClickEvent>(_ => _mainWindow.ChangeInspectorPanelTab(behaviorTab));

                tagWindowButton?.RegisterCallback<ClickEvent>(_ =>
                {
                    OnToggleButtonClick();
                    tagWindowButton.SetValueWithoutNotify(true);
                });
                
                bundleWindowButton.RegisterCallback<ClickEvent>(_ =>
                {
                    OnToggleButtonClick();
                    BundleManagerWindow.ShowWindow();
                    bundleWindowButton.SetValueWithoutNotify(true);
                });
                
            }
        }
        
        
        //TODO: Change this behavior for tabs system.
        public void OnToggleButtonClick()
        {
            foreach (var toggleTab in inspectorToggleTabs)
            {
                if (toggleTab is Toggle toggle)
                {
                    toggle.SetValueWithoutNotify(false); // Deselect
                }
            }
        }
        
    }
}
