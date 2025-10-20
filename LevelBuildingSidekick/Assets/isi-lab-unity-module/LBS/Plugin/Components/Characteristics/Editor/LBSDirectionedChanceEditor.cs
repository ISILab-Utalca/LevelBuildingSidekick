using ISILab.LBS.Characteristics;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static ISILab.LBS.Modules.ConnectedTileMapModule;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Connections group chance", typeof(LBSDirectionedChance))]
    public class LBSDirectionedChanceEditor : LBSCustomEditor
    {
        public VisualElement content;

        public LBSDirectionedChanceEditor()
        {
        }

        public LBSDirectionedChanceEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }


        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var target = paramTarget as LBSDirectionedChance;

            if (target == null)
                return;

            //target._Update();

            content = new VisualElement();
            Add(content);

            var tiletype = new DropdownField
            {
                label = "Tile Type",
                choices = System.Enum.GetNames(typeof(ConnectedTileType)).ToList(),
                value = target.currentType.ToString()
            };

            tiletype.RegisterValueChangedCallback(evt =>
            {
                if (System.Enum.TryParse<ConnectedTileType>(evt.newValue, out var result))
                {
                    target.currentType = result;
                }
            });

            content.Add(tiletype);

            content.Add(new VisualElement() { style = { height = 20 } });

            var weights = target.tileDirections;

            /*
            // Show warning if there are no child bundles to add weights
            if (weights.Count <= 0)
            {
                var wp = new WarningPanel("This feature adds chances to the child bundles, " +
                                          "make sure to have child bundles for this feature to work.");

                content.Add(wp);
                return;
            }
            */

            var list = new LBSCustomTreeView();
            var treeData = new List<TreeViewItemData<TreeNodeData>>
            {
                new TreeViewItemData<TreeNodeData>(0, new TreeNodeData { Id = 0, Label = "Grupo 1", Type = NodeType.Label }, new List<TreeViewItemData<TreeNodeData>>
                {
                    new TreeViewItemData<TreeNodeData>(1, new TreeNodeData { Id = 1, Label = "Texto", Type = NodeType.Label }),
                    new TreeViewItemData<TreeNodeData>(2, new TreeNodeData { Id = 2, Label = "Peso", SliderValue = 0.5f, Type = NodeType.Slider }),
                    new TreeViewItemData<TreeNodeData>(3, new TreeNodeData { Id = 3, Label = "Objeto", ObjectFieldValue = null, Type = NodeType.ObjectField })
                })
            };


            list.SetRootItems(treeData);
            list.makeItem = () => {
                var ve = new VisualElement();

                ve.ClearClassList();
                ve.AddToClassList("lbs-tree-view-item");

                return ve;
            };

            list.bindItem = (element, index) =>
            {
                element.Clear();
                var nodeData = list.GetItemDataForIndex<TreeNodeData>(index);

                var label = new Label(nodeData.Label);
                label.AddToClassList("lbs-tree-view-item");

                switch (nodeData.Type)
                {
                    case NodeType.Label:
                        element.Add(label);
                        break;
                    case NodeType.Slider:
                        var slider = new Slider(0, 1) { value = nodeData.SliderValue ?? 0, showInputField = true };
                        slider.RegisterValueChangedCallback(evt => nodeData.SliderValue = evt.newValue);
                        slider.AddToClassList("lbs-tree-view-item");
                        element.Add(label);
                        element.Add(slider);
                        break;
                    case NodeType.ObjectField:
                        var objField = new LBSCustomObjectField { objectType = typeof(UnityEngine.Object), value = nodeData.ObjectFieldValue };
                        objField.RegisterValueChangedCallback(evt => nodeData.ObjectFieldValue = evt.newValue as UnityEngine.Object);
                        objField.AddToClassList("lbs-tree-view-item");
                        element.Add(label);
                        element.Add(objField);
                        break;
                }

                element.AddToClassList("lbs-tree-view-item");
            };
            content.Add(list);


            /*
            // Instance the weights of the child bundles
            for (int i = 0; i < weights.Count; i++)
            {
                var current = weights[i];
                var box = new VisualElement();
                content.Add(box);

                box.Add(new Label(current.mainTarget.Name));

                var slider = new Slider();
                slider.showInputField = true;
                box.Add(slider);
                slider.lowValue = 0;
                slider.highValue = 1;
                slider.value = current.weight;
                slider.RegisterValueChangedCallback(evt =>
                {
                    current.weight = evt.newValue;
                });
            }
            */


        }

        protected override VisualElement CreateVisualElement()
        {
            var target = this.target as LBSDirectionedChance;

            content = new VisualElement();
            content.AddToClassList("lbs-tree-view");

            return this;
        }
    }

    public class TreeNodeData
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public float? SliderValue { get; set; }
        public UnityEngine.Object ObjectFieldValue { get; set; }
        public NodeType Type { get; set; }
        public List<TreeNodeData> Children { get; set; } = new();

    }

    public enum NodeType
    {
        Label,
        Slider,
        ObjectField
    }
}

