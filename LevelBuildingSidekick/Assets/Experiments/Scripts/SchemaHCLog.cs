using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class SchemaHCLog : ScriptableObject
{
    public List<HCLog> log = new List<HCLog>();

    public void ExportCSV()
    {
        var t1 = new Tuple<string, List<string>>("Neighbor Time", new List<string>());
        var t2 = new Tuple<string, List<string>>("Neighbor Count", new List<string>());
        var t3 = new Tuple<string, List<string>>("Evaluation Time", new List<string>());
        var t4 = new Tuple<string, List<string>>("Time", new List<string>());
        var t5 = new Tuple<string, List<string>>("Best Fitness", new List<string>());

        foreach(var l in log)
        {
            t1.Item2.Add(l.neighborTime.ToString());
            t2.Item2.Add(l.neighborCount.ToString());
            t3.Item2.Add(l.evaluationTime.ToString());
            t4.Item2.Add(l.time.ToString());
            t5.Item2.Add(l.bestFitness.ToString());
        }

        ExcelExporter.ExportToExcel(new List<Tuple<string, List<string>>>() { t1,t2,t3,t4,t5}, name);
    }
}

[System.Serializable]
public struct HCLog
{
    public double neighborTime;
    public double neighborCount;
    public double evaluationTime;
    public double time;
    public double bestFitness;
}
