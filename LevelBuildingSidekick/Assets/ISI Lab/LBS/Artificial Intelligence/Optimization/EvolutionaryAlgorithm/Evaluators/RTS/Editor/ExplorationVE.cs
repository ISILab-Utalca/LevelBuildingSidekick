using ISILab.AI.Categorization;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Exploration", typeof(Exploration))]
    public class ExplorationVE : LBSCustomEditor
    {
        DynamicFoldout colliderCharacteristic;

        public ExplorationVE(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {

            var eval = target as Exploration;
            this.target = eval;

            if (eval == null)
                return;

            if (eval.colliderCharacteristic != null)
            {
                colliderCharacteristic.Data = eval.colliderCharacteristic;
            }

            colliderCharacteristic.OnChoiceSelection += () => { eval.colliderCharacteristic = colliderCharacteristic.Data as LBSCharacteristic; };
        }

        protected override VisualElement CreateVisualElement()
        {

            colliderCharacteristic = new DynamicFoldout(typeof(LBSCharacteristic));
            colliderCharacteristic.Label = "Collider Characteristic";

            this.Add(colliderCharacteristic);


            return this;
        }
    }
}