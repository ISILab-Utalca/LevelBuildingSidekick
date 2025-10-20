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
        private LBSLayer _target;

        #region CONSTRUCTORS
        public LBSLocalBehaviours()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
            visualTree.CloneTree(this);

            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentPanel = this.Q<VisualElement>("ContentBehaviour");

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
                    if (customEditor.ContainsKey(type)) continue;
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
            noContentPanel.SetDisplay(layer is null);
            contentPanel.Clear();
            _target = layer;
            
            if (layer == null)
                return;
            
            noContentPanel.SetDisplay(!_target.Behaviours.Any());

            OnFocus = null;
            
            // Add the tools into the toolkit and set the data of behaviour
            foreach (var behaviour in _target.Behaviours)
            {
                Type editorType = customEditor.GetValueOrDefault(behaviour.GetType());
                if(editorType == null) continue;
                LBSCustomEditor instance = Activator.CreateInstance(editorType, behaviour) as LBSCustomEditor;
                
                instance?.SetInfo(behaviour);
                ToolKit.Instance.SetTarget(instance);

                OnFocus += instance.OnFocus;
                
                var content = new InspectorContentPanel(instance, behaviour.Name, behaviour.Icon, behaviour.ColorTint);
                contentPanel.Add(content);
  
            }
        }

        public override void Repaint()
        {
            if(_target is not null)SetTarget(_target);
            MarkDirtyRepaint();
        }
        #endregion
    }
}