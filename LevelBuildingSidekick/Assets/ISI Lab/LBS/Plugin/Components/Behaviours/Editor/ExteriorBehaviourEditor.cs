using ISILab.Commons.Utility.Editor;
using LBS;
using LBS.Bundles;
using ISILab.LBS.Settings;
using LBS.VisualElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Internal;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.Macros;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Exterior Behaviour", typeof(ExteriorBehaviour))]
    public class ExteriorBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private ExteriorBehaviour exterior;
   
        private List<LBSIdentifierBundle> Groups;
        private object[] options;

        private AddExteriorTile addExteriorTile;
        private RemoveTileExterior removeTile;
        private SetExteriorTileConnection setConnection;
        private RemoveConnectionInArea removeConnectionInArea;
        #endregion

        #region VIEW FIELDS
        private VectorImage icon =LBSAssetMacro.LoadAssetByGuid<VectorImage>("e17eb0e02534666439fca8ea30b4d4e4");
        private SimplePallete connectionPallete;
        private ObjectField bundleField;
        private WarningPanel warningPanel;
        private string tileIconGuid = "";
        #endregion

        #region PROPERTIES
        private Color BHcolor => LBSSettings.Instance.view.behavioursColor;
        #endregion

        #region CONSTRUCTORS
        public ExteriorBehaviourEditor(object target) : base(target)
        {
            // Set target Behaviour
            exterior = target as ExteriorBehaviour;
            
            //SetInfo(target);
            
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object paramTarget)
        {
            exterior = paramTarget as ExteriorBehaviour;
            CheckTargetBundle();
        }

        public void SetTools(ToolKit toolKit)
        {
            addExteriorTile = new AddExteriorTile();
            var t1 = new LBSTool(addExteriorTile);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            removeTile = new RemoveTileExterior();
            var t2 = new LBSTool(removeTile);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            setConnection = new SetExteriorTileConnection();
            var t3 = new LBSTool(setConnection);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
                        
            addExteriorTile.SetRemover(removeTile);
            
            toolKit.ActivateTool(t1,exterior.OwnerLayer, exterior);
            toolKit.ActivateTool(t2,exterior.OwnerLayer, exterior);
            toolKit.ActivateTool(t3,exterior.OwnerLayer, exterior);
            
       
        }

        private void CheckTargetBundle() 
        {
            var exteriorBundle = exterior.Bundle;
            if ( exteriorBundle == null)
            {
                warningPanel.SetDisplay(true);
                connectionPallete.SetDisplay(false);
            }
            else
            {
                warningPanel.SetDisplay(false);
                connectionPallete.SetDisplay(true);
                SetConnectionPallete(exterior.Bundle);
            }
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ExteriorBehaviourEditor");
            visualTree.CloneTree(this);

            // WarningPanel
            warningPanel = this.Q<WarningPanel>();

            // BundleField
            bundleField = this.Q<ObjectField>("BundleField");
            bundleField.label = "Exterior Tile Bundle";
            bundleField.objectType = typeof(Bundle);
            bundleField.value = exterior.Bundle;
            // only updates the first bundle value change - fix pending
            bundleField.RegisterValueChangedCallback(evt =>
            {
                var bundle = evt.newValue as Bundle;

                System.Action invalidBundleAction = () =>
                {
                    bundleField.value = exterior.Bundle;
                    LBSMainWindow.MessageNotify("Selected bundle was invalid.", LogType.Warning);
                };

                if(bundle)
                {
                    var identifierTags = LBSAssetsStorage.Instance.Get<LBSTag>();
                    var idents = SePalleteConnectionView(bundle, identifierTags);

                    if (idents.Any())
                    {
                        exterior.Bundle = bundle; // valid for exterior
                        var owner = exterior.OwnerLayer;
                        owner.OnChangeUpdate(); // updates the assistant and viceversa
                    }
                    else
                    {
                        invalidBundleAction(); // set default or current if new option not valid
                    }
                }
                else
                {
                    invalidBundleAction(); // set default or current if new option not valid
                }

                CheckTargetBundle();
            });

            // Connection Pallete
            connectionPallete = this.Q<SimplePallete>("ConnectionPallete");
            CheckTargetBundle();

            exterior.OwnerLayer.OnChange += () =>
            {
                bundleField.SetValueWithoutNotify(exterior.Bundle);
                CheckTargetBundle();
            };
            
            return this;
        }

        private void SetConnectionPallete(Bundle bundle)
        {
            if (bundle == null) return;

            connectionPallete.style.display = DisplayStyle.Flex;
            
            // Set init options
            connectionPallete.ShowGroups = true;
            connectionPallete.ShowAddButton = false;
            connectionPallete.ShowRemoveButton = false;
            connectionPallete.ShowDropdown = false;
            connectionPallete.ShowNoElement = false;
            
            // Set basic value
            connectionPallete.SetName("Tile Brushes");
            connectionPallete.SetIcon(icon, BHcolor);
            
            var identifierTags = LBSAssetsStorage.Instance.Get<LBSTag>();
            var idents = SePalleteConnectionView(bundle, identifierTags);

            exterior.identifierToSet = idents[0];
            
            // Selected option event
            connectionPallete.OnSelectOption += (selected) =>
            {
                exterior.identifierToSet = selected as LBSTag;
                // by default set the 
                var activeManipulator = ToolKit.Instance.GetActiveManipulator().GetType();
                if ( activeManipulator != typeof(AddExteriorTile) &&
                     activeManipulator != typeof(SetExteriorTileConnection))
                {
                    ToolKit.Instance.SetActive(typeof(AddExteriorTile));
                }
            };

            // Init options
            connectionPallete.SetOptions(options, (optionView, option) =>
            {
                var identifier = option as LBSTag;
                optionView.Label = identifier.Label;
                optionView.Color = identifier.Color;
                optionView.Icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(tileIconGuid);
                // optionView.Icon = identifier.Icon;
            });

            connectionPallete.OnRepaint += () => { connectionPallete.Selected = exterior.identifierToSet; };

            connectionPallete.Repaint();
        }

        private List<LBSTag> SePalleteConnectionView(Bundle bundle, List<LBSTag> identifierTags)
        {
            var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
            var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
            if (tags.Remove("Empty"))  tags.Insert(0, "Empty");
            var idents = tags.Select(s => identifierTags.Find(i => s == i.Label)).ToList().RemoveEmpties();
            
            // Set Options
            options = new object[idents.Count];
            for (int i = 0; i < idents.Count; i++)
            {
                if (idents[i] == null)
                    continue;

                options[i] = idents[i];
            }

            return idents;
        }

        #endregion
    }
}