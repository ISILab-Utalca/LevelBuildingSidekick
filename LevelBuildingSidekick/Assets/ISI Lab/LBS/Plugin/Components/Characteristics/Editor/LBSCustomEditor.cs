using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Editor
{
    public abstract class LBSCustomEditor : VisualElement
    {
        #region FIELDS
        protected object target;
        #endregion

        public object Target => target;

        #region CONSTRUCTORS
        public LBSCustomEditor() { }

        public LBSCustomEditor(object target)
        {
            this.target = target;
        }
        #endregion

        #region METHODS
        public virtual void ContextMenu(ContextualMenuPopulateEvent evt) { }

        public virtual void Repaint() { }

        public virtual void OnFocus() { Debug.Log("On Focus: " + target.ToString()); }


        public abstract void SetInfo(object paramTarget);

        protected abstract VisualElement CreateVisualElement();

        #endregion

    }
}