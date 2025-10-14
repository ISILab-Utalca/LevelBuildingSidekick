using System;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Editor.Windows;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Visual representation of a suggestion area inside the graph.
    /// Provides UI for applying or discarding quest suggestions.
    /// </summary>
    public sealed class SuggestionElementArea : GraphElement
    {
        #region Constants
        private const float GraphGridLength = 100;
        #endregion

        #region Fields
        private readonly BaseQuestNodeData _data;
        private readonly QuestNode _generatedQuestNode;

        private Button _applyButton;
        private Button _discardButton;
        private LBSToolbarToggle _visibleToggle; 
        
        private VisualElement _triggerElementGizmo;
        private StyleBackground _triggerBackground;

        private bool _resizing;
        private TriggerElementArea _suggestionArea;
        
        #endregion

        #region Constructor
        public SuggestionElementArea(QuestNode suggestion, Rect area)
        {
            _generatedQuestNode = suggestion;
            if (_generatedQuestNode is null) return;

            _data = _generatedQuestNode.NodeData;
            if (_data is null) return;

            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("SuggestionElementArea");
            visualTree.CloneTree(this);

            pickingMode = PickingMode.Ignore;
            
            AreaSetUp(area);
            CapsuleSetUp();

            SetSelected(false);
        
        }

        private void AreaSetUp(Rect area)
        {
            // Calculate visual position
            if(LBSMainWindow.Instance is null) return;
            if(LBSMainWindow.Instance._selectedLayer is null) return;
            var position = LBSMainWindow.Instance._selectedLayer.FixedToPosition(
                new Vector2Int((int)area.x, (int)area.y), true);

            var drawArea = new Rect(position,
                new Vector2(area.width * GraphGridLength, area.height * GraphGridLength));

            SetPosition(drawArea);

            // Main gizmo
            _triggerElementGizmo = this.Q<VisualElement>("TriggerElementSelector");
            _triggerElementGizmo.focusable = false;
            _triggerElementGizmo.pickingMode = PickingMode.Ignore;
            _triggerBackground = _triggerElementGizmo.style.backgroundImage;
            
            ApplyStyling();

            // Target icons
            var targetIcon = this.Q<VisualElement>("TargetIcon");
            targetIcon.style.backgroundImage = new StyleBackground(_data.GetIcon());
            targetIcon.style.display = DisplayStyle.None;

            var cornerTargetIcon = this.Q<VisualElement>("CornerTargetIcon");
            cornerTargetIcon.style.backgroundImage = new StyleBackground(_data.GetIcon());
            cornerTargetIcon.style.display = DisplayStyle.None;
        }

        private void CapsuleSetUp()
        {
            // Capsule hover handling
            var capsule = this.Q<VisualElement>("Capsule");
            capsule.RegisterCallback<MouseEnterEvent>(_ => SetSelected(true));
            capsule.RegisterCallback<MouseLeaveEvent>(_ => SetSelected(false));
     
            // Action label
            var actionLabel = this.Q<Label>("ActionLabel");
            if (!string.IsNullOrEmpty(_generatedQuestNode.QuestAction))
            {
                actionLabel.text = char.ToUpper(_generatedQuestNode.QuestAction[0]) +
                                   _generatedQuestNode.QuestAction.Substring(1);
            }

            // Buttons
            _applyButton = this.Q<Button>("ApplyButton");
            _discardButton = this.Q<Button>("DiscardButton");
            _visibleToggle= this.Q<LBSToolbarToggle>("VisibiliityToggle");
            _discardButton.clicked += () =>_generatedQuestNode.Graph.OnRemoveSuggestion?.Invoke(_generatedQuestNode);
            _applyButton.clicked += () =>
            {
                _generatedQuestNode.Graph.OnRemoveSuggestion?.Invoke(_generatedQuestNode);
                _generatedQuestNode.Graph.AddSuggestionNode(_generatedQuestNode);
            };
            _visibleToggle.RegisterCallback<ChangeEvent<bool>>(x =>
            {
                DisplayTriggerArea(x.newValue);
            });
            
           
            _applyButton.focusable = true;
            _discardButton.focusable = true;
            _visibleToggle.focusable = true;
            _visibleToggle.value = true;
                
            // Subscribe to geometry change (fires once layout finishes)
            EventCallback<GeometryChangedEvent> onGeometryReady = null;
            onGeometryReady = (evt) =>
            {
                capsule.UnregisterCallback(onGeometryReady);

                if (_generatedQuestNode.NodeViewPosition == Rect.zero)
                {
                    // add offset to capsule (previously set in the assistant generation function 
                    capsule.style.left = capsule.resolvedStyle.left + _generatedQuestNode.Position.x;
                    capsule.style.top = capsule.resolvedStyle.top - _generatedQuestNode.Position.y;

                    // offset by capsule size
                    var capsulePos = capsule.worldBound.position; 
                    capsulePos.x += capsule.resolvedStyle.width * 0.5f;
                    capsulePos.y += capsule.resolvedStyle.height * 0.5f;

                    var capsuleOffset = _generatedQuestNode.Graph.OwnerLayer.ToFixedPosition(capsulePos);
                    var graphPos = _generatedQuestNode.Graph.OwnerLayer.ToFixedPosition(GetPosition().position);
                    _generatedQuestNode.NodeViewPosition = new Rect(
                        graphPos,
                        new Vector2(capsule.resolvedStyle.width, capsule.resolvedStyle.height)
                    );
                }
            };

            // Register the callback
            capsule.RegisterCallback(onGeometryReady);

        }

        private void DisplayTriggerArea(bool display)
        {
            _triggerElementGizmo.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }

        #endregion

        #region Private Methods
        private void ApplyStyling()
        {
            Color backgroundColor = _data.Color;
            backgroundColor.a = 0.2f;
            _triggerElementGizmo.style.backgroundColor = backgroundColor;
            _triggerElementGizmo.style.unityBackgroundImageTintColor = backgroundColor;

            const float borderWidth = 4f;
            _triggerElementGizmo.style.borderBottomWidth = borderWidth;
            _triggerElementGizmo.style.borderLeftWidth = borderWidth;
            _triggerElementGizmo.style.borderRightWidth = borderWidth;
            _triggerElementGizmo.style.borderTopWidth = borderWidth;
        }

        private void SetSelected(bool isSelected)
        {
            _triggerElementGizmo.style.backgroundImage = isSelected ? _triggerBackground : null;
            
            Color backgroundColor = _data.Color;
            backgroundColor.a = isSelected ? 0.2f : 0f;
            _triggerElementGizmo.style.backgroundColor = backgroundColor;
            _triggerElementGizmo.style.unityBackgroundImageTintColor = backgroundColor;
            
            Color color = _data.Color;
            color.a = isSelected ? 1f : 0f;
            _triggerElementGizmo.style.borderBottomColor = color;
            _triggerElementGizmo.style.borderTopColor = color;
            _triggerElementGizmo.style.borderRightColor = color;
            _triggerElementGizmo.style.borderLeftColor = color;
        }
        #endregion
    }
}
