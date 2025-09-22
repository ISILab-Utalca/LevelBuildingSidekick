using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS.VisualElements;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("QuestAssistant", typeof(QuestAssistant))]
    public class QuestAssistantEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS

        private const int SuggestAmount = 3;
        
        private QuestAssistant _questAssistant;
        private QuestBehaviour _questBehaviour;
        
        private QuestGraph _questGraph;
        
        private ListView _layerList;
        private ListView _suggestionList;
        private Button _addLayerButton;
        private Button _genSuggestionButton;
        private VisualElement _lockedContextEntryContainer;
        private LBSPanelTextIcon _noSuggestionPanel;

        #endregion

        #region PROPERTIES
        private LBSLevelData Data => _questGraph.OwnerLayer.Parent;
        #endregion

        #region CONSTRUCTORS
        public QuestAssistantEditor(QuestAssistant target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object paramTarget)
        {
            target = paramTarget as QuestAssistant;
            _questAssistant = target as QuestAssistant;
            _questGraph = _questAssistant?.OwnerLayer.GetModule<QuestGraph>();
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
            InitializeUIElements();
            SetupLayerContextList();
            SetupSuggestionList();
            SetupButtons();
            AddLockedLayer();
            return this;
        }

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

        private void InitializeUIElements()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestAssistantEditor");
            visualTree.CloneTree(this);
            
            _lockedContextEntryContainer = this.Q<VisualElement>("LockedLayerContainer");
            _layerList = this.Q<ListView>("LayerList");
            _suggestionList = this.Q<ListView>("SuggestionList");
            _addLayerButton = this.Q<Button>("AddLayerButton");
            _genSuggestionButton = this.Q<Button>("GenerateSuggestions");
            _noSuggestionPanel = this.Q<LBSPanelTextIcon>("NoSuggestionPanel");
        }

        private void SetupLayerContextList()
        {
            _layerList.reorderable = false;
            _layerList.makeItem = CreateLayerContextEntry;
            _layerList.bindItem = BindLayerContextEntry;
            _layerList.itemsSource = Data.ContextLayers;
            
            UpdateContextDisplay();
        }

        private void UpdateContextDisplay()
        {
            _layerList.Rebuild();
            bool existingContext = Data.ContextLayers.Count > 0;
            _layerList.style.display =  existingContext ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static VisualElement CreateLayerContextEntry()
        {
            return new LayerContextEntry();
        }

        private void BindLayerContextEntry(VisualElement element, int index)
        {
            if (element is not LayerContextEntry layerContextVe) return;

            layerContextVe.UpdateData(Data.ContextLayers[index]);
            layerContextVe.EvaluateOverlap(Data.ContextLayers);
            layerContextVe.OnRemoveButtonClicked = null;
            layerContextVe.OnRemoveButtonClicked += () =>
            {
                Data.ContextLayers.RemoveAt(index);
                _layerList.Remove(element);
                UpdateContextDisplay();
            };
        }

        private void SetupSuggestionList()
        {
            _suggestionList.reorderable = false;
            _suggestionList.makeItem = CreateQuestNodeSuggestion;
            _suggestionList.bindItem = BindQuestNodeSuggestion;
            _suggestionList.itemsSource = _questGraph.Suggestions;

            UpdateSuggestionsDisplay();
        }

        private void UpdateSuggestionsDisplay()
        {
            bool existingSuggestions = _suggestionList.itemsSource.Count > 0;
            _suggestionList.Rebuild();
            _noSuggestionPanel.style.display =  existingSuggestions ? DisplayStyle.None : DisplayStyle.Flex;
            _suggestionList.style.display =  existingSuggestions ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static VisualElement CreateQuestNodeSuggestion()
        {
            return new QuestNodeSuggestion();
        }

        private void BindQuestNodeSuggestion(VisualElement element, int index)
        {
            if (element is not QuestNodeSuggestion suggestionVe) return;

            suggestionVe.UpdateData(_questGraph.Suggestions[index]);
            suggestionVe.OnDiscard = null;
            suggestionVe.OnDiscard += () =>
            {
                _questGraph.Suggestions.RemoveAt(index);
                _suggestionList.Remove(element);
                _suggestionList.Rebuild();
            };
        }

        private void SetupButtons()
        {
            _addLayerButton.clicked += AddLayerMenu;
            _genSuggestionButton.clicked += GenerateSuggestions;
        }

        private void GenerateSuggestions()
        {
            for (int i = 0; i < SuggestAmount; i++)
            {
                foreach (var contextLayer in Data.ContextLayers)
                {
                   // Add AI behaviour LBSLayerHelper.GetObjectFromLayer<>(contextLayer);
                    PopulationBehaviour populationLayer = LBSLayerHelper.GetObjectFromLayer<PopulationBehaviour>(contextLayer);
                    if (populationLayer is not null)
                    {
                        bool data = SuggestInfoFromPopulation(populationLayer);
                    }
                    ExteriorBehaviour exteriorLayer = LBSLayerHelper.GetObjectFromLayer<ExteriorBehaviour>(contextLayer);
                    SchemaBehaviour interiorLayer = LBSLayerHelper.GetObjectFromLayer<SchemaBehaviour>(contextLayer);
                    
          
                }
             
            }
            UpdateSuggestionsDisplay();
            MarkDirtyRepaint();
        }

        private bool SuggestInfoFromPopulation(PopulationBehaviour populationLayer)
        {
            // In theory a layer can only have one of those behaviours as per templates
            foreach (var tileBundleGroup in populationLayer.Tilemap)
            {
                tileBundleGroup.BundleData.Bundle.GetHasTagCharacteristic()
            }

            return false;
        }

        #region LAYER CONTEXT METHODS

        private void AddLockedLayer()
        {
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(_questGraph.OwnerLayer);
            lockedLayer.SetEnabled(false);
            _lockedContextEntryContainer.Add(lockedLayer);
        }

        private void AddLayerMenu()
        {
            var menu = new GenericMenu();
            foreach (var layer in Data.Layers)
            {
                if (!_questGraph.OwnerLayer.Equals(layer))
                {
                    menu.AddItem(new GUIContent(layer.Name), Data.ContextLayers.Contains(layer), ToggleLayerContext, layer);
                }
            }
            menu.ShowAsContext();
        }

        private void ToggleLayerContext(object layer)
        {
            if (layer is not LBSLayer objectLayer)
            {
                Debug.LogError("Object Layer was null.");
                return;
            }

            if (Data.ContextLayers.Contains(objectLayer))
            {
                Data.ContextLayers.Remove(objectLayer);
            }
            else
            {
                Data.ContextLayers.Add(objectLayer);
            }
            UpdateSuggestionsDisplay();
        }
        #endregion

        public void SetTools(ToolKit toolkit)
        {
            // stub
        }
        #endregion
    }
}