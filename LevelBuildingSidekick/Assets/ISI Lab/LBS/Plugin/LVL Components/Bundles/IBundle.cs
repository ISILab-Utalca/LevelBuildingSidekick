using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBundle
{
    public List<LBSTag> GetTags();
    public List<GameObject> GetObjects(List<string> tags = null);
    public LBSTag GetTag(int index);
    public GameObject GetObject(int index);

    public void Add(List<IBundle> data);
    public void Remove(List<IBundle> data);
}
