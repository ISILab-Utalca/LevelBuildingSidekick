using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

[LBSCustomEditor("ResourcesSafetyFairness", typeof(ResourcesSafetyFairness))]
public class ResourceFairnessVE : LBSCustomEditor
{
    DynamicFoldout playerCharacteristic;
    ListView resourceChracteristics;

    public ResourceFairnessVE(object target) : base(target)
    {
        Add(CreateVisualElement());
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
        var ve = new VisualElement();

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
        resourceChracteristics.showFoldoutHeader = true;
        resourceChracteristics.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

        resourceChracteristics.makeItem = MakeItem;
        resourceChracteristics.bindItem = BindItem;
        //resourceChracteristics.destroyItem = DestroyItem;
        resourceChracteristics.itemsSource = eval.resourceCharactersitic;

        ve.Add(playerCharacteristic);
        ve.Add(resourceChracteristics);

        return ve;
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
            //Debug.Log("Bind");
            if (eval.resourceCharactersitic[index] != null)
            {
                //Debug.Log(eval.resourceCharactersitic[index]);
                cf.Data = eval.resourceCharactersitic[index];
            }
            cf.OnChoiceSelection = () => { eval.resourceCharactersitic[index] = cf.Data as LBSCharacteristic; };
            cf.Label = "Resource Characteristic " + index + ":";
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
