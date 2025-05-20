using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements;
using LBS;
using LBS.Bundles;
using ISILab.LBS.Settings;
using LBS.VisualElements;
using System.Collections.Generic;
using System.Resources;
using ISILab.Macros;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours.Editor
{

    [LBSCustomEditor("Schema Behaviour", typeof(SchemaBehaviour))]
    public class SchemaBehaviourEditor : LBSCustomEditor, IToolProvider
    {

        #region FIELDS
        private SchemaBehaviour _target;

        private AddSchemaTile addSchemaTile;
        private RemoveSchemaTile removeSchemaTile;

        private AddSchemaTileConnection addTileConnection;
        private RemoveTileConnection removeTileConnection;
        #endregion

        #region VIEW FIELDS
        private VectorImage icon = Resources.Load<VectorImage>("Icons/Vectorial/Icon=Behavior");
        private SimplePallete areaPallete;
        private SimplePallete connectionPallete;
        private string zoneIconGuid = "76bf813a38668ce439887addd209058c";
        private string windowConnectionIconGuid = "c0d00de1d82858c4b9d772a012caf67d";
        private string doorConnectionIconGuid = "cd77d8067cf8b6b44ab23da9a62173c0";
        private string wallConnectionIconGuid = "b29ab5d90498432409a5fb48f6be7bd5";
        private string emptyConnectionIconGuid = "072eebdede709814ea347b1cde4b51a2";

        #endregion

        #region PROPERTIES
        private Color BHcolor => LBSSettings.Instance.view.behavioursColor;
        #endregion

        #region CONSTRUCTORS

        public SchemaBehaviourEditor(object target) : base(target)
        {
            this._target = target as SchemaBehaviour;

            CreateVisualElement();
        }


        #endregion

        #region METHODS
        public void SetTools(ToolKit toolKit)
        {
            addSchemaTile = new AddSchemaTile();
            var t1 = new LBSTool(addSchemaTile);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t1.OnEnd += l => areaPallete.Repaint();
            t1.Init(_target.OwnerLayer, _target);
            
            removeSchemaTile = new RemoveSchemaTile();
            var t2 = new LBSTool(removeSchemaTile);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t2.Init(_target.OwnerLayer, _target);
            
            addTileConnection = new AddSchemaTileConnection();
            var t3 = new LBSTool(addTileConnection);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t3.Init(_target.OwnerLayer, _target);
            
            removeTileConnection = new RemoveTileConnection();
            var t4 = new LBSTool(removeTileConnection);
            t4.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t4.Init(_target.OwnerLayer, _target);
            
            addSchemaTile.SetRemover(removeSchemaTile);
            addTileConnection.SetRemover(removeTileConnection);
            
            toolKit.AddTool(t1);
            toolKit.AddTool(t2);
            toolKit.AddTool(t3);
            toolKit.AddTool(t4);
        }

        public override void SetInfo(object target)
        {
            this._target = target as SchemaBehaviour;
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("SchemaBehaviourEditor");
            visualTree.CloneTree(this);

            // Area Pallete
            this.areaPallete = this.Q<SimplePallete>("ZonePallete");
            SetAreaPallete();

            // Connection Pallete
            this.connectionPallete = this.Q<SimplePallete>("ConnectionPallete");
            SetConnectionPallete();

            // Inside Field
            var insideField = this.Q<ObjectField>("InsideField");
            insideField.value = _target.PressetInsideStyle;
            insideField.RegisterValueChangedCallback(evt =>
            {
                _target.PressetInsideStyle = evt.newValue as Bundle;
            });

            // Outside Field
            var outsideField = this.Q<ObjectField>("OutsideField");
            outsideField.value = _target.PressetOutsideStyle;
            outsideField.RegisterValueChangedCallback(evt =>
            {
                _target.PressetOutsideStyle = evt.newValue as Bundle;
            });

            return this;
        }

        private void SetAreaPallete()
        {
            if (areaPallete == null) return;
            
            areaPallete.ShowGroups = false;
            areaPallete.SetName("Zones");
            areaPallete.SetIcon(icon, BHcolor);

            if (!_target.ValidArea) return;
            
            var zones = _target.Zones;
            var options = new object[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                options[i] = zones[i];
            }

            // Select option event
            areaPallete.OnSelectOption += (selected) =>
            {
                _target.RoomToSet = selected as Zone;
                //ToolKit.Instance.SetActive("Paint Zone");
                //Debug.Log("Hi");
            };

            // OnAdd option event
            areaPallete.OnAddOption += () =>
            {
                var newZone = _target.AddZone();
                newZone.InsideStyles = new List<string>() { _target.PressetInsideStyle.Name };
                newZone.OutsideStyles = new List<string>() { _target.PressetOutsideStyle.Name };
                areaPallete.Options = new object[_target.Zones.Count];
                for (int i = 0; i < _target.Zones.Count; i++)
                {
                    areaPallete.Options[i] = _target.Zones[i];
                }
                _target.RoomToSet = newZone;
                areaPallete.Repaint();
            };

            // Init options
            areaPallete.SetOptions(options, (optionView, option) =>
            {
                var area = (Zone)option;
                optionView.Label = area.ID; // ID or name (??)
                optionView.Color = area.Color;
                optionView.Icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(zoneIconGuid);

            });

            areaPallete.OnRepaint += () =>
            {
                var zones = _target.Zones;
                areaPallete.Options = new object[zones.Count];
                for (int i = 0; i < zones.Count; i++)
                {
                    areaPallete.Options[i] = _target.Zones[i];
                }
            };

            areaPallete.OnRemoveOption += (option) =>
            {
                if (option == null)
                    return;

                var answer = EditorUtility.DisplayDialog("Caution",
                    "You are about to delete a zone, which may be related" +
                    " to tiles on your map. If you delete the zone," +
                    " the corresponding tiles will also be removed." +
                    " Are you sure you want to proceed?", "Continue", "Cancel");

                if (!answer)
                    return;

                _target.RemoveZone(option as Zone);
                ToolKit.Instance.SetActive(typeof(AddSchemaTile));

                DrawManager.ReDraw();
                areaPallete.Repaint();
            };
            
            areaPallete.OnRepaint += () => { areaPallete.Selected = _target.RoomToSet; };

            areaPallete.Repaint();
        }
        
        private void SetConnectionPallete()
        {
            connectionPallete.ShowGroups = false;
            connectionPallete.ShowRemoveButton = false;
            connectionPallete.ShowAddButton = false;
            connectionPallete.ShowNoElement = false;
            
            connectionPallete.SetName("Connections");
            connectionPallete.SetIcon(icon, BHcolor);

  
            var connections = _target.Connections;
            var options = new object[connections.Count];
            for (int i = 0; i < connections.Count; i++)
            {
                options[i] = connections[i];
            }
            
            // Select option event
            connectionPallete.OnSelectOption += (selected) =>
            {
                // var tk = ToolKit.Instance;
                _target.conectionToSet = selected as string;
                //setTileConnection.ToSet = selected as string;
                ToolKit.Instance.SetActive(typeof(AddSchemaTileConnection));
            };

            // Init options}
            
            connectionPallete.SetOptions(options, (optionView, option) =>
            {
                var arg1Label = (string)option;
                optionView.Label = arg1Label;
                optionView.Icon = GetOptionIcon(arg1Label);

            });
            
            
            connectionPallete.OnRepaint += () => { connectionPallete.Selected = _target.conectionToSet; };

            connectionPallete.Repaint();
        }

        VectorImage GetOptionIcon(string label)
        {
            if(label == "Empty") return LBSAssetMacro.LoadAssetByGuid<VectorImage>(emptyConnectionIconGuid);
            if(label == "Wall") return LBSAssetMacro.LoadAssetByGuid<VectorImage>(wallConnectionIconGuid);
            if(label == "Door") return LBSAssetMacro.LoadAssetByGuid<VectorImage>(doorConnectionIconGuid);
            if(label == "Window") return LBSAssetMacro.LoadAssetByGuid<VectorImage>(windowConnectionIconGuid);
            return LBSAssetMacro.LoadAssetByGuid<VectorImage>(zoneIconGuid);
        }
        
        #endregion
    }
}