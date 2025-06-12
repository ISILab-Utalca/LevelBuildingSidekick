using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNode_Explore : NodeEditor
    {
        private IntegerField subareas;
        private Toggle useRandomPoint;
        private DataExplore currentData;

        private EventCallback<ChangeEvent<bool>> onToggleChanged;
        private EventCallback<ChangeEvent<int>> onSubareasChanged;

        public QuestNode_Explore()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Explore");
            visualTree.CloneTree(this);

            subareas = this.Q<IntegerField>("ExploreSubareas");
            useRandomPoint = this.Q<Toggle>("ExploreUseRandomPosition");

            useRandomPoint.value = false;
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if (data is not DataExplore de) return;
            currentData = de;
            
            // Unregister previous to avoid stacking calls
            if (onToggleChanged != null)
                useRandomPoint.UnregisterValueChangedCallback(onToggleChanged);
            if (onSubareasChanged != null)
                subareas.UnregisterValueChangedCallback(onSubareasChanged);
            
            SetToggleCallback();
            SetSubareaCallback();
            
            useRandomPoint.SetValueWithoutNotify(currentData.findRandomPosition);
            subareas.SetValueWithoutNotify(currentData.subdivisions);

            // must manually set the display type, because unity has no function via code that calls SetValue
            // thanks unity.
            subareas.style.display = currentData.findRandomPosition ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void SetSubareaCallback()
        {
            onSubareasChanged = evt =>
            {
                currentData.subdivisions = evt.newValue;
            };
            subareas.RegisterValueChangedCallback(onSubareasChanged);
        }

        private void SetToggleCallback()
        {
            onToggleChanged = evt =>
            {
                currentData.findRandomPosition = evt.newValue;
                subareas.style.display = evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
            };
            useRandomPoint.RegisterValueChangedCallback(onToggleChanged);
        }
    }
}
