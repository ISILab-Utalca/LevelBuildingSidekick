using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/Bundle")]
[System.Serializable]
public class Bundle : ScriptableObject, IBundle
{
    public List<LBSTag> tags = new List<LBSTag>();
    public List<GameObject> objects = new List<GameObject>();

    public void Add(List<IBundle> data)
    {
        foreach (IBundle bundle in data)
        {
            tags.AddRange(bundle.GetTags());
            objects.AddRange(bundle.GetObjects());
        }
    }

    public GameObject GetObject(int index)
    {
        return objects[index];
    }

    public List<GameObject> GetObjects()
    {
        return objects;
    }

    public LBSTag GetTag(int index)
    {
        return tags[index];
    }

    public List<LBSTag> GetTags()
    {
        return tags;
    }

    public void Remove(List<IBundle> data)
    {
        foreach (IBundle bundle in data)
        {
            tags.RemoveAll(bundle.GetTags().Contains);
            objects.RemoveAll(bundle.GetObjects().Contains);
        }
    }
}
