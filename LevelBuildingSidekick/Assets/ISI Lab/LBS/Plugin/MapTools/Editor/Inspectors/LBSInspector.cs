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

        protected List<LBSCustomEditor> visualElements = new();
        
        public abstract void InitCustomEditors(ref List<LBSLayer> layers);
        public abstract void SetTarget(LBSLayer layer);
        public virtual void Repaint() 
        {
            Debug.LogWarning("[ISILab]: The inspector (" + ToString() + ") does not implement repainting.");
        }
    }
}