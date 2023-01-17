using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New tags bundle", menuName = "ISILab/Tags Bundle")]
public class LBSTagsBundle : ScriptableObject
{
    public List<LBSTag> tags = new List<LBSTag>();

    public void AddTag(LBSTag tag)
    {
        tags.Add(tag);
    }
}
