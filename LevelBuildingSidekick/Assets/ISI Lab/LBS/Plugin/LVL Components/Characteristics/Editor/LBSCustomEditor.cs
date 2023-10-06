using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    public abstract void SetInfo(object target);

    protected abstract VisualElement CreateVisualElement();

    #endregion

}