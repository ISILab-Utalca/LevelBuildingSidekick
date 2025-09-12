using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
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
        private QuestAssistant _questAssistant;
        private QuestBehaviour _questBehaviour;
        
        private QuestGraph _questGraph;
        
        private ListView _layerList;
        private ListView _suggestionList;
        private Button _addLayerButton;
        private Button _genSuggestionButton;
        private VisualElement _lockedContextEntryContainer;
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
            SetupLayerList();
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
        }

        private void SetupLayerList()
        {
            _layerList.reorderable = false;
            _layerList.makeItem = CreateLayerContextEntry;
            _layerList.bindItem = BindLayerContextEntry;
        }

        private VisualElement CreateLayerContextEntry()
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
                _layerList.Rebuild();
            };
        }

        private void SetupSuggestionList()
        {
            _suggestionList.reorderable = false;
            _suggestionList.makeItem = CreateQuestNodeSuggestion;
            _suggestionList.bindItem = BindQuestNodeSuggestion;
            _suggestionList.itemsSource = _questGraph.Suggestions;
        }

        private VisualElement CreateQuestNodeSuggestion()
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
            for (int i = 0; i < 3; i++)
            {
                var newNode = _questGraph.AddSuggestion(_questGraph.Grammar.TerminalActions.Random(), Vector2.zero);
                _questGraph.Suggestions.Add(newNode);
            }
        }

        #region LAYER CONTEXT METHODS
        public void AddLockedLayer()
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
            _layerList.Rebuild();
        }
        #endregion

        public void SetTools(ToolKit toolkit)
        {
            // stub
        }
        #endregion
    }
}