using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ModulesPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ModulesPanel, VisualElement.UxmlTraits> { }
        #endregion

        private Foldout foldout;
        private VisualElement content;

        public ModulesPanel()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ModulesPanel");
            visualTree.CloneTree(this);

            this.foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                content.SetDisplay(evt.newValue);
            });

            this.content = this.Q<VisualElement>("Content");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modules"></param>
        public void SetInfo(List<LBSModule> modules)
        {
            content.Clear();

            foreach (var module in modules)
            {
                // Create view
                var view = new ModuleSimpleView();

                // Set data
                view.SetInfo(module.GetType().ToString(), module.ID);

                // Add to panel
                content.Add(view);
            }
        }
    }
}
