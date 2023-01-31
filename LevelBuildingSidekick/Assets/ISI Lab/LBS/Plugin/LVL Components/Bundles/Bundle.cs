using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bundle : ScriptableObject
{
    [SerializeField]
    protected List<LBSTag> tags = new List<LBSTag>();

    public abstract void Add(List<Bundle> data);
    public abstract GameObject GetObject(int index);
    public abstract List<GameObject> GetObjects(List<string> tags = null);
    public abstract LBSTag GetTag(int index);
    public abstract List<LBSTag> GetTags();
    public abstract void Remove(List<Bundle> data);
}
