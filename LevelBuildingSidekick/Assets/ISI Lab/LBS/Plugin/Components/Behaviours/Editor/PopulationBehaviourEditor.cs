using System;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using LBS;
using LBS.Bundles;
using ISILab.LBS.Settings;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PopulationBehaviour", typeof(PopulationBehaviour))]
    public class PopulationBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        private readonly Color BHcolor = LBSSettings.Instance.view.behavioursColor;

        private PopulationBehaviour _target;

        private Dictionary<string, List<Bundle.PopulationTypeE>> displayChoices = new Dictionary<string, List<Bundle.PopulationTypeE>>();
        
        [SerializeField]
        private BundleCollection _collection; 
        [SerializeField]
        private string _populationFilter;
        private const string allFilter = "All";
        //Manipulators
        AddPopulationTile addPopulationTile;
        RemovePopulationTile removePopulationTile;
        RotatePopulationTile rotatePopulationTile;

        //Palletes
        private SimplePallete bundlePallete;
        private WarningPanel warningPanel;

        public PopulationBehaviourEditor(object target) : base(target)
        {
            _target = target as PopulationBehaviour;
            //_collection = load default collection

            _populationFilter = allFilter;
            List<Bundle.PopulationTypeE> characterList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Character };
            List<Bundle.PopulationTypeE> itemList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Item };
            List<Bundle.PopulationTypeE> interactableList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Interactable };
            List<Bundle.PopulationTypeE> areaList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Area };
            List<Bundle.PopulationTypeE> propList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Prop };
            List<Bundle.PopulationTypeE> miscList = new List<Bundle.PopulationTypeE> { Bundle.PopulationTypeE.Misc };
            List<Bundle.PopulationTypeE> allList = new List<Bundle.PopulationTypeE>
            {
                Bundle.PopulationTypeE.Misc,
                Bundle.PopulationTypeE.Prop,
                Bundle.PopulationTypeE.Area,
                Bundle.PopulationTypeE.Interactable,
                Bundle.PopulationTypeE.Item,
                Bundle.PopulationTypeE.Character
            };
            
            displayChoices.Add(allFilter, allList);
            displayChoices.Add(Bundle.PopulationTypeE.Character.ToString(), characterList);
            displayChoices.Add(Bundle.PopulationTypeE.Item.ToString(), itemList);
            displayChoices.Add(Bundle.PopulationTypeE.Interactable.ToString(), interactableList);
            displayChoices.Add(Bundle.PopulationTypeE.Area.ToString(), areaList);
            displayChoices.Add(Bundle.PopulationTypeE.Prop.ToString(), propList);
            displayChoices.Add(Bundle.PopulationTypeE.Misc.ToString(), miscList);

            SetInfo(_target);
            CreateVisualElement();
        }


        public override void SetInfo(object target)
        {
            _target = target as PopulationBehaviour;
            if (_target != null) _collection = _target.selectedCollectionToSet;
            if (_target != null) _populationFilter = _target.selectedTypeFilter;
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

        protected sealed override VisualElement CreateVisualElement()
        {
          
            /*bundlePallete = new SimplePallete();
            bundlePallete = new SimplePallete();
            Add(bundlePallete);
            bundlePallete.SetName("Population");
                        // Connection Pallete
            bundlePallete = this.Q<SimplePallete>("ConnectionPallete");
              SetBundlePallete();
            */

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationBehaviourEditor");
            visualTree.CloneTree(this);
            
            // WarningPanel
            warningPanel = this.Q<WarningPanel>();
            
            var collectionField = this.Q<ObjectField>("BundleCollection");
          
            collectionField.value = _collection;
            // only updates the first bundle value change - fix pending
            collectionField.RegisterValueChangedCallback(evt =>
            {
                var collection = evt.newValue as BundleCollection;
                collectionField.value = collection;
                _collection = collection;
                UpdateElementBundles();
                
            });
            
            var type =  this.Q<DropdownField>("Type");
            type.choices = displayChoices.Keys.ToArray().ToList();
            type.RegisterValueChangedCallback(evt =>
            {
                _populationFilter = evt.newValue as string;
                UpdateElementBundles();
            });

            type.SetValueWithoutNotify(_populationFilter); 
            
            
            bundlePallete = this.Q<SimplePallete>("ConnectionPallete");
            bundlePallete.DisplayAddElement = false;
            UpdateElementBundles();
            SetPallete();
            bundlePallete.Repaint();
            return this;
        }

        private void SetPallete()
        {
            // Set init options
            bundlePallete.ShowGroups = false;
            bundlePallete.ShowAddButton = false;
            bundlePallete.ShowRemoveButton = false;
            
            bundlePallete.OnSelectOption += (selected) =>
            {
                _target.selectedToSet = selected as Bundle;
                _target.selectedCollectionToSet = _collection;
                _target.selectedTypeFilter = _populationFilter;
                /*
                if (selected == null)
                {
                    _target.selectedCollectionToSet = null;
                    _target.selectedTypetoSet = PopulationType.Entity;
                }
                else
                {
                    _target.selectedCollectionToSet = _collection;
                    _target.selectedTypetoSet = _populationType;
                }
                */
             
                ToolKit.Instance.SetActive("Paint Tile");
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

            bundlePallete.OnRepaint += () =>
            {
                bundlePallete.Selected = _target.selectedToSet;
                bundlePallete.CollectionSelected = _target.selectedCollectionToSet;
            };
            
            

        }

        private void UpdateElementBundles()
        {
            if (_collection == null)
            {
                warningPanel.SetDisplay(true);
                bundlePallete.DisplayContent(false);
                return;
            }
            warningPanel.SetDisplay(false);
            bundlePallete.DisplayContent(true);
            var bundles = _collection.Collection;
            var candidates = new List<Bundle>();
            if (_populationFilter == allFilter)
            {
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element).ToList();
            }
            else
            {
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element && b.PopulationType == displayChoices[_populationFilter][0]) // get the bundle type at the filter index
                    .ToList();
            }
            bundlePallete.ShowGroups = false;
            var options = new object[candidates.Count];
            for (int i = 0; i < candidates.Count; i++)
            {
                options[i] = candidates[i];
            }
         
            // Init options
            bundlePallete.SetOptions(options, (optionView, option) =>
            {
                var bundle = (Bundle)option;
                optionView.Label = bundle.name;
                optionView.Color = bundle.Color;
                optionView.Icon = bundle.Icon;
            });
            
            bundlePallete.Repaint();
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

            bundlePallete.OnRepaint += () =>
            {
                bundlePallete.Selected = _target.selectedToSet;
            };

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