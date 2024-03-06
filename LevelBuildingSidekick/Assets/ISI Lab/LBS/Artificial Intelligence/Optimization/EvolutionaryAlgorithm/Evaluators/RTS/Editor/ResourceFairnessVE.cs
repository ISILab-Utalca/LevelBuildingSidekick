using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.AI.Categorization;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("ResourcesSafetyFairness", typeof(ResourcesSafetyFairness))]
    public class ResourceFairnessVE : LBSCustomEditor
    {
        DynamicFoldout playerCharacteristic;
        ListView resourceChracteristics;

        public ResourceFairnessVE(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            this.target = target;
            var eval = target as ResourcesSafetyFairness;
            if (eval.playerCharacteristc != null)
            {
                playerCharacteristic.SetInfo(eval.playerCharacteristc);
            }

        }

        protected override VisualElement CreateVisualElement()
        {
            var eval = target as ResourcesSafetyFairness;

            playerCharacteristic = new DynamicFoldout(typeof(LBSCharacteristic));
            playerCharacteristic.Label = "Player Characteristic";

            if (eval != null && eval.playerCharacteristc != null)
            {
                playerCharacteristic.Data = eval.playerCharacteristc;
            }

            playerCharacteristic.OnChoiceSelection += () => { eval.playerCharacteristc = playerCharacteristic.Data as LBSCharacteristic; };


            resourceChracteristics = new ListView();
            resourceChracteristics.showAddRemoveFooter = true;
            resourceChracteristics.showBorder = true;
            resourceChracteristics.headerTitle = "Resource Characteristics";
            resourceChracteristics.showFoldoutHeader = true;
            resourceChracteristics.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            resourceChracteristics.makeItem = MakeItem;
            resourceChracteristics.bindItem = BindItem;
            //resourceChracteristics.destroyItem = DestroyItem;
            resourceChracteristics.itemsSource = eval.resourceCharactersitic;

            this.Add(playerCharacteristic);
            this.Add(resourceChracteristics);

            return this;
        }

        VisualElement MakeItem()
        {
            var v = new DynamicFoldout(typeof(LBSCharacteristic));
            v.Label = "Resource Characteristic";
            return v;
        }

        void BindItem(VisualElement ve, int index)
        {
            var eval = target as ResourcesSafetyFairness;
            if (index < eval.resourceCharactersitic.Count)
            {
                var cf = ve.Q<DynamicFoldout>();
                cf.Label = "Resource Characteristic " + index + ":";
                //Debug.Log("Bind");
                if (eval.resourceCharactersitic[index] != null)
                {
                    cf.Data = eval.resourceCharactersitic[index];
                    //Debug.Log(eval.resourceCharactersitic[index]);
                }
                cf.OnChoiceSelection = () => {  eval.resourceCharactersitic[index] = cf.Data as LBSCharacteristic; };
                //Debug.Log(eval.resourceCharactersitic[index] == cf.Data);
            }
        }
        /*
        void DestroyItem(VisualElement ve)
        {
            //var cf = ve as ClassFoldout;
            var eval = target as ResourcesSafetyFairness;
            var index = resourceChracteristics.IndexOf(ve);
            eval.resourceCharactersitic.RemoveAt(index);
        }*/

    }
}