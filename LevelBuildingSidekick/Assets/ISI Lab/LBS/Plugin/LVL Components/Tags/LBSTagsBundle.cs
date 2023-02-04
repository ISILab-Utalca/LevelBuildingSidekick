using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New tags bundle", menuName = "ISILab/Tags Bundle")]
public class LBSTagsBundle : ScriptableObject
{
    [SerializeField]
    [ScriptableToString(typeof(LBSTag))]
    List<string> tags = new List<string>();

    public List<LBSTag> Tags => tags.Select(t => Utility.DirectoryTools.GetScriptable<LBSTag>(t)).ToList();

    public void AddTag(LBSTag tag)
    {
        tags.Add(tag.value);
    }
}
