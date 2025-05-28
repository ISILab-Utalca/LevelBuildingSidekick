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
                layer.index = list.childCount - index;
                if (view == null) return;
                view.SetInfo(layer);
                view.OnVisibilityChange += () => OnLayerVisibilityChange(layer);
                ChangeListItemView(item);
            };
            
            // list configuration
            list.fixedItemHeight = 24;
            list.itemsSource = data.Layers;
            list.makeItem += MakeItem;
            list.itemsChosen += ItemChosen;
            list.selectionChanged += SelectionChange;

            // NameField
            nameField = this.Q<TextField>("NameField");

            //Add Layer Button Menu
            var addLayerButton = this.Q<ToolbarMenu>("AddLayerButtonMenu");
            foreach (var ve in addLayerButton.Children())
            {
                if (ve != addLayerButton.Children().Last())
                {
                    ve.style.display = DisplayStyle.None; // Hide button dropdown, ugly af
                }
            }
            
            for(int i = 0; i < templates.Count; i++)
            {
                int x = i;
                addLayerButton.menu.AppendAction(templates[i].name, _ => AddLayerByTemplate(x));
            }

            // RemoveSelectedButton
            Button RemoveSelectedBtn = this.Q<Button>("RemoveSelectedButton");
            RemoveSelectedBtn.clicked += RemoveSelectedLayer;

            noLayerNotificators = this.Query<VisualElement>("NoLayerNotify").ToList();
            noSelectedLayerNotificator = this.Q<VisualElement>("NoSelectedLayerNotify");
            
            list.style.display = DisplayStyle.None;
            noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
            
            list.Rebuild();
            
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

        private void AddLayerByTemplate(int index)
        {
            if (index < 0)
            {
                Debug.LogWarning("No layer type has been selected yet, make sure to select one.");
                return;
            }

            var layer = CreateLayer(index);
            layer.Name = LBSSettings.Instance.general.baseLayerName;

            AddLayer(layer);

        }

        private void AddLayer(LBSLayer layer)
        {
            int i = 1;
            while (data.Layers.Any(l => l.Name.Equals(layer.Name)))
            {
                layer.Name = nameField.text + " " + i;
                i++;
            }

            data.AddLayer(layer);
            list.SetSelectionWithoutNotify(new List<int>() {0});
            OnAddLayer?.Invoke(layer);
            OnLayerSelectedEventHandle(layer);
            LBSMainWindow.MessageNotify("New Data layer created");
            list.Rebuild();
        }

        private void RemoveSelectedLayer()
        {
            if (!data.Layers.Any()) return;
                

            var index = list.selectedIndex;
            if (index < 0) return;

            var answer = EditorUtility.DisplayDialog("Caution",
            "You are about to delete a layer. If you proceed with this action, all of its" +
            " content will be permanently removed, and you won't be able to recover it. Are" +
            " you sure you want to continue?", "Continue", "Cancel");

            if (!answer) return;
            
            var layer = data.RemoveAt(index);

            //Select a new layer
            if(data.LayerCount != 0)
            {
                if (index >= data.LayerCount) list.selectedIndex--;
                else OnSelectLayer(data.GetLayer(list.selectedIndex));
            }
            
            DrawManager.Instance.RemoveContainer(layer);
            
            OnRemoveLayer?.Invoke(layer);
            list.Rebuild();

            if (list.childCount == 0)
            {
                OnLayerSelectedEventHandle(null);
                ResetSelection();
            }
            
            LBSMainWindow.MessageNotify("Data layer deleted");
            // No need to redraw everything because now we manually delete the visual elements (graph element)
            // that belong to a layer
            //  DrawManager.ReDraw(); 
        }

        // Simple Click over an element
        private void SelectionChange(IEnumerable<object> objs)
        {
            if (!objs.Any()) 
            {
                return;
            }

            var selected = objs.ToList()[0] as LBSLayer;
            OnSelectLayer?.Invoke(selected);
        }
        
        // Double Click over an element
        private void ItemChosen(IEnumerable<object> objs)
        {
            List<object> enumerable = objs.ToList();
            if (!enumerable.Any())
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                return;
            }
            var selected = objs.ToList()[0] as LBSLayer;
            OnDoubleSelectLayer?.Invoke(selected);
        }

        public void ResetSelection()
        {
            list.ClearSelection();
        }

        private void OnLayerChangeEventHandle(LBSLayer _layer)
        {
            bool hasItems = list.itemsSource.Count > 0;
            DisplayStyle notificatorsDisplay = hasItems ? DisplayStyle.None : DisplayStyle.Flex;
            DisplayStyle noSelectedDisplay = _layer is null && hasItems ? DisplayStyle.Flex : DisplayStyle.None;
            DisplayStyle listDisplay = hasItems ? DisplayStyle.Flex : DisplayStyle.None;
            
            foreach (VisualElement layer in noLayerNotificators)
            {
                layer.style.display = notificatorsDisplay;
            }
            list.style.display = listDisplay; 
            noSelectedLayerNotificator.style.display = noSelectedDisplay;
            if(_layer is not null) OnSelectLayer?.Invoke(_layer);
        }
        
        private void OnLayerSelectedEventHandle(LBSLayer layer)
        {
            selectedLayer = layer;
            if (layer is not null)
            {
                LBSInspectorPanel.ActivateDataTab();
                noSelectedLayerNotificator.style.display = DisplayStyle.None;
            } 
            else 
            {
                noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                LBSInspectorPanel.Instance.SetSelectedTab(null);
            }
        }

        /// <summary>
        /// Removes the Animation handle and overwrites the padding after the list item container
        /// overwrites the LayerView padding
        /// </summary>
        /// <param name="item"></param>
        private static void ChangeListItemView(VisualElement item)
        {
            var container = item.parent;
            if (container is null) return;
            
            container.style.paddingRight = 5;
            container.style.paddingLeft = 5;

            var containerParent = item.parent.parent;
            if (containerParent is null) return;
            if (!containerParent.Children().Any()) return;
            
            var handle = containerParent.Children().First();
            handle.style.display = DisplayStyle.None;
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

            if (evt.ctrlKey)
            {
                if(evt.keyCode == KeyCode.D)  AddLayer(selectedLayer.Clone() as LBSLayer);
            }
        }
        
        
        #endregion


    }
}