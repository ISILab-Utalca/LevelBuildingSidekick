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
using Debug = UnityEngine.Debug;


namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class LayersPanel : VisualElement
    {
        
        #region FACTORY
        //public new class UxmlFactory: UxmlFactory<LayersPanel, VisualElement.UxmlTraits> { }
        #endregion

        #region FIELDS
        public LBSLevelData data;
        private LBSLayer selectedLayer;
        
        // templates
        private List<LayerTemplate> templates;
        #endregion

        #region FIELD VIEW
        private ListView list;
        private TextField nameField;
        private List<VisualElement> noLayerNotificators; 

        private VisualElement noSelectedLayerNotificator;
        private VisualElement layerSettings;
        #endregion

        #region EVENTS ACTIONS
        public event Action<LBSLayer> OnAddLayer;
        public event Action<LBSLayer> OnRemoveLayer;
        public event Action<LBSLayer> OnSelectLayer; // click simple (!)
        public event Action<LBSLayer> OnDoubleSelectLayer; // double click (!)
        public event Action<LBSLayer> OnLayerVisibilityChange;
        #endregion

        #region CONSTRUCTORS
        public LayersPanel() {  }

        public LayersPanel(LBSLevelData data, ref List<LayerTemplate> templates)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayersPanel");
            visualTree.CloneTree(this);

            this.data = data;
            this.templates = templates;

            //Event self conection
            OnAddLayer += OnLayerChangeEventHandle;
            OnRemoveLayer += OnLayerChangeEventHandle;
            OnSelectLayer += OnLayerSelectedEventHandle;

            // LayerList
            list = this.Q<ListView>("List");

            VisualElement MakeItem()
            {
                return new LayerView();
            }

            list.bindItem += (item, index) =>
            {
                if (index >= this.data.LayerCount)
                    return;

                var view = item as LayerView;
                var layer = this.data.GetLayer(index);
                if (view == null) return;
                view.SetInfo(layer);
                view.OnVisibilityChange += () => { OnLayerVisibilityChange(layer); };
            };

            // list configuration
            list.fixedItemHeight = 24;
            list.itemsSource = data.Layers;
            list.makeItem += MakeItem;
            list.itemsChosen += ItemChosen;
            list.selectionChanged += SelectionChange;

            // NameField
            nameField = this.Q<TextField>("NameField");

            // TypeDropdown
            var typeDropdown1 = this.Q<DropdownField>("TypeDropdown");
            typeDropdown1.choices = templates.Select(t => t.name).ToList();
            typeDropdown1.index = 0;

            //Add Layer Button Menu
            var addButtonMenu1 = this.Q<ToolbarMenu>("AddLayerButtonMenu");
            for(int i = 0; i < templates.Count; i++)
            {
                int x = i;
                addButtonMenu1.menu.AppendAction(templates[i].name, dma => AddLayer(x));
            }

            // AddLayerButton
            // var addLayerBtn = this.Q<Button>("AddLayerButton");
            // addLayerBtn.clicked += AddLayer;

            // RemoveSelectedButton
            Button RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
            RemoveSelectedBtn.clicked += RemoveSelectedLayer;

            noLayerNotificators = this.Query<VisualElement>("NoLayerNotify").ToList();
            noSelectedLayerNotificator = this.Q<VisualElement>("NoSelectedLayerNotify");
            layerSettings = this.Q<VisualElement>("LayerSettings");
            
            list.style.display = DisplayStyle.None;
            noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
            
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            OnLayerChangeEventHandle(null);
        }
        #endregion

        #region METHODS
        private LBSLayer CreateLayer(int index)
        {
            var layers = templates.Select(t => t.layer).ToList();
            return layers[index].Clone() as LBSLayer;
        }

        private void AddLayer(int index)
        {
            if (index < 0)
            {
                Debug.LogWarning("No layer type has been selected yet, make sure to select one.");
                return;
            }

            var layer = CreateLayer(index);

            layer.Name = LBSSettings.Instance.general.baseLayerName;

            int i = 1;
            while (data.Layers.Any(l => l.Name.Equals(layer.Name)))
            {
                layer.Name = nameField.text + " " + i;
                i++;
            }

            data.AddLayer(layer);
            
            list.SetSelectionWithoutNotify(new List<int>() {0});

            OnAddLayer?.Invoke(layer);

            LBSMainWindow.MessageNotify("New Data layer created");
            list.Rebuild();
        }

        private void RemoveSelectedLayer()
        {
            if (data.Layers.Count <= 0)
                return;

            var index = list.selectedIndex;
            if (index < 0)
                return;

            var answer = EditorUtility.DisplayDialog("Caution",
            "You are about to delete a layer. If you proceed with this action, all of its" +
            " content will be permanently removed, and you won't be able to recover it. Are" +
            " you sure you want to continue?", "Continue", "Cancel");

            if (!answer)
                return;

            var layer = data.RemoveAt(index);

            //Select a new layer
            if(data.LayerCount != 0)
            {
                if (index >= data.LayerCount) list.selectedIndex--;
                else OnSelectLayer(data.GetLayer(list.selectedIndex));
            }

            OnRemoveLayer?.Invoke(layer);
            list.Rebuild();

            LBSMainWindow.MessageNotify("Data layer deleted");
            DrawManager.ReDraw();
        }

        // Simple Click over an element
        private void SelectionChange(IEnumerable<object> objs)
        {
            if (!objs.Any()) {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                return;
            }

            var selected = objs.ToList()[0] as LBSLayer;
            OnSelectLayer?.Invoke(selected);
            
        }
        
        // Double Click over an element
        private void ItemChosen(IEnumerable<object> objs)
        {
            if (!objs.Any())
                return;

            var selected = objs.ToList()[0] as LBSLayer;
            OnDoubleSelectLayer?.Invoke(selected);
        }

        public void ResetSelection()
        {
            list.ClearSelection();
            //list.RemoveFromSelection(list.selectedIndex);
        }

        private void OnLayerChangeEventHandle(LBSLayer _layer)
        {
            bool hasItems = list.itemsSource.Count > 0;
            DisplayStyle notificatorsDisplay = hasItems ? DisplayStyle.None : DisplayStyle.Flex;
            DisplayStyle noSelectedDisplay = hasItems ? DisplayStyle.None : DisplayStyle.Flex;
            DisplayStyle listDisplay = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
            
            DisplayStyle settingsDisplay = (_layer!=null && hasItems) ? DisplayStyle.Flex : DisplayStyle.None;
            
            foreach (VisualElement layer in noLayerNotificators)
            {
                layer.style.display = notificatorsDisplay;
            }
            list.style.display = listDisplay; 
            layerSettings.style.display = settingsDisplay;
            noSelectedLayerNotificator.style.display = noSelectedDisplay;
        }


        private void OnLayerSelectedEventHandle(LBSLayer layer){
            if (layer is not null)
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.None;
                layerSettings.style.display = DisplayStyle.Flex;
            } 
            else 
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                layerSettings.style.display = DisplayStyle.None;
            }
        }


        private void OnKeyDown(KeyDownEvent evt)
        {
            // delete selected layer
            if (evt.keyCode == KeyCode.Delete)
            {
                var predeleteIndex = list.selectedIndex;
                
                RemoveSelectedLayer();
                
                if (data.LayerCount < 1)
                {
                    //OnSelectLayer?.Invoke(null); 
                    evt.StopPropagation();
                    return;
                }
                
                int nextIndex = predeleteIndex >= data.LayerCount ? data.LayerCount - 1 : predeleteIndex;
                
                LBSLayer selected = data.GetLayer(nextIndex);
                if (selected != null)
                {
                    OnSelectLayer?.Invoke(selected);
                    OnLayerChangeEventHandle(selected);
                    OnLayerSelectedEventHandle(selected);
                    
                    evt.StopPropagation();
                    return;
                }
            }
            if (evt.keyCode == KeyCode.Z)
            {
                evt.StopPropagation(); 
            }

        }
        
        #endregion


    }
}