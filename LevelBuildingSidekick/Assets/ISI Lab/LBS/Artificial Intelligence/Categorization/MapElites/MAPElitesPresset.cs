using ISILab.AI.Optimization;
using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MAPElitesPresset : ScriptableObject
{
    public int xSampleCount = 4;

    public int ySampleCount = 4;

    [Range(0, 0.5f), HideInInspector]
    public double devest = 0.5;

    [SerializeField, SerializeReference]
    public IRangedEvaluator xEvaluator;

    [SerializeField]
    public Vector2 xThreshold = new Vector2(0.2f, 0.8f);

    [SerializeField, SerializeReference]
    public IRangedEvaluator yEvaluator;

    [SerializeField]
    public Vector2 yThreshold = new Vector2(0.2f, 0.8f);

    [SerializeField, SerializeReference]
    public BaseOptimizer optimizer = new GeneticAlgorithm();
}
