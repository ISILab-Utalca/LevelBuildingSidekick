using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("ExhaustiveMoveGene", typeof(ExhaustiveMoveGene))]
public class ExhaustiveMoveGeneVE : LBSCustomEditor
{
    ListView blackList;
    IntegerField range;

    public ExhaustiveMoveGeneVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var mut = target as ExhaustiveMoveGene;

        range.value = mut.Range;
        range.RegisterValueChangedCallback(e => { mut.Range = e.newValue; });

        blackList.itemsSource = mut.blackList;
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var mut = target as ExhaustiveMoveGene;

        range = new IntegerField();
        range.label = "Movement Range";

        blackList = new ListView();
        blackList.showAddRemoveFooter = true;
        blackList.showBorder = true;
        blackList.showFoldoutHeader = true;
        blackList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

        blackList.makeItem = MakeItem;
        blackList.bindItem = BindItem;
        //resourceChracteristics.destroyItem = DestroyItem;

        ve.Add(blackList);

        return ve;
    }

    VisualElement MakeItem()
    {
        var v = new ObjectField();
        v.objectType = typeof(Object);
        return v;
    }

    void BindItem(VisualElement ve, int index)
    {
        var mut = target as ExhaustiveMoveGene;
        if (index < mut.blackList.Count)
        {
            var of = ve.Q<ObjectField>();
            //Debug.Log("Bind");
            if (mut.blackList[index] != null)
            {
                //Debug.Log(eval.resourceCharactersitic[index]);
                of.value = mut.blackList[index] as Object;
            }
            of.RegisterValueChangedCallback(e => { mut.blackList[index] = e.newValue; });
            of.label = "Element " + index + ":";
        }
    }
}
