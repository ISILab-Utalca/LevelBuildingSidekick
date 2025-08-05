using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS;
using LBS.Bundles;
using LBS.Components;
using LBS.VisualElements;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        // Visual Element
        VisualElement warning;
        VisualElement mappingContent;

        List<TileBundleGroup> populationGroups = new List<TileBundleGroup>();
        #endregion

        #region PROPERTIES
        public PathOSWindow PathOSOriginalWindow { get => pathOSOriginalWindow; set => pathOSOriginalWindow = value; }

        LBSLevelData Data { get => _target.OwnerLayer.Parent; }
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

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("TestingBehaviourEditor");
            visualTree.CloneTree(this);
            
            warning = this.Q<VisualElement>("Warning");
            mappingContent = this.Q<VisualElement>("TestingMappingContent");

            var toggle = this.Q<Toggle>("AutoMap");
            toggle.RegisterValueChangedCallback(evt => throw new System.NotImplementedException("This feature is currently not available."));

            var mapPopulButton = this.Q<Button>("MapPopulation");
            mapPopulButton.clicked += () => MapToPopulation();

            var clearButton = this.Q<Button>("Clear");
            clearButton.clicked += () => _target.ClearMapping();

            // Add and set Tag Pallete

            bundlePallete = new PathOSTagPallete();
            Add(bundlePallete);
            bundlePallete.SetName("[LEGACY] PathOS+ Tags");
            SetBundlePallete();

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
            tAdd.OnSelect += () => LBSInspectorPanel.ActivateBehaviourTab();
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
            //tClosed.OnSelect += () => LBSInspectorPanel.ActivateAssistantTab();
            toolkit.ActivateTool(tClosed, _target.OwnerLayer, _target);

            // Add open obstacle
            icon = Resources.Load<Texture2D>("Icons/AddOpenedObstacle");
            addOpenedObstacle = new AddOpenedObstacle();
            var tOpen = new LBSTool(addOpenedObstacle);
            //tOpen.OnSelect += () => LBSInspectorPanel.ActivateAssistantTab();
            toolkit.ActivateTool(tOpen, _target.OwnerLayer, _target);
        }

        public override void OnFocus()
        {
            base.OnFocus();

            populationGroups.Clear();

            if (Data.LayerCount <= 1)
            {
                ShowMappingContent(false);
                return;
            }

            List<LBSLayer> populationLayers = Data.Layers.FindAll(l => l.ID.Equals("Population"));
            if(populationLayers.Count == 0)
            {
                ShowMappingContent(false);
                return;
            }

            GetPopulationGroups(populationLayers);

            ShowMappingContent(true);
        }

        public void GetPopulationGroups(List<LBSLayer> layers)
        {
            var tileMaps = new List<BundleTileMap>();
            foreach (LBSLayer layer in layers)
            {
                var tileMap = layer.GetModule<BundleTileMap>();
                if (tileMap != null)
                    tileMaps.Add(tileMap);
            }

            foreach(BundleTileMap tileMap in tileMaps)
            {
                populationGroups.AddRange(tileMap.Groups);
            }
        }

        public void ShowMappingContent(bool show)
        {
            mappingContent  .style.display = (DisplayStyle)( show ? 0 : 1);
            warning         .style.display = (DisplayStyle)(!show ? 0 : 1);
        }

        private void MapToPopulation()
        {
            LoadedLevel level = LBSController.CurrentLevel;

            EditorGUI.BeginChangeCheck();

            _target.MapToPopulation(populationGroups);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(level);

            DrawManager.Instance.RedrawLayer(_target.OwnerLayer);
        }

        #endregion
    }
}
