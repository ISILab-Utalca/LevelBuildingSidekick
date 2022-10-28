using Commons.Optimization.Evaluator;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThresholdEvaluator : IRangedEvaluator
{
    float max = 1;
    public float MaxValue => max;

    float min = 0;
    public float MinValue => min;

    [Range(0,1)]
    public float Threshold = 0.5f;
    IRangedEvaluator evaluator;

    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        var tfield = new FloatField("Desired Fitness");
        tfield.value = Threshold;
        tfield.RegisterValueChangedCallback(e => {
            Threshold = e.newValue;
        });

        SubPanel FitnessPanel = new SubPanel();
        FitnessPanel.style.display = DisplayStyle.None;

        var fitnessDD = new DropdownField("Fitness");
        var fitClass = new ClassDropDown(fitnessDD, typeof(IRangedEvaluator), true);
        fitClass.Dropdown.RegisterCallback<ChangeEvent<string>>(e => {
            var value = fitClass.GetChoiceInstance();
            evaluator = value as IRangedEvaluator;
            if (value is IShowable)
                fitClass.Dropdown.value = e.newValue;
            if (evaluator != null)
            {
                FitnessPanel.style.display = DisplayStyle.Flex;
                FitnessPanel.SetValue(evaluator, evaluator.GetType().ToString());
            }
        });

        content.Add(v2);
        content.Add(tfield);
        content.Add(fitnessDD);
        content.Add(FitnessPanel);
        //content.Add(evaluator.CIGUI());

        return content;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        float fitness = evaluator.Evaluate(evaluable);
        var desired = evaluator.MinValue + (evaluator.MaxValue - evaluator.MinValue) * Threshold;

        //Debug.Log("F: " + fitness + " - D: " + desired);
        //Debug.Log(1 - (Mathf.Abs(desired - fitness) / (evaluator.MaxValue - evaluator.MinValue)));
        return 1 - (Mathf.Abs(desired - fitness) / (evaluator.MaxValue - evaluator.MinValue));
    }

    public string GetName()
    {
        return "Threshold Evaluator";
    }
}
