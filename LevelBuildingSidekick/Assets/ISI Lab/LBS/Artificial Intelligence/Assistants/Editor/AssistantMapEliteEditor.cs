using LBS;
using LBS.VisualElements;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using ISILab.Extensions;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Drawers;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;

namespace ISILab.LBS.AI.Assistants.Editor
{
    //TODO:
    //AssistantMapEliteEditor debe volverse el objeto principal.
    /// <summary>
    /// Reemplazar los VisualElements de Configuration y Content con PopulationAssistantWindow para permitirles ser una ventana independiente.
    /// </summary>

    [LBSCustomEditor("Assistant Map Elite", typeof(AssistantMapElite))]
    public class AssistantMapEliteEditor : LBSCustomEditor, IToolProvider
    {
        MAPEliteConfiguration config;
        MAPEliteContent content;

        private object locker = new object();

        private MapEliteAreaSelector _mapEliteAreaSelector;
        private AssistantMapElite assistant;

        public AssistantMapEliteEditor(object target) : base(target)
        {
            Add(new PopulationAssistantTab(target as AssistantMapElite));
            SetInfo(target);
        }

        private void Run()
        {
            var assitant = target as AssistantMapElite;

            if (assitant is { Running: true })
                return;

            content.Reset();
           
            if (assitant != null)
            {
                assitant.LoadPresset(config.GetPresset());
                if (assitant.RawToolRect.width == 0 || assitant.RawToolRect.height == 0)
                {
                    Debug.LogError("[ISI Lab]: Selected evolution area height or with < 0");
                    return;
                }

                SetBackgorundTexture(assitant.RawToolRect);
                assitant.SetAdam(assitant.RawToolRect);
                assitant.Execute();
            }

            //LBSMainWindow.OnWindowRepaint += RepaintContent; 
        }

        private void RepaintContent()
        {
            content.UpdateContent();
            if (assistant.Finished)
                LBSMainWindow.OnWindowRepaint -= RepaintContent;
        }

        public void ChangePresset()
        {
            var assitant = target as AssistantMapElite;
            assitant.LoadPresset(config.GetPresset());
            content.Reset();
        }

        private void Continue()
        {
            assistant.Continue();
        }

        public override void SetInfo(object paramTarget)
        {
            assistant = paramTarget as AssistantMapElite;
        }

        protected override VisualElement CreateVisualElement()
        {
            var ve = new VisualElement();
            config = new MAPEliteConfiguration();
            content = new MAPEliteContent(assistant);

            config.OnCalculate += Run;
            config.OnContinue += Continue;
            config.OnPressetChange += (p) =>
            {
                ChangePresset();
                ToolKit.Instance.SetActive(typeof(MapEliteAreaSelector));
            };

            content.OnSelectOption += (s) =>
            {
                // Save history version to revert
                var level = LBSController.CurrentLevel;
                Undo.RegisterCompleteObjectUndo(level, "Select Suggestion");
                EditorGUI.BeginChangeCheck();

                // Apply suggestion
                assistant.ApplySuggestion(s);

                // Mark as dirty
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(level);
                }
            };
            
            ve.Add(content);
            ve.Add(config);
            return ve;
        }

        public void SetTools(ToolKit toolkit)
        {
            _mapEliteAreaSelector = new MapEliteAreaSelector((r) => assistant.RawToolRect = r);
            var t1 = new LBSTool(_mapEliteAreaSelector);
            t1.OnSelect += LBSInspectorPanel.ActivateAssistantTab;
            toolkit.ActivateTool(t1,assistant.OwnerLayer, assistant);
        }

        public void SetBackgorundTexture(Rect rect)
        {
            var behaviours = assistant.OwnerLayer.Parent.Layers.SelectMany(l => l.Behaviours);
            var bh = assistant.OwnerLayer.Behaviours.Find(b => b is PopulationBehaviour);

            var size = 16;

            var textures = new List<Texture2D>();

            foreach (var b in behaviours)
            {
                if (b == null)
                    continue;

                if (bh != null && b.Equals(bh))
                    continue;

                var drawerT = LBS_Editor.GetDrawer(b.GetType());
                var drawer = Activator.CreateInstance(drawerT) as Drawer;
                textures.Add(drawer.GetTexture(b, rect, Vector2Int.one * size));
            }

            var texture = new Texture2D((int)(rect.width * size), (int)(rect.height * size));

            for (int j = 0; j < texture.height; j++)
            {
                for (int i = 0; i < texture.height; i++)
                {
                    texture.SetPixel(i, j, new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1));
                }
            }

            for (int i = textures.Count - 1; i >= 0; i--)
            {
                if (textures[i] == null)
                    continue;

                texture = texture.MergeTextures(textures[i]);
            }

            texture.Apply();

            content.background = texture;
        }
    }
}