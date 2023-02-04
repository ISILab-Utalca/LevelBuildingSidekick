using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Bundle : ScriptableObject
{
    [SerializeField]
    [ScriptableToString(typeof(LBSTag))]
    protected List<string> tags = new List<string>();

    public List<LBSTag> Tags => tags.Select(t => Utility.DirectoryTools.GetScriptable<LBSTag>(t)).ToList();

    public abstract void Add(List<Bundle> data);
    public abstract GameObject GetObject(int index);
    public abstract List<GameObject> GetObjects(List<string> tags = null);
    public abstract LBSTag GetTag(int index);
    public abstract List<LBSTag> GetTags();
    public abstract void Remove(List<Bundle> data);
}
