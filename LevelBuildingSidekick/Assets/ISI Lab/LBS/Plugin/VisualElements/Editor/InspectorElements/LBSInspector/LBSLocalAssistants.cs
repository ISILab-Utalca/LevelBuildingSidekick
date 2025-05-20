using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using LBS.Components;
using ISILab.LBS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Editor;
using ISILab.LBS.Template;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSLocalAssistants : LBSInspector
    {

        private LBSLayer target;
        
        private VisualElement noContentPanel;
        private VisualElement contentAssist;

        #region CONSTRUCTORS
        public LBSLocalAssistants()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalAssistants");
            visualTree.CloneTree(this);
            
            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentAssist = this.Q<VisualElement>("ContentAssist");
            
            this.Q<Button>("Add").SetEnabled(false);
        }
        #endregion
        
        #region METHODS
        public override void InitCustomEditors(ref List<LBSLayer> layers)
        {
            foreach (LBSLayer reflayer in layers)
            {
                var layer = reflayer.Clone() as LBSLayer;
                if (layer == null) continue;
                foreach (var assistant in layer.Assistants)
                {
                    var type = assistant.GetType();
                    var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                        .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                    if (!ves.Any())
                    {
                        Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                        continue;
                    }

                    Type assistantEditorType = ves.First().Item1;
                    if (assistantEditorType == null) continue;
                    customEditor.Add(type, assistantEditorType);
                
                }
            }
        }

        public override void SetTarget(LBSLayer layer)
        {
            visualElements.Clear();
            if (layer == null)
                return;
            
            //  this.content.Clear();
            // TODO UNCOMMENT THIS? contentBehaviour.Clear();
            
            target = layer;
            noContentPanel.SetDisplay(!target.Assistants.Any());
     
            
            // Add the tools into the toolkit and set the data of behaviour
            ToolKit.Instance.AddSeparator();
            foreach (var assistant in target.Assistants)
            {
                Type editorType = customEditor.GetValueOrDefault(assistant.GetType());
                if(editorType == null) continue;
                
                LBSCustomEditor instance = Activator.CreateInstance(editorType, assistant) as LBSCustomEditor;
                ToolKit.Instance.SetTarget(instance);
                var content = new InspectorContentPanel(instance, assistant.Name, assistant.Icon, assistant.ColorTint);
                contentAssist.Add(content);
                visualElements.Add(instance);
                
                assistant.OnTermination += () =>
                {
                    LBSInspectorPanel.Instance.SetTarget(assistant.OwnerLayer);
                    Debug.Log("OnTermination");
                };
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