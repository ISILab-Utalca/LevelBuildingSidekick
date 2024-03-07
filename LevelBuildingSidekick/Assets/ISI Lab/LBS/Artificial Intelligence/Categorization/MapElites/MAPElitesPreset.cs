using ISILab.AI.Optimization;
using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using ISILab.LBS.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ISILab.LBS.AI.Categorization
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ISILab/LBS/MapElitePreset")]
    public class MAPElitesPreset : ScriptableObject, ICloneable
    {
        [SerializeField]
        MapElites mapElites = new MapElites();

        [SerializeField]
        string maskType = "";

        public List<LBSTag> blackList = new List<LBSTag>();

        [JsonIgnore]
        public MapElites MapElites => mapElites?.Clone() as MapElites;

        [JsonIgnore]
        public Type MaskType
        {
            get => Type.GetType(maskType);
            set => maskType = value.FullName;
        }



        public Vector2Int SampleCount
        {
            get => new Vector2Int(mapElites.XSampleCount, mapElites.YSampleCount);
            set
            {
                mapElites.XSampleCount = value.x;
                mapElites.YSampleCount = value.y;
            }
        }

        public double Devest
        {
            get => mapElites.devest;
            set => mapElites.devest = value;
        }

        public IRangedEvaluator XEvaluator
        {
            get => mapElites?.XEvaluator;
            set => mapElites.XEvaluator = value;
        }

        public Vector2 XThreshold
        {
            get => mapElites.XThreshold;
            set => mapElites.XThreshold = value;
        }

        public IRangedEvaluator YEvaluator
        {
            get => mapElites?.YEvaluator;
            set => mapElites.YEvaluator = value;
        }

        [SerializeField]
        public Vector2 YThreshold
        {
            get => mapElites.YThreshold;
            set => mapElites.YThreshold = value;
        }

        public BaseOptimizer Optimizer
        {
            get => mapElites?.Optimizer;
            set => mapElites.Optimizer = value;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}