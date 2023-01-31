using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Composite bundle", menuName = "ISILab/Composite bundle")]
[System.Serializable]
public class CompositeBundle : Bundle
{
    [SerializeReference]
    public List<Bundle> bundles = new List<Bundle>();

    public override void Add(List<Bundle> data)
    {
        bundles.AddRange(data);
    }

    public override GameObject GetObject(int index)
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

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        var ts = this.tags.Select(t => t.value);
        if(tags != null)
        {
            for(int i = 0; i < tags.Count; i++)
            {
                if(ts.Contains(tags[i]))
                {
                    tags.RemoveAt(i);
                }
            }
        }

        List<GameObject> objects = new List<GameObject>();
        foreach (Bundle bundle in bundles)
        {
            objects.AddRange(bundle.GetObjects(tags));
        }
        return objects;
    }

    public override LBSTag GetTag(int index)
    {
        int currentIndex = 0;
        foreach (Bundle bundle in bundles)
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

    public override List<LBSTag> GetTags()
    {
        List<LBSTag> allTags = new List<LBSTag>();
        foreach (Bundle bundle in bundles)
        {
            allTags.AddRange(bundle.GetTags());
        }
        return allTags;
    }

    public override void Remove(List<Bundle> data)
    {
        throw new System.NotImplementedException();
    }
}
