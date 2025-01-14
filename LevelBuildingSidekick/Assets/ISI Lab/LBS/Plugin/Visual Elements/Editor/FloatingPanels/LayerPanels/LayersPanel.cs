using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Template;
using LBS.Components;
using LBS.Settings;

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

    public struct SavedLayerPanel
    {
        public LBSLevelData data;
        public LBSLayer selectedLayer;
    }
    
    public class LayersPanel : VisualElement
    {

        
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<LayersPanel, VisualElement.UxmlTraits> { }
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
        private ToolbarMenu addButtonMenu;   
        private DropdownField typeDropdown;
        private List<VisualElement> noLayerNotificators; 

        private VisualElement noSelectedLayerNotificator;
        private VisualElement layerSettings;
        #endregion

        #region EVENTS ACTIONS
        public event Action<LBSLayer> OnAddLayer;
        public event Action<LBSLayer> OnRemoveLayer;
        public event Action<LBSLayer> OnSelectLayer; // click simple (!)
        public event Action<LBSLayer> OnDoubleSelectLayer; // doble click (!)
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
            this.OnAddLayer += (LBSLayer layer) => OnLayerChangeEventHandle(layer);
            this.OnRemoveLayer += (LBSLayer layer) => OnLayerChangeEventHandle(layer);
            this.OnSelectLayer += (LBSLayer layer) => OnLayerSelectedEventHandle(layer);

            // LayerList
            list = this.Q<ListView>("List");

            Func<VisualElement> makeItem = () =>
            {
                return new LayerView();
            };

            list.bindItem += (item, index) =>
            {
                if (index >= this.data.LayerCount)
                    return;

                var view = (item as LayerView);
                var layer = this.data.GetLayer(index);
                view.SetInfo(layer);
                view.OnVisibilityChange += () => { OnLayerVisibilityChange(layer); };
            };

            // list configuration
            list.fixedItemHeight = 24;
            list.itemsSource = data.Layers;
            list.makeItem += makeItem;
            list.itemsChosen += ItemChosen;
            list.selectionChanged += SelectionChange;

            // NameField
            nameField = this.Q<TextField>("NameField");

            // TypeDropdown
            typeDropdown = this.Q<DropdownField>("TypeDropdown");
            typeDropdown.choices = templates.Select(t => t.name).ToList();
            typeDropdown.index = 0;
            
            //Add Layer Button Menu
            addButtonMenu = this.Q<ToolbarMenu>("AddLayerButtonMenu");
            for(int i = 0; i < templates.Count; i++)
            {
                int x = i;
                addButtonMenu.menu.AppendAction(templates[i].name, (DropdownMenuAction dma) => {
                    AddLayer(x);
                });
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
        private LBSLayer CreateLayer(int _index)
        {
            var layers = templates.Select(t => t.layer).ToList();
            return layers[_index].Clone() as LBSLayer;
        }

        public void AddLayer(int _index)
        {
            if (_index < 0)
            {
                Debug.LogWarning("No layer type has been selected yet, make sure to select one.");
                return;
            }

            var layer = CreateLayer(_index);

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

            LBSMainWindow.MessageNotify("New Data layer created", LogType.Log, 2);
            
            list.Rebuild();
        }
        
        public void RemoveSelectedLayer()
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
            OnRemoveLayer?.Invoke(layer);
            list.Rebuild();

            LBSMainWindow.MessageNotify("Data layer deleted", LogType.Log, 2);
            
            DrawManager.ReDraw();
        }

        // Simple Click over element
        private void SelectionChange(IEnumerable<object> objs)
        {
            if (objs.Count() <= 0) {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                return;
                }

            var selected = objs.ToList()[0] as LBSLayer;
            OnSelectLayer?.Invoke(selected);
        }

        // Double Click over element
        private void ItemChosen(IEnumerable<object> objs)
        {
            if (objs.Count() <= 0)
                return;

            var selected = objs.ToList()[0] as LBSLayer;
            OnDoubleSelectLayer?.Invoke(selected);
        }

        public void ResetSelection()
        {
            list.ClearSelection();
            //list.RemoveFromSelection(list.selectedIndex);
        }

        void OnLayerChangeEventHandle(LBSLayer _layer)
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


        void OnLayerSelectedEventHandle(LBSLayer _layer){
            if (_layer != null)
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.None;
                layerSettings.style.display = DisplayStyle.Flex;
            } else 
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                layerSettings.style.display = DisplayStyle.None;
            }
        }
        

        void OnKeyDown(KeyDownEvent evt)
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
                //Debugging here
                
                evt.StopPropagation(); 
                return;
            }

        }
        
        #endregion


    }
}