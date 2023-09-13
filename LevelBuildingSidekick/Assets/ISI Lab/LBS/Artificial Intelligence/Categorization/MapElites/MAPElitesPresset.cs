using ISILab.AI.Optimization;
using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(menuName = "ISILab/LBS/MapElitePresset")]
public class MAPElitesPresset : ScriptableObject, ICloneable
{
    [SerializeField]
    MapElites mapElites = new MapElites();

    public MapElites MapElites => mapElites?.Clone() as MapElites;

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

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }
}
