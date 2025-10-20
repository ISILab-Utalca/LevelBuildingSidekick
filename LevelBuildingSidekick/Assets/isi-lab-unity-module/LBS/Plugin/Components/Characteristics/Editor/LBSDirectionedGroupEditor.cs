using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static ISILab.LBS.Modules.ConnectedTileMapModule;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Connections group", typeof(LBSDirectionedGroup))]
    public class LBSDirectionedGroupEditor : LBSCustomEditor
    {
        public VisualElement content;

        public LBSDirectionedGroupEditor()
        {

        }

        public LBSDirectionedGroupEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);

        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var target = paramTarget as LBSDirectionedGroup;

            if (target == null)
                return;

            target._Update();

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

            var weights = target.Weights;

            // Show warning if there are no child bundles to add weights
            if(weights.Count <= 0)
            {
                var wp = new WarningPanel("This feature adds weights to the child bundles, " +
                                          "make sure to have child bundles for this feature to work.");
                
                content.Add(wp);
                return;
            }

            // Intance the weights of the child bundles
            for (int i = 0; i < weights.Count; i++)
            {
                var current = weights[i];
                var box = new VisualElement();
                content.Add(box);

                box.Add(new Label(current.target.name));

                var slider = new Slider();
                slider.showInputField = true;
                box.Add(slider);
                slider.lowValue = 0;
                slider.highValue = 1;
                slider.value = current.weight;
                slider.RegisterValueChangedCallback( evt =>
                {
                    current.weight = evt.newValue;
                });
            }
        }

        protected override VisualElement CreateVisualElement()
        {
            var target = this.target as LBSDirectionedGroup;

            content = new VisualElement();

            return this;
        }
    }
}