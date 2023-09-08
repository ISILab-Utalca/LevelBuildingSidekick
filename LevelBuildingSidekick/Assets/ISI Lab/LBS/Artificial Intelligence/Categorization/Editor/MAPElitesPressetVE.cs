using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("MAPElitesPresset", typeof(MAPElitesPresset))]
public class MAPElitesPressetVE : LBSCustomEditor
{
    TextField name;
    Vector2Field samples;

    ClassDropDown evaluatorX;
    Vector2Field thresholdX;
    VisualElement contentX;

    ClassDropDown evaluatorY;
    Vector2Field thresholdY;
    VisualElement contentY;

    ClassDropDown optimizer;
    VisualElement contentO;

    public MAPElitesPressetVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        InitialValues();
    }

    public override void SetInfo(object target)
    {
        throw new System.NotImplementedException();
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var vt = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MAPElitesPresset");
        vt.CloneTree(ve);

        var presset = target as MAPElitesPresset;

        name = ve.Q<TextField>(name: "Name");
        name.RegisterValueChangedCallback(
            evt => 
            {
                presset.name = evt.newValue;
            });

        samples = ve.Q<Vector2Field>(name: "Samples");
        samples.RegisterValueChangedCallback(
             evt =>
             {
                 presset.xSampleCount = (int)evt.newValue.x;
                 presset.ySampleCount = (int)evt.newValue.y;
             });

        evaluatorX = ve.Q<ClassDropDown>(name: "XDropdown");
        thresholdX = ve.Q<Vector2Field>(name: "XThreshold");
        thresholdX.RegisterValueChangedCallback(
             evt =>
             {
                 presset.xThreshold = evt.newValue;
             });
        contentX = ve.Q<VisualElement>(name: "XContent");

        evaluatorY = ve.Q<ClassDropDown>(name: "YDropdown");
        thresholdY = ve.Q<Vector2Field>(name: "YThreshold");
        thresholdY.RegisterValueChangedCallback(
             evt =>
             {
                 presset.yThreshold = evt.newValue;
             });
        contentY = ve.Q<VisualElement>(name: "YContent");

        optimizer = ve.Q<ClassDropDown>(name: "ODropdown");
        contentO = ve.Q<VisualElement>(name: "OContent");

        evaluatorX.Type = typeof(IRangedEvaluator);
        evaluatorY.Type = typeof(IRangedEvaluator);
        optimizer.Type = typeof(BaseOptimizer);

        evaluatorX.RegisterValueChangedCallback(
            evt => 
            {
                var obj = evaluatorX.GetChoiceInstance();
                presset.xEvaluator = obj as IRangedEvaluator;
                LoadEditor(contentX, obj);
            });

        evaluatorY.RegisterValueChangedCallback(
            evt =>
            {
                var obj = evaluatorY.GetChoiceInstance();
                presset.yEvaluator = obj as IRangedEvaluator;
                LoadEditor(contentY, obj);
            });

        optimizer.RegisterValueChangedCallback(
            evt =>
            {
                var obj = optimizer.GetChoiceInstance();
                presset.optimizer = obj as BaseOptimizer;
                LoadEditor(contentO, obj);
            });


        InitialValues();


        return ve;
    }

    private void InitialValues()
    {
        var presset = target as MAPElitesPresset;
        name.value = presset.name;
        samples.value = new Vector2(presset.xSampleCount, presset.ySampleCount);

        if (presset.xEvaluator != null)
        {
            evaluatorX.value = presset.xEvaluator.GetType().Name;
            LoadEditor(contentX, presset.xEvaluator);
        }
        thresholdX.value = presset.xThreshold;


        if (presset.yEvaluator != null)
        {
            evaluatorY.value = presset.yEvaluator.GetType().Name;
            LoadEditor(contentY, presset.yEvaluator);
        }
        thresholdY.value = presset.yThreshold;

        if (presset.optimizer != null)
        {
            optimizer.value = presset.optimizer.GetType().Name;
            LoadEditor(contentO, presset.optimizer);
        }

    }


    private void LoadEditor(VisualElement container, object target)
    {
        container.Clear();

        var prosp = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>();

        if (prosp.Count <= 0)
        {
            return;
        }

        var ves = prosp.Where(t => t.Item2.Any(v => v.type == target.GetType()));

        if(ves.Count() <= 0)
        {
            return;
        }

        var ve = Activator.CreateInstance(ves.First().Item1, new object [] {target}) as VisualElement;

        container.Add(ve);
    }

}
