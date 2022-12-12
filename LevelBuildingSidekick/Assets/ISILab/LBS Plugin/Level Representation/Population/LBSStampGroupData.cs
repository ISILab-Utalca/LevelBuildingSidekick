using LBS;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSStampGroupData : LBSRepesentationData
{
    [SerializeField, JsonRequired, SerializeReference]
    private List<StampData> stamps = new List<StampData>();

    public StampData GetStamp(int index)
    {
        return stamps[index];
    }

    public StampData GetStamp(string label)
    {
        return stamps.Find(s => s.Label == label);
    }

    public void RemoveStamp(string label)
    {
        var r = stamps.Find(s => s.Label == label);
        if (r != null)
            RemoveStamp(r);
    }


    public void RemoveStamp(StampData stamp)
    {
        stamps.Remove(stamp);
    }

    /// <summary>
    /// Returns a list with the stamps saved, this list is a
    /// copy so you can not use this method to add or remove stamps.
    /// </summary>
    /// <returns></returns>
    public List<StampData> GetStamps()
    {
        return new List<StampData>(stamps);
    }

    public void AddStamp(StampData stamp)
    {
        stamps.Add(stamp);
    }

    public override void Clear()
    {
        stamps.Clear();
    }

    public override void Print()
    {
        var msg = "";
        msg += "<b>Stamp group. (step 2)</b>" + "\n";
        msg += "stamp amount: " + this.stamps.Count + "\n";
        msg += "------------";
        stamps.ForEach(s => msg += s.Label + ": " + s.Position + "\n");
        Debug.Log(msg);
    }

    internal void MoveStamp(Vector2Int x)
    {
        for (int i = 0; i < stamps.Count; i++)
        {
            stamps[i].Position += x;
        }
    }
}
