using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class SchemaHCLog : ScriptableObject
{
    public List<HCLog> log = new List<HCLog>();
}

[System.Serializable]
public struct HCLog
{
    public double neighborTime;
    public double neighborCount;
    public double evaluationTime;
    public double time;
    public double bestFitness;
    public LBSSchema result;
}
