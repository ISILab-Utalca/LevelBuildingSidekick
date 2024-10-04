using ISILab.LBS.Behaviours;
using ISILab.LBS;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("PathOSBehaviour", typeof(PathOSBehaviour))]
    public class PathOSBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        public override void SetInfo(object target)
        {
            throw new System.NotImplementedException();
        }

        protected override VisualElement CreateVisualElement()
        {
            throw new System.NotImplementedException();
        }

        public void SetTools(ToolKit toolkit)
        {
            throw new System.NotImplementedException();
        }
    }
}
