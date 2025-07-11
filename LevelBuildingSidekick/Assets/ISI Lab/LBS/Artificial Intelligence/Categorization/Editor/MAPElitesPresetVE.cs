using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using System;
using ISILab.LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("MAPElitesPresset", typeof(MAPElitesPreset))]
    public class MAPElitesPresetVE : LBSCustomEditor
    {
        TextField presetName;
        Vector2IntField samples;

        ClassDropDown evaluatorX;
        Vector2Field thresholdX;
        VisualElement contentX;

        ClassDropDown evaluatorY;
        Vector2Field thresholdY;
        VisualElement contentY;

        ClassDropDown optimizer;
        VisualElement contentO;

        ClassDropDown maskType;

        ListView blacklist;

        public MAPElitesPresetVE(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            var presset = paramTarget as MAPElitesPreset;
            samples.value = presset.SampleCount;

            if (presset.PresetName != null)
            {
                presetName.value = presset.PresetName.GetType().Name;
                LoadEditor(contentX, presset.PresetName);
            }
            presetName.value = presset.PresetName;

            if (presset.XEvaluator != null)
            {
                evaluatorX.value = presset.XEvaluator.GetType().Name;
                LoadEditor(contentX, presset.XEvaluator);
            }
            thresholdX.value = presset.XThreshold;


            if (presset.YEvaluator != null)
            {
                evaluatorY.value = presset.YEvaluator.GetType().Name;
                LoadEditor(contentY, presset.YEvaluator);
            }
            thresholdY.value = presset.YThreshold;

            if (presset.Optimizer != null)
            {
                optimizer.value = presset.Optimizer.GetType().Name;
                LoadEditor(contentO, presset.Optimizer);
            }
            if (presset.MaskType != null)
                maskType.value = presset.MaskType.Name;
        }

        protected override VisualElement CreateVisualElement()
        {
            var vt = DirectoryTools.GetAssetByName<VisualTreeAsset>("MAPElitesPresset");
            vt.CloneTree(this);

            var presset = target as MAPElitesPreset;
            presetName = this.Q<TextField>(name: "PresetName");
            presetName.RegisterValueChangedCallback(
                evt => {
                    presset.PresetName = evt.newValue;
                    }
                );
            
            samples = this.Q<Vector2IntField>(name: "Samples");
            samples.RegisterValueChangedCallback(
                 evt =>
                 {
                     presset.SampleCount = evt.newValue;
                 });

            evaluatorX = this.Q<ClassDropDown>(name: "XDropdown");
            thresholdX = this.Q<Vector2Field>(name: "XThreshold");
            thresholdX.RegisterValueChangedCallback(
                 evt =>
                 {
                     presset.XThreshold = evt.newValue;
                 });
            contentX = this.Q<VisualElement>(name: "XContent");

            evaluatorY = this.Q<ClassDropDown>(name: "YDropdown");
            thresholdY = this.Q<Vector2Field>(name: "YThreshold");
            thresholdY.RegisterValueChangedCallback(
                 evt =>
                 {
                     presset.YThreshold = evt.newValue;
                 });
            contentY = this.Q<VisualElement>(name: "YContent");

            optimizer = this.Q<ClassDropDown>(name: "ODropdown");
            contentO = this.Q<VisualElement>(name: "OContent");

            evaluatorX.Type = typeof(IRangedEvaluator);
            evaluatorY.Type = typeof(IRangedEvaluator);
            optimizer.Type = typeof(BaseOptimizer);

            evaluatorX.RegisterValueChangedCallback(
                evt =>
                {
                    var obj = evaluatorX.GetChoiceInstance();
                    presset.XEvaluator = obj as IRangedEvaluator;
                    LoadEditor(contentX, obj);
                });

            evaluatorY.RegisterValueChangedCallback(
                evt =>
                {
                    var obj = evaluatorY.GetChoiceInstance();
                    presset.YEvaluator = obj as IRangedEvaluator;
                    LoadEditor(contentY, obj);
                });

            optimizer.RegisterValueChangedCallback(
                evt =>
                {
                    var obj = optimizer.GetChoiceInstance();
                    presset.Optimizer = obj as BaseOptimizer;
                    LoadEditor(contentO, obj);
                });

            maskType = this.Q<ClassDropDown>(name: "BehaviourContext");
            maskType.Type = typeof(LBSBehaviour);

            maskType.RegisterValueChangedCallback(
                evt =>
                {
                    UnityEngine.Debug.Log(maskType.TypeValue);
                    presset.MaskType = maskType.TypeValue;
                });

            blacklist = this.Q<ListView>(name: "BlackList");
            blacklist.itemsSource = presset.blackList;

            blacklist.makeItem = MakeItem;
            blacklist.bindItem = BindItem;

            return this;
        }

        private void LoadEditor(VisualElement container, object target)
        {
            container.Clear();

            var veType = LBS_Editor.GetEditor(target.GetType());

            if (veType == null)
            {
                return;
            }

            var ve = Activator.CreateInstance(veType, new object[] { target }) as VisualElement;
            if (ve is ClassFoldout cf)
            {
                //cf.OnCreate(veType, target);
            }

            container.Add(ve);
        }

        VisualElement MakeItem()
        {
            var v = new ObjectField();
            v.objectType = typeof(LBSTag);
            return v;
        }

        void BindItem(VisualElement element, int index)
        {
            var presset = target as MAPElitesPreset;
            if (index < presset.blackList.Count)
            {
                var of = element as ObjectField;
                if (presset.blackList[index] != null)
                {
                    of.value = presset.blackList[index];
                }
                of.RegisterValueChangedCallback(e => { presset.blackList[index] = e.newValue as LBSTag; });
                of.label = "Element " + index + ":";
            }
        }

    }
}