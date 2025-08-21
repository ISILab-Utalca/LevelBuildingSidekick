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
        
        #region CONSTRUCTORS
        public LBSLocalAssistants()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalAssistants");
            visualTree.CloneTree(this);
            
            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentPanel = this.Q<VisualElement>("ContentAssist");
            
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
            noContentPanel.SetDisplay(layer is null);
            contentPanel.Clear();
            target = layer;
            
            if (layer == null)
                return;
            
            noContentPanel.SetDisplay(!target.Assistants.Any());

            OnFocus = null;
            
            // Add the tools into the toolkit and set the data of behaviour
            foreach (var assistant in target.Assistants)
            {
                Type editorType = customEditor.GetValueOrDefault(assistant.GetType());
                if(editorType == null) continue;
                
                LBSCustomEditor instance = Activator.CreateInstance(editorType, assistant) as LBSCustomEditor;
              
                instance.SetInfo(assistant);
                ToolKit.Instance.SetTarget(instance);

                OnFocus += instance.OnFocus;
                
                var content = new InspectorContentPanel(instance, assistant.Name, assistant.Icon, assistant.ColorTint);
                contentPanel.Add(content);
                assistant.OnTermination += () =>
                {
                    LBSInspectorPanel.Instance.SetTarget(assistant.OwnerLayer);
                    Debug.Log("OnTermination");
                };
            }
         
        }
        

        public override void Repaint()
        {
            if(target is not null) SetTarget(target);
            MarkDirtyRepaint();
        }
        #endregion
    }
    
}