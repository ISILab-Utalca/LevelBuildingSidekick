using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class LBSNullEditor : LBSCustomEditor
    {
        public LBSNullEditor()
        {
        }

        public override void SetInfo(object paramTarget)
        {
            var target = paramTarget as LBSCharacteristic;

            var label = new Label("la caracteristica " + target + " no se puede visualizar.");
            label.style.flexWrap = Wrap.Wrap;
            label.style.whiteSpace = WhiteSpace.Normal;
            Add(label);
        }

        protected override VisualElement CreateVisualElement()
        {
            throw new System.NotImplementedException();
        }
    }
}