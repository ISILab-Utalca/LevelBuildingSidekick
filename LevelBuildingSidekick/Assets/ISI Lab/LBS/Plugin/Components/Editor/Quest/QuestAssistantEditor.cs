using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.VisualElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("QuestAssistant", typeof(QuestAssistant))]
    public class QuestAssistantEditor : LBSCustomEditor, IToolProvider
    {
        
        #region FIELDS
        // Boolean to use editor for debugging purpuses
        private const bool Debugging = false;
        private const uint DefaultSuggestionValue = 3;
        
        private static class UIElementNames
        {
            public const string VisualTree = "QuestAssistantEditor";
            public const string LockedLayerContainer = "LockedLayerContainer";
            public const string LayerList = "LayerList";
            public const string SuggestionList = "SuggestionList";
            public const string AddLayerButton = "AddLayerButton";
            public const string GenerateSuggestionsButton = "GenerateSuggestions";
            public const string ConnectAll = "ConnectAll";
            public const string SuggestionField = "SuggestionField";
            public const string NoSuggestionPanel = "NoSuggestionPanel";
        }

        private QuestAssistant _questAssistant;
        private QuestGraph _questGraph;
        private ListView _layerList;
        private ListView _suggestionList;
        private Button _addLayerButton;
        private Button _autoConnectButton;
        private Button _generateSuggestionsButton;
        private VisualElement _lockedContextEntryContainer;
        private LBSPanelTextIcon _noSuggestionPanel;
        private LBSCustomUnsignedIntegerField _suggestionField;
        
        #endregion

        #region CONSTRUCTORS
        public QuestAssistantEditor(QuestAssistant target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object target)
        {
            this.target = target as QuestAssistant;
            _questAssistant = target as QuestAssistant;
            _questGraph = _questAssistant?.OwnerLayer.GetModule<QuestGraph>();
        }

        /// <summary>
        /// Creates and configures the visual elements for the editor.
        /// </summary>
        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>(UIElementNames.VisualTree);
            visualTree.CloneTree(this);

            _lockedContextEntryContainer = this.Q<VisualElement>(UIElementNames.LockedLayerContainer);
            _layerList = this.Q<ListView>(UIElementNames.LayerList);
            _suggestionList = this.Q<ListView>(UIElementNames.SuggestionList);
            _addLayerButton = this.Q<Button>(UIElementNames.AddLayerButton);
            _generateSuggestionsButton = this.Q<Button>(UIElementNames.GenerateSuggestionsButton);
            _autoConnectButton = this.Q<Button>(UIElementNames.ConnectAll);
            _noSuggestionPanel = this.Q<LBSPanelTextIcon>(UIElementNames.NoSuggestionPanel);
            _suggestionField = this.Q<LBSCustomUnsignedIntegerField>(UIElementNames.SuggestionField);
            _suggestionField.value = DefaultSuggestionValue;
            
            _addLayerButton.clicked += ShowAddLayerMenu;
            if (Debugging)
            {
                _generateSuggestionsButton.clicked += () =>
                {
                    _questAssistant.GenerateRandomNodes((int)GetSuggestionCount());
                };
                _autoConnectButton.clicked += _questAssistant.ConnectAllNodes;
                _autoConnectButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                _generateSuggestionsButton.clicked += ()=>
                {
                    _questAssistant.GenerateSuggestions((int)GetSuggestionCount());
                    UpdateSuggestionsDisplay();
                };
                _autoConnectButton.style.display = DisplayStyle.None;
            }
            
            SetupLayerContextList();
            SetupSuggestionList();
            AddLockedLayer();
            return this;
        }
        
        public uint GetSuggestionCount()
        {
            return _suggestionField.value;
        }

        #region LAYERS
        private void SetupLayerContextList()
        {
            _layerList.reorderable = false;
            _layerList.makeItem = () => new LayerContextEntry();
            _layerList.bindItem = BindLayerContextEntry;
            _layerList.itemsSource = _questAssistant.Data.ContextLayers;
            UpdateContextDisplay();
        }
        
        private void UpdateContextDisplay()
        {
            _layerList.Rebuild();
            _layerList.style.display = _questAssistant.Data.ContextLayers.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void BindLayerContextEntry(VisualElement element, int index)
        {
            if (element is not LayerContextEntry layerContextEntry) return;

            layerContextEntry.UpdateData(_questAssistant.Data.ContextLayers[index]);
            layerContextEntry.EvaluateOverlap(_questAssistant.Data.ContextLayers);
            layerContextEntry.OnRemoveButtonClicked = null;
            layerContextEntry.OnRemoveButtonClicked += () =>
            {
                _questAssistant.Data.ContextLayers.RemoveAt(index);
                UpdateContextDisplay();
            };
        }
        
        private void AddLockedLayer()
        {
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(_questGraph.OwnerLayer);
            lockedLayer.SetEnabled(false);
            _lockedContextEntryContainer.Add(lockedLayer);
        }
        
        private void ShowAddLayerMenu()
        {
            var menu = new GenericMenu();
            foreach (var layer in _questAssistant.Data.Layers)
            {
                if (!_questGraph.OwnerLayer.Equals(layer))
                {
                    menu.AddItem(new GUIContent(layer.Name), _questAssistant.Data.ContextLayers.Contains(layer), ToggleLayerContext, layer);
                }
            }
            menu.ShowAsContext();
        }
        
        private void ToggleLayerContext(object layer)
        {
            if (layer is not LBSLayer lbsLayer)
            {
                Debug.LogError("Invalid layer object.");
                return;
            }

            if (_questAssistant.Data.ContextLayers.Contains(lbsLayer))
                _questAssistant.Data.ContextLayers.Remove(lbsLayer);
            else
                _questAssistant.Data.ContextLayers.Add(lbsLayer);

            UpdateContextDisplay();
        }
        #endregion

        #region SUGGESTIONS
        private void SetupSuggestionList()
        {
            _suggestionList.reorderable = false;
            _suggestionList.makeItem = () => new QuestNodeSuggestion();
            _suggestionList.bindItem = BindQuestNodeSuggestion;
            _suggestionList.itemsSource = _questGraph.Suggestions;
            UpdateSuggestionsDisplay();
        }
        
        private void UpdateSuggestionsDisplay()
        {
            bool hasSuggestions = _suggestionList.itemsSource.Count > 0;
            _suggestionList.Rebuild();
            _noSuggestionPanel.style.display = hasSuggestions ? DisplayStyle.None : DisplayStyle.Flex;
            _suggestionList.style.display = hasSuggestions ? DisplayStyle.Flex : DisplayStyle.None;
            MarkDirtyRepaint();
        }
        
        private void BindQuestNodeSuggestion(VisualElement element, int index)
        {
            if (element is not QuestNodeSuggestion suggestionEntry) return;

            suggestionEntry.UpdateData(_questGraph.Suggestions[index]);
            suggestionEntry.OnDiscard = null;
            suggestionEntry.OnDiscard += () =>
            {
                _questGraph.Suggestions.RemoveAt(index);
                UpdateSuggestionsDisplay();
            };
        }
        
        #endregion
        
        public override void OnFocus()
        {
            _questGraph.displaySuggestions = true;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public override void OnUnfocus()
        {
            _questGraph.displaySuggestions = false;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public void SetTools(ToolKit toolkit) { }
        #endregion
    }
}