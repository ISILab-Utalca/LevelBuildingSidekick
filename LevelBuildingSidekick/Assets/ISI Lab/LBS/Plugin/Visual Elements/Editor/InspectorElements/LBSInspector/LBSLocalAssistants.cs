using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Editor;

namespace ISILab.LBS.VisualElements
{
    public class LBSLocalAssistants : LBSInspector
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<LBSLocalAssistants, UxmlTraits> { }
        #endregion

        private Color color => LBSSettings.Instance.view.assitantsColor;

        private VisualElement content;
        private VisualElement noContentPanel;
        private VisualElement contentAssist;

        public List<LBSCustomEditor> CustomEditors = new List<LBSCustomEditor>();

        private LBSLayer target;

        public LBSLocalAssistants()
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalAssistants");
            visualTree.CloneTree(this);

            content = this.Q<VisualElement>("Content");
            noContentPanel = this.Q<VisualElement>("NoContentPanel");
            contentAssist = this.Q<VisualElement>("ContentAssist");

            this.Q<Button>("Add").SetEnabled(false);
        }

        public void SetInfo(LBSLayer target)
        {
            contentAssist.Clear();

            this.target = target;

            if (target.Assitants.Count <= 0)
            {
                noContentPanel.SetDisplay(true);
                return;
            }

            noContentPanel.SetDisplay(false);

            foreach (var assist in target.Assitants)
            {
                var type = assist.GetType();
                var ves = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type));

                if (ves.Count() == 0)
                {
                    Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                    continue;
                }

                var ovg = ves.First().Item1;
                var ve = Activator.CreateInstance(ovg, new object[] { assist });
                if (!(ve is VisualElement))
                {
                    Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                    continue;
                }

                CustomEditors.Add(ve as LBSCustomEditor);

                var content = new BehaviourContent(ve as LBSCustomEditor, assist.Name, assist.Icon, color);
                contentAssist.Add(content);

                assist.OnTermination += () =>
                {
                    LBSInspectorPanel.Instance.SetTarget(assist.Owner);
                    Debug.Log("OnTermination");
                };
            }
        }

        public override void Repaint()
        {
            foreach (var ve in CustomEditors)
            {
                ve?.Repaint();
            }
        }

        public override void SetTarget(LBSLayer layer)
        {
            SetInfo(layer);
        }
    }
}