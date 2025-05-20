using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Editor;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSLocalBehaviours : LBSInspector
    {
        private LBSLayer target;
        
        private VisualElement noContentPanel;
        private VisualElement contentBehaviour;
        

        #region CONSTRUCTORS
        public LBSLocalBehaviours()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
            visualTree.CloneTree(this);

            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentBehaviour = this.Q<VisualElement>("ContentBehaviour");

            this.Q<Button>("Add").SetEnabled(false);
            this.Q<Button>("Add").SetDisplay(false);
        }
        #endregion


        #region METHODS
        public override void InitCustomEditors(ref List<LBSLayer> layers)
        {
            foreach (LBSLayer reflayer in layers)
            {
               // var layer = reflayer.Clone() as LBSLayer;
                if (reflayer == null) continue;
                foreach (var behaviour in reflayer.Behaviours)
                {
                    Type type = behaviour.GetType();
                    var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                        .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                    if (!ves.Any())
                    {
                        Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                        continue;
                    }

                    Type behaviourEditorType = ves.First().Item1;
                    if (behaviourEditorType == null) continue;
                    customEditor.Add(type, behaviourEditorType);

                   
                }
            }
        }

        public override void SetTarget(LBSLayer layer)
        {

            if (layer == null)
                return;
            
            //  this.content.Clear();
            // TODO UNCOMMENT THIS? contentBehaviour.Clear();
            
            target = layer;
            noContentPanel.SetDisplay(!target.Behaviours.Any());
            visualElements.Clear();
            
            // Add the tools into the toolkit and set the data of behaviour
            ToolKit.Instance.AddSeparator();
            foreach (var behaviour in target.Behaviours)
            {
                Type editorType = customEditor.GetValueOrDefault(behaviour.GetType());
                if(editorType == null) continue;
                LBSCustomEditor instance = Activator.CreateInstance(editorType, behaviour) as LBSCustomEditor;
                ToolKit.Instance.SetTarget(instance);
                var content = new InspectorContentPanel(instance, behaviour.Name, behaviour.Icon, behaviour.ColorTint);
                contentBehaviour.Add(content);
                visualElements.Add(instance);
            }

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
        #endregion
    }
}