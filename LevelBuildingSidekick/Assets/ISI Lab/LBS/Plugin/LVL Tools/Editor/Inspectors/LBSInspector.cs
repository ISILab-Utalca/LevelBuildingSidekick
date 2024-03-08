using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public abstract class LBSInspector : VisualElement
    {
        public abstract void SetTarget(LBSLayer layer);

        public virtual void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour) { } // FIX: This function is not called anywhere, it is not working

        public virtual void Repaint() 
        {
            Debug.LogWarning("[ISILab]: The inspector (" + this.ToString() + ") does not implement repainting.");
        }
    }
}