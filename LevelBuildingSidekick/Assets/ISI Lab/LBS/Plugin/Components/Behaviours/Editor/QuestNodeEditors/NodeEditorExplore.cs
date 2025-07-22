using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorExplore : NodeEditor
    {
        private readonly IntegerField _subareas;
        private readonly Toggle _useRandomPoint;
        private DataExplore _currentData;

        private EventCallback<ChangeEvent<bool>> _onToggleChanged;
        private EventCallback<ChangeEvent<int>> _onSubareasChanged;

        public NodeEditorExplore()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorExplore");
            visualTree.CloneTree(this);

            _subareas = this.Q<IntegerField>("ExploreSubareas");
            _useRandomPoint = this.Q<Toggle>("ExploreUseRandomPosition");

            _useRandomPoint.value = false;
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if (data is not DataExplore de) return;
            _currentData = de;
            
            // Unregister previous to avoid stacking calls
            if (_onToggleChanged != null)
                _useRandomPoint.UnregisterValueChangedCallback(_onToggleChanged);
            if (_onSubareasChanged != null)
                _subareas.UnregisterValueChangedCallback(_onSubareasChanged);
            
            SetToggleCallback();
            SetSubareaCallback();
            
            _useRandomPoint.SetValueWithoutNotify(_currentData.findRandomPosition);
            _subareas.SetValueWithoutNotify(_currentData.subdivisions);

            // must manually set the display type, because unity has no function via code that calls SetValue
            // thanks unity.
            _subareas.style.display = _currentData.findRandomPosition ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void SetSubareaCallback()
        {
            _onSubareasChanged = evt =>
            {
                _currentData.subdivisions = evt.newValue;
            };
            _subareas.RegisterValueChangedCallback(_onSubareasChanged);
        }

        private void SetToggleCallback()
        {
            _onToggleChanged = evt =>
            {
                _currentData.findRandomPosition = evt.newValue;
                _subareas.style.display = evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
            };
            _useRandomPoint.RegisterValueChangedCallback(_onToggleChanged);
        }
    }
}
