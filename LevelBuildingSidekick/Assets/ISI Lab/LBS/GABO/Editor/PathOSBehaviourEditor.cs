using ISILab.LBS.Behaviours;
using ISILab.LBS;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Characteristics;
using LBS.VisualElements;
using ISILab.LBS.Internal;
using LBS.Bundles;
using System.Linq;
using ISILab.LBS.Assistants;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSBehaviour", typeof(PathOSBehaviour))]
    public class PathOSBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        // Palletes
        private PathOSTagPallete bundlePallete;
        // Target (PathOSBehaviour)
        private PathOSBehaviour _target;
        #endregion

    #region METHODS
        public PathOSBehaviourEditor(object target) : base(target)
        {
            _target = target as PathOSBehaviour;

            CreateVisualElement();
        }

        public override void SetInfo(object target)
        {
            _target = target as PathOSBehaviour;
        }

        protected override VisualElement CreateVisualElement()
        {
            bundlePallete = new PathOSTagPallete();
            Add(bundlePallete);
            bundlePallete.SetName("PathOS+");

            SetBundlePallete();

            return this;
        }

        // GABO TODO: Terminarrrrrr
        private void SetBundlePallete()
        {
            bundlePallete.name = "Bundles";
            Texture2D icon = Resources.Load<Texture2D>("Icons/TinyIconPathOSModule16x16");
            bundlePallete.SetIcon(icon, Color.white);

            // Get proper bundles
            List<Bundle> allBundles = LBSAssetsStorage.Instance.Get<Bundle>();
            List<Bundle> pathOSBundles = allBundles.Where(
                b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>().Count > 0).ToList();

            // If there are no PathOS bundles, abort.
            if (pathOSBundles.Count == 0) { return; }

            // Generalizacion de Bundles a "object" (ej.: Para usar en el objeto pallete, y los option views)
            var options = new object[pathOSBundles.Count];
            for (int i = 0; i < pathOSBundles.Count; i++)
            {
                options[i] = pathOSBundles[i];
            }

            // No mostrar Dropdown por defecto
            bundlePallete.ShowGroups = false;

            // OnSelect event
            // GABO TODO: Agregar herramienta propia cuando este hecha
            bundlePallete.OnSelectOption += (selected) =>
            {
                _target.selectedToSet = selected as Bundle;
                //ToolKit.Instance.SetActive("Paint Tile");
            };

            // OnAdd option event
            bundlePallete.OnAddOption += () =>
            {
                Debug.LogWarning("Por ahora esta herramienta no permite agregar nuevos tipos de bundles");
            };

            // Tooltip event (texto al mantener mouse sobre opcion)
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

            // Init options para el Pallete mismo
            bundlePallete.SetOptions(options, (optionView, option) =>
            {
                var bundle = (Bundle)option;
                optionView.Label = bundle.name;
                optionView.Color = bundle.Color;
                optionView.Icon = bundle.Icon;
            });

            bundlePallete.Repaint();
        }

        
        //GABO TODO
        public void SetTools(ToolKit toolkit)
        {
            return;
        }
    }
    #endregion
}
