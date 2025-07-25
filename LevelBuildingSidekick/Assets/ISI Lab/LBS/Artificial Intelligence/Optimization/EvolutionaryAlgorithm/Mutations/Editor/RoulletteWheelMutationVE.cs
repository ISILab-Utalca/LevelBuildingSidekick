using GeneticSharp.Domain.Mutations;
using ISILab.AI.Categorization;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("RoulleteWheelMutation", typeof(RoulleteWheelMutation))]
    public class RoulletteWheelMutationVE : LBSCustomEditor
    {

        ListView mutations;

        public RoulletteWheelMutationVE(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            this.target = paramTarget;
            var mut = paramTarget as RoulleteWheelMutation;
            mutations.itemsSource = mut.mutations;
        }

        protected override VisualElement CreateVisualElement()
        {
            var ve = new VisualElement();
            var mut = target as RoulleteWheelMutation;

            mutations = new ListView();
            mutations.showAddRemoveFooter = true;
            mutations.showBorder = true;
            mutations.headerTitle = "Mutations";
            mutations.showFoldoutHeader = true;
            mutations.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;


            mutations.makeItem = MakeItem;
            mutations.bindItem = BindItem;

            ve.Add(mutations);

            return ve;
        }

        VisualElement MakeItem()
        {
            var v = new VisualElement();
            var f = new FloatField();
            f.label = "Weight";
            v.Add(f);
            var df = new DynamicFoldout(typeof(MutationBase));
            df.Label = "Mutation";
            v.Add(df);

            return v;
        }

        void BindItem(VisualElement ve, int index)
        {
            var eval = target as RoulleteWheelMutation;
            if (index < eval.mutations.Count)
            {
                var cf = ve.Q<DynamicFoldout>();
                var ff = ve.Q<FloatField>();
                if (eval.mutations[index] != null)
                {
                    cf.Data = eval.mutations[index].mutation;
                    cf.Label = "Mutation " + index + ":";
                    ff.value = eval.mutations[index].weight;
                }
                cf.OnChoiceSelection = () => { eval.mutations[index] = new WeightedMutation(cf.Data as MutationBase, ff.value); };
                ff.RegisterValueChangedCallback(e => { eval.mutations[index] = new WeightedMutation(cf.Data as MutationBase, e.newValue); });
            }
        }
    }
}