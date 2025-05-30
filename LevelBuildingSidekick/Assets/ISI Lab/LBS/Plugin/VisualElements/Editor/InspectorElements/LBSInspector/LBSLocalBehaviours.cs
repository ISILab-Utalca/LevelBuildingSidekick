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
using ISILab.Macros;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class LBSLocalBehaviours : LBSInspector
    {
        #region FACTORY
     //   public new class UxmlFactory : UxmlFactory<LBSLocalBehaviours, UxmlTraits> { }
        #endregion

        #region FIELDS
        private LBSLayer target;
        #endregion

        #region VIEW FIELDS
        private VisualElement content;
        private VisualElement noContentPanel;
        private VisualElement contentBehaviour;

        public List<LBSCustomEditor> CustomEditors = new List<LBSCustomEditor>();
        #endregion

        #region PROPERTIES
        public Color color;//=> LBSSettings.Instance.view.behavioursColor;
        private ToolKit toolkit => ToolKit.Instance;
        #endregion

        #region CONSTRUCTORS
        public LBSLocalBehaviours()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
            visualTree.CloneTree(this);

            content = this.Q<VisualElement>("Content");
            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentBehaviour = this.Q<VisualElement>("ContentBehaviour");

            this.Q<Button>("Add").SetEnabled(false);
            this.Q<Button>("Add").SetDisplay(false);
        }
        #endregion

        #region METHODS
        public void SetInfo(LBSLayer target)
        {
            contentBehaviour.Clear();

            this.target = target;

            if (target == null)
                return;

            if (target.Behaviours.Count <= 0)
            {
                noContentPanel.SetDisplay(true);
                return;
            }

            noContentPanel.SetDisplay(false);

            foreach (var behaviour in target.Behaviours)
            {
                var type = behaviour.GetType();
                var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (!ves.Any())
                {
                    Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                    continue;
                }

                var ovg = ves.First().Item1;
                if(ovg == null) continue;
                 var ve = Activator.CreateInstance(ovg, new object[] { behaviour });
                if (!(ve is VisualElement))
                {
                    Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                    continue;
                }

                CustomEditors.Add(ve as LBSCustomEditor);
                
                var content = new BehaviourContent(ve as LBSCustomEditor, behaviour.Name, behaviour.Icon, behaviour.ColorTint);
                contentBehaviour.Add(content);

            }
        }

        public override void SetTarget(LBSLayer layer)
        {
            SetInfo(layer);
        }

        public override void Repaint()
        {
            foreach (var ve in CustomEditors)
            {
                ve?.Repaint();
            }

            SetInfo(target);
        }
        #endregion
    }
}