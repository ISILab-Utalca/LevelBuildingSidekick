using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Editor;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        }

        public override void SetTarget(LBSLayer target)
        {
            Clear();
            this.target = target;
            
            ToolKit.Instance.InitGeneralTools(this.target);
            
            modulesPanel.SetInfo(target.Modules);
            layerInfoView.SetInfo(target);
        }

        public override void Repaint()
        {
            MarkDirtyRepaint();
            if(target is not null)SetTarget(target);
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