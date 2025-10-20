using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Template;
using LBS.Components;
using ISILab.LBS.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class LayersPanel : VisualElement
    {
        #region FIELDS
        public readonly LBSLevelData Data;
        private LBSLayer _selectedLayer;
        private readonly List<LayerTemplate> _templates;

        private ListView _list;
        private TextField _nameField;
        private VisualElement _noLayerNotifications;
        private VisualElement _noSelectedLayerNotificator;

        private readonly HashSet<LayerView> _layerViews = new HashSet<LayerView>();
        
        [SerializeField]
        private Toggle _toggleFocus;
        
        private const float UnfocusOpacity = 0.33f;
        private readonly List<int> _dragAffected = new();
       

        #endregion

        #region EVENTS
        public event Action<LBSLayer> OnAddLayer;
        public event Action<LBSLayer> OnRemoveLayer;
        public event Action<LBSLayer> OnSelectLayer;
        public event Action<LBSLayer> OnDoubleSelectLayer;
        public event Action<LBSLayer> OnLayerVisibilityChange;
        public event Action<LBSLayer> OnLayerOrderChange;
        #endregion

        #region CONSTRUCTOR
        public LayersPanel(){}
        
        public LayersPanel(LBSLevelData data, ref List<LayerTemplate> templates)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayersPanel");
            visualTree.CloneTree(this);

            Data = data;
            _templates = templates;

            RegisterEvents();
            InitializeUI();
            RefreshUI();
        }
        #endregion

        #region METHODS

        #region INITIALIZATION
        private void RegisterEvents()
        {
            OnAddLayer          += HandleLayerChangeEvent;
            OnRemoveLayer       += HandleLayerChangeEvent;
            OnSelectLayer       += HandleSelectLayerEvent;
            OnLayerOrderChange  += HandleLayerChangeEvent;
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void InitializeUI()
        {
            _list = this.Q<ListView>("List");
            _list.itemsSource = Data.Layers;
            
            _list.makeItem = () => new LayerView();
            _list.bindItem = BindListItem;
            _list.itemsChosen += ItemChosen;
            _list.selectionChanged += SelectionChanged;
            _list.itemIndexChanged += OnItemDrag;

            _nameField = this.Q<TextField>("NameField");

            SetupAddLayerButton();
            SetupRemoveLayerButton();
            SetupFocusToggle();

            _noLayerNotifications = this.Q<VisualElement>("NoLayerNotify");
            _noSelectedLayerNotificator = this.Q<VisualElement>("NoSelectedLayerNotify");
        }

        private void SetupAddLayerButton()
        {
            var addLayerButton = this.Q<ToolbarMenu>("AddLayerButtonMenu");

            for (int i = 0; i < _templates.Count; i++)
            {
                int index = i;
                addLayerButton.menu.AppendAction(_templates[i].templateName, _ => AddLayerByTemplate(index));
            }
        }

        private void SetupRemoveLayerButton()
        {
            var removeSelectedBtn = this.Q<Button>("RemoveSelectedButton");
            removeSelectedBtn.clicked += RemoveSelectedLayer;
        }

        private void SetupFocusToggle()
        {
            _toggleFocus = this.Q<Toggle>("ToggleFocusButton");
            _toggleFocus.RegisterValueChangedCallback(OnToggleFocusChanged);
        }
        #endregion

        #region LISTVIEW HANDLERS
        private void BindListItem(VisualElement item, int index)
        {
            //Debug.Log("Bind List Item [Layers Panel]");
            if (index >= Data.LayerCount)
            {
                _dragAffected.Remove(index);
                return;
            }

            if (item is not LayerView view) return;

            bool last = index == Data.LayerCount - 1;

            var layer = Data.GetLayer(index);
            _layerViews.Add(view);
            layer.index = _list.childCount - index;

            if (_dragAffected.Count == 0)
            {
                ResetLayerViewEvents(view, layer, last);
            }
            else
            {
                _dragAffected.Remove(index);
                if (_dragAffected.Count == 0)
                {
                    OnLayerOrderChange?.Invoke(layer);
                }
            }
            view.UpdateSelect(GetSelectedLayer());
            view.SetStyleSelectors();
            CheckOpacity();
        }

        private void ResetLayerViewEvents(LayerView view, LBSLayer layer, bool last)
        {
            if (view.OnLayerVisibilityChangeAction != null)
                view.OnVisibilityChange -= view.OnLayerVisibilityChangeAction;

            view.OnLayerVisibilityChangeAction = () => OnLayerVisibilityChange?.Invoke(layer);
            view.OnVisibilityChange += view.OnLayerVisibilityChangeAction;
            view.SetInfo(layer);
            view.OnNameChange += layer.InvokeNameChanged;
            // OnLayerOrderChange -= view.UpdateSelect;
            // OnLayerOrderChange += view.UpdateSelect;
            CheckOpacity();
    }

        private void SelectionChanged(IEnumerable<object> objs)
        {
            //Debug.Log("LIST SELECTION CHANGED");
            var selected = objs.FirstOrDefault() as LBSLayer;
            LBSMainWindow.Instance._selectedLayer = selected;
            OnSelectLayer?.Invoke(GetSelectedLayer());
        }

        private void ItemChosen(IEnumerable<object> objs)
        {
            var selected = objs.FirstOrDefault() as LBSLayer;
            LBSMainWindow.Instance._selectedLayer = selected;
            OnDoubleSelectLayer?.Invoke(GetSelectedLayer());
        }

        private void OnItemDrag(int oldIndex, int newIndex)
        {
            Debug.Log("On Item Drag");
            int count = Mathf.Abs(newIndex - oldIndex) + 1;
            int step = (int)Mathf.Sign(newIndex - oldIndex);
            for (int i = 0; i < count; i++)
            {
                int index = oldIndex + i * step;
                _dragAffected.Add(index);
            }
            RefreshUI();
        }
        #endregion
        
        #region EVENT HANDLERS
        private void UpdateListChildItems(LBSLayer layer)
        {
            _layerViews.Clear();
            _list.RefreshItems(); // calls rebind -> refill _layerViews
            foreach (var layerView in _layerViews)
            {
                layerView.UpdateSelect(layer, IsFocusToggleOn());
            }
        }
        
        #endregion


        #region LAYER MANAGEMENT
        private void AddLayerByTemplate(int index)
        {
            if (index < 0)
            {
                Debug.LogWarning("No layer type selected.");
                return;
            }

            if (_templates[index].layer.Clone() is not LBSLayer layer) return;
            layer.Name = LBSSettings.Instance.general.baseLayerName;
            AddLayer(layer);
        }

        private void AddLayer(LBSLayer layer)
        {
            layer.Name = GenerateUniqueLayerName(layer.Name);

            Data.AddLayer(layer);
            _list.SetSelection(new List<int> { 0 }); // Aca se invoca OnSelectLayer
            OnAddLayer?.Invoke(layer);
            SetSelectedLayer(layer); // Aca tambien se invoca OnSelectLayer, seria bueno unificarlo de alguna forma para que se llame solo una vez en los casos que corresponda

            LBSMainWindow.MessageNotify("New Data layer created");
            _list.Rebuild();

            foreach (var layerView in _layerViews)
            {
                if (Equals(layerView.Target, layer))
                {
                    layerView.UpdateSelect(layer, IsFocusToggleOn());
                }
            }
            
            CheckOpacity();
        }

        private string GenerateUniqueLayerName(string baseName)
        {
            int i = 1;
            string newName = baseName;

            while (Data.Layers.Any(l => l.Name.Equals(newName)))
            {
                newName = $"{baseName} {i}"; 
                i++;
            }

            return newName;
        }


        private void RemoveSelectedLayer()
        {
            if (!Data.Layers.Any()) return;
            var index = _list.selectedIndex;
            if (index < 0) return;

            if (!EditorUtility.DisplayDialog("Caution",
                "You are about to delete a layer. Are you sure?",
                "Continue", "Cancel")) return;

            var removedLayer = Data.RemoveAt(index);
            removedLayer.RemoveAll();
            DrawManager.Instance.RemoveContainer(removedLayer);
            OnRemoveLayer?.Invoke(removedLayer);
            _list.Rebuild();

            SetSelectedLayer(GetNextLayerAfterRemoval(index));
            LBSMainWindow.MessageNotify("Data layer deleted");
        }

        private LBSLayer GetNextLayerAfterRemoval(int removedIndex)
        {
            if (Data.LayerCount <= 0) return null;
            int nextIndex = Mathf.Clamp(removedIndex, 0, Data.LayerCount - 1);
            return Data.GetLayer(nextIndex);
        }
        #endregion

        #region FOCUS MANAGEMENT
        private void OnToggleFocusChanged(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
            {
                DrawManager.Instance.ChangeOpacityAll(1f);
            }
            OnSelectLayer?.Invoke(GetSelectedLayer());
        }

        private void CheckOpacity()
        {
            if (DrawManager.Instance is null) return;

            var selectedLayer = GetSelectedLayer();
            if (IsFocusToggleOn() && selectedLayer != null)
            {
                DrawManager.Instance.ChangeOpacityAll(UnfocusOpacity);
                DrawManager.ChangeLayerOpacity(selectedLayer, 1f);
            }
        }

        private bool IsFocusToggleOn()
        {
            return _toggleFocus.value;
        }

        #endregion

        #region SELECTION MANAGEMENT
        private void SetSelectedLayer(LBSLayer layer)
        {
            //Debug.Log("SET SELECTED LAYER");
            _selectedLayer = layer;
            OnSelectLayer?.Invoke(layer);
        }

        private LBSLayer GetSelectedLayer() => LBSMainWindow.Instance._selectedLayer;

        public void ResetSelection()
        {
            _list.ClearSelection();
            SetSelectedLayer(null);
   
        }

        private void UpdateNoSelectedLayer()
        {
            var layer = GetSelectedLayer();
            if (layer != null)
            {
                LBSInspectorPanel.ActivateDataTab();
                _noSelectedLayerNotificator.style.display = DisplayStyle.None;
            }
            else
            {
                _noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                LBSInspectorPanel.Instance.SetSelectedTab(null);
            }
        }
        #endregion

        #region UI UPDATES
        private void HandleLayerChangeEvent(LBSLayer _) => RefreshUI();

        private void HandleSelectLayerEvent(LBSLayer layer)
        {
            foreach (var layerView in _layerViews)
            {
                layerView.UpdateSelect(layer, IsFocusToggleOn());
            }
            CheckOpacity();
        }

        private void RefreshUI()
        {
            UpdateNoLayerPanel();
            UpdateNoSelectedLayer();
            UpdateDisplayList();
            UpdateListChildItems(GetSelectedLayer());
            CheckOpacity();
        }

        private void UpdateNoLayerPanel()
        {
            bool noItems = _list.itemsSource.Count <= 0;
            _noLayerNotifications.style.display = noItems ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateDisplayList()
        {
            bool hasItems = _list.itemsSource.Count > 0;
            _list.style.display = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
        }
        #endregion

        #region KEYBOARD HANDLING
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                int preIndex = _list.selectedIndex;
                RemoveSelectedLayer();
                if (Data.LayerCount > 0)
                    SetSelectedLayer(GetNextLayerAfterRemoval(preIndex));
                evt.StopPropagation();
            }

            if (evt.ctrlKey && evt.keyCode == KeyCode.D && _selectedLayer != null)
            {
                AddLayer(_selectedLayer.Clone() as LBSLayer);
            }
        }
        #endregion

        #endregion
    }
}
