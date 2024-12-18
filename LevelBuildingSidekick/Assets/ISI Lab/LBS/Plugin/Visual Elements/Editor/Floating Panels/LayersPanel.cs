using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Template;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;


namespace ISILab.LBS.VisualElements.Editor
{
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
        private VisualElement noLayerNotificator; 
        #endregion

        #region EVENTS ACTIONS
        public event Action<LBSLayer> OnAddLayer;
        public event Action<LBSLayer> OnRemoveLayer;
        public event Action<LBSLayer> OnSelectLayer; // click simple (!)
        public event Action<LBSLayer> OnDoubleSelectLayer; // doble click (!)
        public event Action<LBSLayer> OnLayerVisibilityChange;
        #endregion

        #region CONSTRUCTORS
        public LayersPanel() { }

        public LayersPanel(LBSLevelData data, ref List<LayerTemplate> templates)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayersPanel");
            visualTree.CloneTree(this);

            this.data = data;
            this.templates = templates;

            //Event self conection
            //this.OnAddLayer += (LBSLayer layer) => OnLayerChangeEventHandle(layer);
            this.OnAddLayer += OnLayerChangeEventHandle;
            this.OnRemoveLayer += OnLayerChangeEventHandle;

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

            noLayerNotificator = this.Q<VisualElement>("NoLayerNotifator");

        }
        #endregion

        #region METHODS
        private LBSLayer CreateLayer(int _index)
        {
            var layers = templates.Select(t => t.layer).ToList();
            return layers[_index].Clone() as LBSLayer;
        }

        private void AddLayer(int _index)
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

            list.Rebuild(); // esta raro
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
            OnRemoveLayer?.Invoke(layer);
            list.Rebuild();

            DrawManager.ReDraw();
        }

        // Simple Click over element
        private void SelectionChange(IEnumerable<object> objs)
        {
            if (objs.Count() <= 0)
                return;

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

        void OnLayerChangeEventHandle(LBSLayer _layer){
            if (list.itemsSource.Count > 0){
                noLayerNotificator.style.display = DisplayStyle.None;
            } else {
                noLayerNotificator.style.display = DisplayStyle.Flex;
            }
        }
        #endregion
    }
}