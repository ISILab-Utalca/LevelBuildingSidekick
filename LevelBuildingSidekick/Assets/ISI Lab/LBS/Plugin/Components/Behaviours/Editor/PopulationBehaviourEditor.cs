using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using LBS;
using LBS.Bundles;
using ISILab.LBS.Settings;
using LBS.VisualElements;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PopulationBehaviour", typeof(PopulationBehaviour))]
    public class PopulationBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS

        private PopulationBehaviour behaviour;

        private Dictionary<string, List<Bundle.PopulationTypeE>> displayChoices = new();
        private BundleCollection _collection; 
        private DropdownField type;
        
        AddPopulationTile addPopulationTile;
        RemovePopulationTile removePopulationTile;
        RotatePopulationTile rotatePopulationTile;
        
        #region VIEW FIELDS
        private readonly Color BHcolor = LBSSettings.Instance.view.behavioursColor;
        private VectorImage icon = Resources.Load<VectorImage>("Icons/Vectorial/Icon=Behavior");
        private SimplePallete bundlePallete;
        private WarningPanel warningPanel;
        #endregion
        
        #endregion
        
        #region CONSTRUCTORS
        public PopulationBehaviourEditor(object target) : base(target)
        {
            behaviour = target as PopulationBehaviour;
            //_collection = load default collection
            
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
            
            _collection = behaviour.BundleCollection;
            behaviour.SelectedFilter = behaviour.allFilter; 
            displayChoices.Add(behaviour.allFilter, allList);
            displayChoices.Add(Bundle.PopulationTypeE.Character.ToString(), characterList);
            displayChoices.Add(Bundle.PopulationTypeE.Item.ToString(), itemList);
            displayChoices.Add(Bundle.PopulationTypeE.Interactable.ToString(), interactableList);
            displayChoices.Add(Bundle.PopulationTypeE.Area.ToString(), areaList);
            displayChoices.Add(Bundle.PopulationTypeE.Prop.ToString(), propList);
            displayChoices.Add(Bundle.PopulationTypeE.Misc.ToString(), miscList);

            SetInfo(behaviour);
            CreateVisualElement();
        }
        #endregion
        
        #region METHODS
        public sealed override void SetInfo(object paramTarget)
        {
            behaviour = paramTarget as PopulationBehaviour;
            if(behaviour == null) return;
            _collection = behaviour.BundleCollection;
        }

        public void SetTools(ToolKit toolkit)
        {

            addPopulationTile = new AddPopulationTile();
            var t1 = new LBSTool(addPopulationTile);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t1.OnEnd += (l) => DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
            
            removePopulationTile = new RemovePopulationTile();
            var t2 = new LBSTool(removePopulationTile);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t2.OnEnd += (l) => DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
            
            // Rotate element
            rotatePopulationTile = new RotatePopulationTile();
            var t3 = new LBSTool(rotatePopulationTile);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t3.OnEnd += (l) => DrawManager.Instance.RedrawLayer(behaviour.OwnerLayer, MainView.Instance);
            
            addPopulationTile.SetRemover(removePopulationTile);
            
            toolkit.ActivateTool(t1,behaviour.OwnerLayer, behaviour);
            toolkit.ActivateTool(t2,behaviour.OwnerLayer, behaviour);
            toolkit.ActivateTool(t3,behaviour.OwnerLayer, behaviour);
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationBehaviourEditor");
            visualTree.CloneTree(this);
            
            // WarningPanel
            warningPanel = this.Q<WarningPanel>();
            
            var collectionField = this.Q<ObjectField>("BundleCollection");
            // only updates the first bundle value change - fix pending
            collectionField.RegisterValueChangedCallback(evt =>
            {
                var collection = evt.newValue as BundleCollection;
                collectionField.value = collection;
                SetCollection(collection);
                UpdateElementBundles();
                
            });
            
            type =  this.Q<DropdownField>("Type");
            type.choices = displayChoices.Keys.ToArray().ToList();
            type.RegisterValueChangedCallback(evt =>
            {
               var filter = evt.newValue;
               behaviour.selectedTypeFilter = filter; 
                UpdateElementBundles();
            });

            type.SetValueWithoutNotify(behaviour.SelectedFilter); 
            
            
            bundlePallete = this.Q<SimplePallete>("ConnectionPallete");
            bundlePallete.DisplayAddElement = false;
            UpdateElementBundles();
            SetPallete();
            bundlePallete.Repaint();
            
            collectionField.SetValueWithoutNotify(behaviour.BundleCollection);
            
            MarkDirtyRepaint();
            
            return this;
        }

        private void SetPallete()
        {
            // Set init options
            bundlePallete.ShowGroups = false;
            bundlePallete.ShowAddButton = false;
            bundlePallete.ShowRemoveButton = false;
            bundlePallete.ShowNoElement = false;
            
            bundlePallete.Repaint();
            
            bundlePallete.OnSelectOption += (selected) =>
            {
                behaviour.selectedToSet = selected as Bundle;
                behaviour.BundleCollection = _collection;
             
                ToolKit.Instance.SetActive(typeof(AddPopulationTile));
            };
            
            bundlePallete.OnSetTooltip += (option) =>
            {
                var b = option as Bundle;

                var tooltip = "Tags:";
                if (b.Characteristics.Count > 0)
                {
                    b.Characteristics.ForEach(c => tooltip += "\n- " + c?.GetType());
                }
                else
                {
                    tooltip += "\n[None]";
                }
                return tooltip;
            };

            bundlePallete.OnRepaint += () =>
            {
                bundlePallete.Selected = behaviour.selectedToSet;
                bundlePallete.CollectionSelected = behaviour.BundleCollection;
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
            
            type.SetValueWithoutNotify(behaviour.SelectedFilter); 
            warningPanel.SetDisplay(false);
            bundlePallete.DisplayContent(true);
            var bundles = _collection.Collection;
            var candidates = new List<Bundle>();
            if (type.value == behaviour.allFilter)
            {
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element).ToList();
            }
            else
            {
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element && b.PopulationType == displayChoices[type.value][0]) // get the bundle type at the filter index
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
            
            // Save current selected options in layer
            behaviour.BundleCollection = _collection;
            
            bundlePallete.Repaint();
            
            
        }

        private void SetCollection(BundleCollection collection)
        {
            behaviour.BundleCollection = collection;
            _collection = collection;
        }

        #endregion
    }
}