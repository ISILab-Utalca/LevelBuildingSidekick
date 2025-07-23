using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Characteristics;
using LBS.VisualElements;
using ISILab.LBS.Internal;
using LBS.Bundles;
using System.Linq;
using LBS;
using UnityEditor;
using System.Dynamic;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSBehaviour", typeof(PathOSBehaviour))]
    public class PathOSBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        // Palletes
        private PathOSTagPallete bundlePallete;
        // PathOS Original Inspector
        private PathOSWindow pathOSOriginalWindow;
        // Target (PathOSBehaviour)
        private PathOSBehaviour _target;
        // Manipulators
        AddPathOSTile addPathOSTile;
        RemovePathOSTile removePathOSTile;
        AddClosedObstacle addClosedObstacle;
        AddOpenedObstacle addOpenedObstacle;
        #endregion

        #region PROPERTIES
        public PathOSWindow PathOSOriginalWindow { get => pathOSOriginalWindow; set => pathOSOriginalWindow = value; }
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
            // Add and set Tag Pallete
            bundlePallete = new PathOSTagPallete();
            Add(bundlePallete);
            bundlePallete.SetName("PathOS+ Tags");
            SetBundlePallete();

            // Add the original PathOSWindow (IMGUI-based), create new if there's no instance.
            PathOSWindow[] oldWindows = Resources.FindObjectsOfTypeAll<PathOSWindow>();
            if (pathOSOriginalWindow == null)
            {
                if (oldWindows.Length == 0)
                {
                    pathOSOriginalWindow = ScriptableObject.CreateInstance<PathOSWindow>();
                }
                else
                {
                    pathOSOriginalWindow = oldWindows[0];
                }
            }
            IMGUIContainer container = new IMGUIContainer(pathOSOriginalWindow.OnGUI);
            Add(container);

            return this;
        }

        private void SetBundlePallete()
        {
            bundlePallete.name = "Bundles";
            Texture2D icon = Resources.Load<Texture2D>("Icons/TinyIconPathOSModule16x16");
            bundlePallete.SetIcon(icon, Color.white);

            // Obtener Bundles PathOS
            List<Bundle> allBundles = LBSAssetsStorage.Instance.Get<Bundle>();
            List<Bundle> pathOSBundles = allBundles.Where(
                b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>().Count > 0).ToList();

            // Si no hay PathOS Bundles, abortar.
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
            bundlePallete.OnSelectOption += (selected) =>
            {
                _target.selectedToSet = selected as Bundle;
                ToolKit.Instance.SetActive(typeof(AddPathOSTile));
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
                optionView.Color = bundle.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0].Value.Color;
                optionView.Icon = bundle.Icon;
            });

            bundlePallete.OnRepaint += () => { bundlePallete.Selected = _target.selectedToSet; };

            bundlePallete.Repaint();
        }

        //GABO TODO: Agregar herramientas faltantes
        public void SetTools(ToolKit toolkit)
        {
            // Add tiles
            Texture2D icon = Resources.Load<Texture2D>("Icons/AddTileBrush");
            addPathOSTile = new AddPathOSTile();
            //var tAdd = new LBSTool(icon, "Paint tiles", "", addPathOSTile);
            var tAdd = new LBSTool(addPathOSTile);
            //tAdd.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
            tAdd.OnSelect += () => LBSInspectorPanel.ActivateAssistantTab();
            //tAdd.Init(_target.OwnerLayer, _target);
            //toolkit.AddTool(tAdd);
            toolkit.ActivateTool(tAdd, _target.OwnerLayer, _target);

            // Remove tiles
            icon = Resources.Load<Texture2D>("Icons/RemoveTileBrush");
            removePathOSTile = new RemovePathOSTile();
            var tRemove = new LBSTool(removePathOSTile);

            toolkit.ActivateTool(tRemove, _target.OwnerLayer, _target);

            // Add closed obstacle
            icon = Resources.Load<Texture2D>("Icons/AddClosedObstacle");
            addClosedObstacle = new AddClosedObstacle();
            var tClosed = new LBSTool(addClosedObstacle);
            tClosed.OnSelect += () => LBSInspectorPanel.ActivateAssistantTab();
            toolkit.ActivateTool(tClosed, _target.OwnerLayer, _target);

            // Add open obstacle
            icon = Resources.Load<Texture2D>("Icons/AddOpenedObstacle");
            addOpenedObstacle = new AddOpenedObstacle();
            var tOpen = new LBSTool(addOpenedObstacle);
            tOpen.OnSelect += () => LBSInspectorPanel.ActivateAssistantTab();
            toolkit.ActivateTool(tOpen, _target.OwnerLayer, _target);
        }
    }
    #endregion
}
