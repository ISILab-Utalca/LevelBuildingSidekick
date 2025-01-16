using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using LBS;
using LBS.Bundles;
using LBS.Settings;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PopulationBehaviour", typeof(PopulationBehaviour))]
    public class PopulationBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        private readonly Color BHcolor = LBSSettings.Instance.view.behavioursColor;

        private PopulationBehaviour _target;

        //Manipulators
        AddPopulationTile addPopulationTile;
        RemovePopulationTile removePopulationTile;
        RotatePopulationTile rotatePopulationTile;

        //Palletes
        private SimplePallete bundlePallete;

        public PopulationBehaviourEditor(object target) : base(target)
        {
            _target = target as PopulationBehaviour;

            CreateVisualElement();
        }


        public override void SetInfo(object target)
        {
            _target = target as PopulationBehaviour;
        }

        public void SetTools(ToolKit toolkit)
        {
            Texture2D icon;

            // Add element Tiles
            icon = Resources.Load<Texture2D>("Icons/Tools/Population_Brush");
            addPopulationTile = new AddPopulationTile();
            var t1 = new LBSTool(icon, "Paint Tile", addPopulationTile);
            t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
            t1.Init(_target.Owner, _target);


            // Rotate element
            icon = Resources.Load<Texture2D>("Icons/Tools/Rotacion_population");
            rotatePopulationTile = new RotatePopulationTile();
            var t3 = new LBSTool(icon, "Rotate Tile", rotatePopulationTile);
            t3.Init(_target.Owner, _target);
     

            // Remove Tiles
            icon = Resources.Load<Texture2D>("Icons/Tools/Delete_population");
            removePopulationTile = new RemovePopulationTile();
            var t2 = new LBSTool(icon, "Remove Tile", removePopulationTile);
            t2.Init(_target.Owner, _target);

            
            addPopulationTile.SetAddRemoveConnection(removePopulationTile);
            
            toolkit.AddTool(t1);
            toolkit.AddTool(t2);
            toolkit.AddTool(t3);
        }

        protected override VisualElement CreateVisualElement()
        {
            bundlePallete = new SimplePallete();
            Add(bundlePallete);
            bundlePallete.SetName("Population");

            SetBundlePallete();

            return this;
        }

        private void SetBundlePallete()
        {
            bundlePallete.name = "Bundles";
            var icon = Resources.Load<Texture2D>("Icons/BrushIcon");
            bundlePallete.SetIcon(icon, BHcolor);

            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            if (bundles.Count == 0)
                return;

            var candidates = bundles.Where(b => b.Type == Bundle.TagType.Element).ToList();

            if (candidates.Count == 0)
                return;

            bundlePallete.ShowGroups = false;
            var options = new object[candidates.Count];
            for (int i = 0; i < candidates.Count; i++)
            {
                options[i] = candidates[i];
            }

            bundlePallete.OnSelectOption += (selected) =>
            {
                _target.selectedToSet = selected as Bundle;
                ToolKit.Instance.SetActive("Paint Tile");
            };

            // OnAdd option event
            bundlePallete.OnAddOption += () =>
            {
                Debug.LogWarning("Por ahora esta herramienta no permite agregar nuevos tipos de bundles");
            };

            bundlePallete.OnSetTooltip += (option) =>
            {
                var b = option as Bundle;

                var tooltip = "Tags:";
                if (b.Characteristics.Count > 0)
                {
                    b.Characteristics.ForEach(c => tooltip += "\n- " + c?.GetType().ToString());
                }
                else
                {
                    tooltip += "\n[None]";
                }
                return tooltip;
            };

            // Init options
            bundlePallete.SetOptions(options, (optionView, option) =>
            {
                var bundle = (Bundle)option;
                optionView.Label = bundle.name;
                optionView.Color = bundle.Color;
                optionView.Icon = bundle.Icon;
            });

            bundlePallete.OnRepaint += () => { bundlePallete.Selected = _target.selectedToSet; };

            bundlePallete.Repaint();

        }

        private void ChangeOptions(string tag)
        {
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            if (bundles.Count == 0)
                return;

            var candidates = bundles.Where(b => b.Type == Bundle.TagType.Element).ToList();

            if (candidates.Count == 0)
                return;

            var options = new List<Bundle>();

            if (tag == "All")
            {
                options = candidates;
            }
            else
            {
                //options = candidates.Where(b => b.name.Equals(tag) || b.Characteristics.Any(c => c is LBSTagsCharacteristic && c.Label.Equals(tag))).ToList();
            }

            bundlePallete.OnSelectOption += (selected) =>
            {
                _target.selectedToSet = selected as Bundle;
            };

            // OnAdd option event
            bundlePallete.OnAddOption += () =>
            {
                Debug.LogWarning("Por ahora esta herramienta no permite agregar nuevos tipos de bundles");
            };

            // Init options
            bundlePallete.SetOptions(options.ToArray(), (optionView, option) =>
            {
                var bundle = (Bundle)option;
                optionView.Label = bundle.Name;
                optionView.Color = bundle.Color;
                optionView.Icon = bundle.Icon;
            });

            bundlePallete.Repaint();
        }

    }
}