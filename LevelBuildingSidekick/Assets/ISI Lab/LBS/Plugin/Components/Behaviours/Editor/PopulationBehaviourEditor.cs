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
using ISILab.LBS.Characteristics;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PopulationBehaviour", typeof(PopulationBehaviour))]
    public class PopulationBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS

        private PopulationBehaviour behaviour;

        private Dictionary<string, List<Bundle.EElementFlag>> displayChoices = new();
        private BundleCollection _collection; 
        private DropdownField type;

        private AddPopulationTile addPopulationTile;
        private RemovePopulationTile removePopulationTile;
        private RotatePopulationTile rotatePopulationTile;
        private MovePopulationTile movePopulationTile;
        
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
            if (behaviour is null) return;
            //_collection = load default collection
            
            List<Bundle.EElementFlag> characterList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Character };
            List<Bundle.EElementFlag> itemList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Item };
            List<Bundle.EElementFlag> interactableList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Interactable };
            List<Bundle.EElementFlag> areaList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Trigger };
            List<Bundle.EElementFlag> propList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Prop };
            List<Bundle.EElementFlag> miscList = new List<Bundle.EElementFlag> { Bundle.EElementFlag.Misc };
            List<Bundle.EElementFlag> allList = new List<Bundle.EElementFlag>
            {
                Bundle.EElementFlag.Misc,
                Bundle.EElementFlag.Prop,
                Bundle.EElementFlag.Trigger,
                Bundle.EElementFlag.Interactable,
                Bundle.EElementFlag.Item,
                Bundle.EElementFlag.Character
            };
            
            _collection = behaviour.BundleCollection;
            behaviour.SelectedFilter = behaviour.allFilter;
            displayChoices.Add(behaviour.allFilter, allList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Character), characterList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Item), itemList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Interactable), interactableList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Trigger), areaList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Prop), propList);
            displayChoices.Add(nameof(Bundle.EElementFlag.Misc), miscList);

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
            
            removePopulationTile = new RemovePopulationTile();
            var t2 = new LBSTool(removePopulationTile);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            rotatePopulationTile = new RotatePopulationTile();
            var t3 = new LBSTool(rotatePopulationTile);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            movePopulationTile = new MovePopulationTile();
            var t4 = new LBSTool(movePopulationTile);
            t4.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            addPopulationTile.SetRemover(removePopulationTile);
            
            toolkit.ActivateTool(t1,behaviour.OwnerLayer, behaviour);
            toolkit.ActivateTool(t2,behaviour.OwnerLayer, behaviour);
            toolkit.ActivateTool(t4,behaviour.OwnerLayer, behaviour);
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

                string tooltip = "Tags:";

                var tags = b.Characteristics.Select(t => t as LBSTagsCharacteristic);
                List<LBSTagsCharacteristic> validTags = tags.Where(t => t.Value != null).ToList();
                if (validTags.Count > 0)
                {
                    validTags.ForEach(t => tooltip += "\n- " + t.Value.Label);
                    //b.Characteristics.ForEach(c => tooltip += "\n- " + c?.ToString());//.GetType());
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
            List<Bundle> bundles = _collection.Collection;
            List<Bundle> candidates = new();
            if (type.value == behaviour.allFilter)
            {
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element).ToList();
            }
            else
            {
                Bundle.EElementFlag filter = displayChoices[type.value][0];
                HashSet<Bundle.EElementFlag> tags = new HashSet<Bundle.EElementFlag>() { filter};
                candidates = bundles
                    .Where(b => b.Type == Bundle.TagType.Element && b.HasAnyFlag(tags)) // get the bundle type at the filter index
                    .ToList();
            }
            bundlePallete.ShowGroups = false;

            candidates.Sort((b1, b2) => b1.BundleName.CompareTo(b2.BundleName));
            var options = new object[candidates.Count];
            for (int i = 0; i < candidates.Count; i++)
            {
                options[i] = candidates[i];
            }
         
            // Init options
            bundlePallete.SetOptions(options, (optionView, option) =>
            {
                var bundle = (Bundle)option;
                optionView.Label = bundle.BundleName;
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