using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Editor;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSLocalCurrent : LBSInspector
    {
        private LBSLayer target;
        
        private ModulesPanel modulesPanel;
        private LayerInfoView layerInfoView;

        #region CONSTRUCTORS
        public LBSLocalCurrent()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalCurrent");
            visualTree.CloneTree(this);

            contentPanel = this.Q<VisualElement>("SelectedContent");

            modulesPanel = this.Q<ModulesPanel>();
            layerInfoView = this.Q<LayerInfoView>();
        }
        #endregion
        
        #region METHODS
        public override void InitCustomEditors(ref List<LBSLayer> layers)
        {
            foreach (LBSLayer reflayer in layers)
            {
                if (reflayer == null) continue;
                foreach (var module in reflayer.Modules)
                {
                    Type type = module.GetType();
                    var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                        .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                    if (!ves.Any())
                    {
                     //   Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                        continue;
                    }

                    Type moduleEditorType = ves.First().Item1;
                    if (moduleEditorType == null) continue;
                    customEditor.Add(type, moduleEditorType);
                }
            }
        }

        public override void SetTarget(LBSLayer layer)
        {
            noContentPanel.SetDisplay(layer is null);
            contentPanel.Clear();
            target = layer;

            if (layer == null)
            {
                layerInfoView.SetInfo(null); // no layer, hide info
                modulesPanel.SetInfo(new List<LBSModule>()); //pass an empty list
                return;    
            }
            
            noContentPanel.SetDisplay(!target.Modules.Any());
            
            ToolKit.Instance.InitGeneralTools(target);
            
            modulesPanel.SetInfo(target.Modules);
            layerInfoView.SetInfo(target);
        }

        public override void Repaint()
        {
            if(target is not null) SetTarget(target);
            MarkDirtyRepaint();
        }

        public void SetSelectedVE(List<object> objs)
        {
            contentPanel.Clear();

            foreach (var obj in objs)
            {
                // Check if obj is valid
                if (obj == null)
                {
                    contentPanel.Add(new Label("[NULL]"));
                    continue;
                }

                // Get type of element
                var type = obj.GetType();

                // Get the editors of the selectable elements
                var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                        .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (ves.Count <= 0)
                {
                    // Add basic label if no have specific editor
                    contentPanel.Add(new Label("'" + type + "' does not contain a visualization."));
                    continue;
                }

                // Get editor class
                var edtr = ves.First().Item1;

                // Instantiate editor class
                var ve = Activator.CreateInstance(edtr) as LBSCustomEditor;

                // set target info on visual element
                ve.SetInfo(obj);
                ToolKit.Instance.SetTarget(ve);
                
                // create content container
                var container = new DataContent(ve, ves.First().Item2.First().name);
                contentPanel.Add(container);
            }
        }
        #endregion
    }
}