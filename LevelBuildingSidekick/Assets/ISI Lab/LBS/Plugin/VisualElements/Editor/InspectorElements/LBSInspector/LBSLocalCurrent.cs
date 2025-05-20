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

        private VisualElement selectedContent;
        private ModulesPanel modulesPanel;
        private LayerInfoView layerInfoView;

        #region CONSTRUCTORS
        public LBSLocalCurrent()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalCurrent");
            visualTree.CloneTree(this);

            selectedContent = this.Q<VisualElement>("SelectedContent");

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
            // SetLayer reference
            this.target = target;
            visualElements.Clear();

            ToolKit.Instance.InitGeneralTools(this.target);
            
            modulesPanel.SetInfo(target.Modules);
            layerInfoView.SetInfo(target);
        }

        public override void Repaint()
        {
            if(!visualElements.Any()) return;
            foreach (var ve in visualElements)
            {
                ve.Repaint(); // TODO may comment
            }

            // TODO may uncomment SetTarget(target);
        }

        public void SetSelectedVE(List<object> objs)
        {
            // Clear previous view
           // selectedContent.Clear();

            foreach (var obj in objs)
            {
                // Check if obj is valid
                if (obj == null)
                {
                    selectedContent.Add(new Label("[NULL]"));
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
                    selectedContent.Add(new Label("'" + type + "' does not contain a visualization."));
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
                visualElements.Add(ve);
                // Add custom editor
                selectedContent.Add(container);
            }
        }
        #endregion
    }
}