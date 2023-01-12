using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Composite bundle", menuName = "ISILab/Composite bundle")]
[System.Serializable]
public class CompositeBundle : ScriptableObject, IBundle
{
    public List<LBSTag> tags = new List<LBSTag>();
    public List<IBundle> bundles = new List<IBundle>();

    public void Add(List<IBundle> data)
    {
        bundles.AddRange(data);
    }

    public GameObject GetObject(int index)
    {
        int currentIndex = 0;
        foreach (var bundle in bundles)
        {
            List<GameObject> prefabs = bundle.GetObjects();
            if (currentIndex + prefabs.Count > index)
            {
                return prefabs[index - currentIndex];
            }
            currentIndex += prefabs.Count;
        }
        return null;
    }

    public List<GameObject> GetObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (IBundle bundle in bundles)
        {
            objects.AddRange(bundle.GetObjects());
        }
        return objects;
    }

    public LBSTag GetTag(int index)
    {
        int currentIndex = 0;
        foreach (IBundle bundle in bundles)
        {
            List<LBSTag> currentTags = bundle.GetTags();
            if (currentIndex + currentTags.Count > index)
            {
                return currentTags[index - currentIndex];
            }
            currentIndex += currentTags.Count;
        }
        return null;
    }

    public List<LBSTag> GetTags()
    {
        List<LBSTag> allTags = new List<LBSTag>();
        foreach (IBundle bundle in bundles)
        {
            allTags.AddRange(bundle.GetTags());
        }
        return allTags;
    }

    public void Remove(List<IBundle> data)
    {
        throw new System.NotImplementedException();
    }
}
