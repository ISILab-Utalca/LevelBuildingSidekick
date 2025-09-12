using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;
using ISILab.Macros;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("QuestAssistant", typeof(QuestAssistant))]
    public class QuestAssistantEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private QuestGraph _questGraph;
        private QuestAssistant _questAssistant;
        private QuestBehaviour _questBehaviour;

        #endregion

        #region VIEW
        //Layer Context
        private ListView layerList;
        private Button addLayerButton;
        private VisualElement lockedContextEntryContainer;
        private Button genSuggestionButton;

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

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestAssistantEditor");
            visualTree.CloneTree(this);
            
            //LAYER CONTEXT
            lockedContextEntryContainer = this.Q<VisualElement>("LockedLayerContainer");
            AddLockedLayer();
            
            layerList = this.Q<ListView>("LayerList");

            layerList.reorderable = false;
            layerList.makeItem += () => new LayerContextEntry();
            layerList.bindItem = (element, index) =>
            {
                if (element is not LayerContextEntry layerContextVE) return;
                
                layerContextVE.UpdateData(Data.ContextLayers[index]);
                layerContextVE.EvaluateOverlap(Data.ContextLayers);
                layerContextVE.OnRemoveButtonClicked = null;
                layerContextVE.OnRemoveButtonClicked += () =>
                {
                    Data.ContextLayers.RemoveAt(index);
                    layerList.Remove(element);
                    layerList.Rebuild();
                };
            };
            
            layerList.itemsSource = Data.ContextLayers;

            addLayerButton = this.Q<Button>("AddLayerButton");
            addLayerButton.clicked += AddLayerMenu;
            
            genSuggestionButton = this.Q<Button>("GenerateSuggestions");
            genSuggestionButton.clicked += GenSuggestions;
            
            return this;
        }

        private void GenSuggestions()
        {
            throw new System.NotImplementedException();
        }

        #region LAYER CONTEXT METHODS
        public void AddLockedLayer()
        {
            //Add the layer to Layer context
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(_questGraph.OwnerLayer);
            lockedLayer.SetEnabled(false);
            lockedContextEntryContainer.Add(lockedLayer);
        }

        public void AddLayerMenu()
        {
            GenericMenu menu = new GenericMenu();
            foreach(LBSLayer layer in Data.Layers)
            {
                //The layer the assistant is working on can't be used as context, since its content is overwritten.
                if (!_questGraph.OwnerLayer.Equals(layer))
                { 
                    menu.AddItem(new GUIContent(layer.Name), Data.ContextLayers.Contains(layer), ToggleLayerContext, layer); 
                }
            }
            menu.ShowAsContext();
        }
        
        private void ToggleLayerContext(object layer)
        {
            LBSLayer objectLayer = layer as LBSLayer;
            if (objectLayer == null)
            {
                Debug.LogError("Object Layer was null.");
                return;
            }
            switch(Data.ContextLayers.Contains(layer))
            {
                case true: Data.ContextLayers.Remove(objectLayer); break;
                case false: Data.ContextLayers.Add(objectLayer); break;
            }
            layerList.Rebuild();

        }
        #endregion

        public void SetTools(ToolKit toolkit)
        {
            // stub
        }
        
        #endregion
    }
}
