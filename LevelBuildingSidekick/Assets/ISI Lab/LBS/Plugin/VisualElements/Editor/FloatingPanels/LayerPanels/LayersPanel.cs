// Cleaned and fixed version
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
        public readonly LBSLevelData Data;
        private LBSLayer _selectedLayer;
        private readonly List<LayerTemplate> _templates;

        private readonly ListView _list;
        private readonly TextField _nameField;
        private readonly List<VisualElement> _noLayerNotifications;
        private readonly VisualElement _noSelectedLayerNotificator;

        public event Action<LBSLayer> OnAddLayer;
        public event Action<LBSLayer> OnRemoveLayer;
        public event Action<LBSLayer> OnSelectLayer;
        public event Action<LBSLayer> OnDoubleSelectLayer;
        public event Action<LBSLayer> OnLayerVisibilityChange;
        public event Action<LBSLayer> OnLayerOrderChange;

        private List<int> dragAffected = new List<int>();

        public LayersPanel() { }

        public LayersPanel(LBSLevelData data, ref List<LayerTemplate> templates)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayersPanel");
            visualTree.CloneTree(this);

            this.Data = data;
            this._templates = templates;

            OnAddLayer += OnLayerChangeEventHandle;
            OnRemoveLayer += OnLayerChangeEventHandle;

            _list = this.Q<ListView>("List");

            VisualElement MakeItem() => new LayerView();
            _list.bindItem += (item, index) =>
            {
                if (index >= this.Data.LayerCount) 
                {
                    dragAffected.Remove(index);
                    return;
                }

                if (item is LayerView view)
                {
                    var layer = this.Data.GetLayer(index);
                    layer.index = _list.childCount - index;
                    if(dragAffected.Count == 0)
                    {

                        if(view.OnLayerVisibilityChangeAction != null)
                            view.OnVisibilityChange -= view.OnLayerVisibilityChangeAction;
                        view.OnLayerVisibilityChangeAction = () => OnLayerVisibilityChange?.Invoke(layer);
                        view.OnVisibilityChange += view.OnLayerVisibilityChangeAction;
                        view.SetInfo(layer);
                        view.OnNameChange += () => layer.InvokeNameChanged();
                    }
                    else
                    {
                        dragAffected.Remove(index);
                        if(dragAffected.Count == 0)
                        {
                            OnLayerOrderChange?.Invoke(layer);
                        }
                    }

                    ChangeListItemView(item);
                }
            };
            _list.itemIndexChanged += (_old, _new) => {
                UnityEngine.Assertions.Assert.IsTrue(dragAffected.Count == 0);
                int count = Mathf.Abs(_new - _old) + 1;
                int step = (int)Mathf.Sign(_new - _old);
                for(int i = 0; i < count; i++)
                {
                    int index = _old + i * step;
                    dragAffected.Add(index);
                }
            };

            _list.fixedItemHeight = 24;
            _list.itemsSource = data.Layers;
            _list.makeItem += MakeItem;
            _list.itemsChosen += ItemChosen;
            _list.selectionChanged += SelectionChange;

            _nameField = this.Q<TextField>("NameField");

            var addLayerButton = this.Q<ToolbarMenu>("AddLayerButtonMenu");
            foreach (var ve in addLayerButton.Children())
            {
                if (ve != addLayerButton.Children().Last())
                    ve.style.display = DisplayStyle.None;
            }

            for (int i = 0; i < templates.Count; i++)
            {
                int x = i;
                addLayerButton.menu.AppendAction(templates[i].templateName, _ => AddLayerByTemplate(x));
            }

            Button removeSelectedBtn = this.Q<Button>("RemoveSelectedButton");
            removeSelectedBtn.clicked += RemoveSelectedLayer;

            _noLayerNotifications = this.Query<VisualElement>("NoLayerNotify").ToList();
            _noSelectedLayerNotificator = this.Q<VisualElement>("NoSelectedLayerNotify");

            _list.style.display = DisplayStyle.None;
            _noSelectedLayerNotificator.style.display = DisplayStyle.Flex;

            _list.Rebuild();

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            OnLayerChangeEventHandle(null);
        }

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
            int i = 1;
            while (Data.Layers.Any(l => l.Name.Equals(layer.Name)))
            {
                layer.Name = _nameField.text + " " + i;
                i++;
            }

            Data.AddLayer(layer);
            _list.SetSelectionWithoutNotify(new List<int>() { 0 });

            OnAddLayer?.Invoke(layer);
            SetSelectedLayer(layer);

            LBSMainWindow.MessageNotify("New Data layer created");
            _list.Rebuild();
        }

        private void RemoveSelectedLayer()
        {
            if (!Data.Layers.Any()) return;

            var index = _list.selectedIndex;
            if (index < 0) return;

            var answer = EditorUtility.DisplayDialog("Caution",
                "You are about to delete a layer. Are you sure?",
                "Continue", "Cancel");

            if (!answer) return;

            var removedLayer = Data.RemoveAt(index);
            removedLayer.RemoveAll();

            LBSLayer next = null;
            if (Data.LayerCount > 0)
            {
                int nextIndex = Mathf.Clamp(index, 0, Data.LayerCount - 1);
                next = Data.GetLayer(nextIndex);
            }

            DrawManager.Instance.RemoveContainer(removedLayer);
            OnRemoveLayer?.Invoke(removedLayer);
            _list.Rebuild();

            SetSelectedLayer(next);
            LBSMainWindow.MessageNotify("Data layer deleted");
        }

        private void SelectionChange(IEnumerable<object> objs)
        {
            var enumerable = objs as object[] ?? objs.ToArray();
            if (!enumerable.Any()) 
            {
                return;
            }

            var selected = enumerable.ToList()[0] as LBSLayer;
            _noSelectedLayerNotificator.style.display = selected == null ? DisplayStyle.Flex : DisplayStyle.None;
            OnSelectLayer?.Invoke(selected);
        }
        private void ItemChosen(IEnumerable<object> objs)
        {
            var selected = objs.FirstOrDefault() as LBSLayer;
            if (selected == null)
            {
                _noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                return;
            }

            OnDoubleSelectLayer?.Invoke(selected);
        }

        public void ResetSelection()
        {
            _list.ClearSelection();
            SetSelectedLayer(null);
        }

        private void OnLayerChangeEventHandle(LBSLayer _)
        {
            bool hasItems = _list.itemsSource.Count > 0;
            DisplayStyle notifDisplay = hasItems ? DisplayStyle.None : DisplayStyle.Flex;

            foreach (var ve in _noLayerNotifications)
                ve.style.display = notifDisplay;

            _list.style.display = hasItems ? DisplayStyle.Flex : DisplayStyle.None;

            if (_selectedLayer == null && hasItems)
                _noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
            else
                _noSelectedLayerNotificator.style.display = DisplayStyle.None;
        }

        private void SetSelectedLayer(LBSLayer layer)
        {
            if (Equals(layer, _selectedLayer)) return;
            _selectedLayer = layer;

            if (layer != null)
            {
                LBSInspectorPanel.ActivateDataTab();
                _noSelectedLayerNotificator.style.display = DisplayStyle.None;
                OnSelectLayer?.Invoke(layer);
            }
            else
            {
                _noSelectedLayerNotificator.style.display = DisplayStyle.Flex;
                LBSInspectorPanel.Instance.SetSelectedTab(null);
                OnSelectLayer?.Invoke(null);
            }
        }

        private static void ChangeListItemView(VisualElement item)
        {
            var container = item.parent;
            if (container != null)
            {
                container.style.paddingRight = 5;
                container.style.paddingLeft = 5;
            }

            var parent = item.parent?.parent;
            if (parent is { childCount: > 0 })
            {
                parent[0].style.display = DisplayStyle.None;
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                var preIndex = _list.selectedIndex;
                RemoveSelectedLayer();

                if (Data.LayerCount < 1)
                {
                    evt.StopPropagation();
                    return;
                }

                int nextIndex = Mathf.Clamp(preIndex, 0, Data.LayerCount - 1);
                var next = Data.GetLayer(nextIndex);
                SetSelectedLayer(next);

                evt.StopPropagation();
            }

            if (evt.ctrlKey && evt.keyCode == KeyCode.D && _selectedLayer != null)
            {
                AddLayer(_selectedLayer.Clone() as LBSLayer);
            }
        }
    }
}
