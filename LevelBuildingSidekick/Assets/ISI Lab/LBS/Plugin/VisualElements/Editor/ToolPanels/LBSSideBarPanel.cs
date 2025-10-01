using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.CustomComponents.Events;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSSideBarPanel: VisualElement
    {
        private Toggle plusToggle;
        private Toggle layerToggle;
        private Toggle gen3DToggle;

        private Toggle layerDataTab;
        private Toggle assistantTab;
        private Toggle behaviorTab;
        
        private Toggle tagWindowButton;
        private Toggle bundleWindowButton;
        
        
        public LBSSideBarPanel(): base()
        {
            VisualTreeAsset visualTreeAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSSideBarPanel");
            visualTreeAsset.CloneTree(this);
            
            plusToggle =  this.Q<Toggle>("PlusToggle");
            layerToggle = this.Q<Toggle>("LayerToggle");
            gen3DToggle = this.Q<Toggle>("Gen3DToggle");
            
            layerDataTab = this.Q<Toggle>("LayerDataButton");
            assistantTab = this.Q<Toggle>("AssistantButton");
            behaviorTab = this.Q<Toggle>("BehaviorButton");
            
            tagWindowButton = this.Q<Toggle>("TagButton");
            bundleWindowButton = this.Q<Toggle>("BundlesButton");

            gen3DToggle.RegisterValueChangedCallback<bool>( _evt =>
            {
                LBSBoolEvent boolEvent = new LBSBoolEvent(_evt.target, _evt.newValue);
                this.SendEvent(boolEvent);
                _evt.StopPropagation();
            });
            
        }
    }
}
