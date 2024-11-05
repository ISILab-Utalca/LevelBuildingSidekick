using ISILab.LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISILab.LBS.Modules;
using ISILab.LBS.Editor;
using UnityEngine.UIElements;


// GABO TODO: TERMINAR
namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSTile", typeof(PathOSTile))]
    public class PathOSTileEditor : LBSCustomEditor
    {
        public override void SetInfo(object target)
        {
            this.target = target as PathOSTile;
            CreateVisualElement();
        }

        protected override VisualElement CreateVisualElement()
        {
            Add(new PathOSTriggerInfoPanel());
            return this;
        }
    }

}