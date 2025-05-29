using System;
using System.Collections.Generic;
using ISILab.LBS.Editor;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public abstract class LBSInspector : VisualElement
    {
        /// <summary>
        /// Dictionary for behaviour, assistants, it assumes each one only has 1 editor!
        /// </summary>
        protected Dictionary<Type, Type> customEditor = new();
        protected VisualElement noContentPanel;
        protected VisualElement contentPanel;
        
        /// <summary>
        /// Gets the classes of editors per component, no avoid using reflection on each instance creation
        /// </summary>
        /// <param name="layer"></param>
        public abstract void InitCustomEditors(ref List<LBSLayer> layers);
        /// <summary>
        /// Sets the active layer into the panel to update the different components of a layer, such as modules,
        /// behaviours, assistants and toolkit. 
        /// </summary>
        /// <param name="layer"></param>
        public abstract void SetTarget(LBSLayer layer);
        /// <summary>
        /// Markes the panel as dirty and calls resetTarget
        /// <param name="layer"></param>
        public virtual void Repaint() 
        {
            Debug.LogWarning("[ISILab]: The inspector (" + ToString() + ") does not implement repainting.");
        }
    }
}