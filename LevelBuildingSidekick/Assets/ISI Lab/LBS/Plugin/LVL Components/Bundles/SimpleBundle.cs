using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/Bundle")]
[System.Serializable]
public class SimpleBundle : Bundle
{
    public List<GameObject> objects = new List<GameObject>();

    public override void Add(List<Bundle> data)
    {
        foreach (Bundle bundle in data)
        {
            tags.AddRange(bundle.Tags.Select(t => t.value));
            objects.AddRange(bundle.GetObjects());
        }
    }

    public override GameObject GetObject(int index)
    {
        return objects[index];
    }

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        if(tags == null)
            return objects;
        foreach(var t in tags)
        {
            if (!this.tags.Contains(t))
                return new List<GameObject>();
        }
        return objects;
    }

    public override LBSTag GetTag(int index)
    {
        return Utility.DirectoryTools.GetScriptable<LBSTag>(tags[index]);
    }

    public override List<LBSTag> GetTags()
    {
        return Tags;
    }

    public override void Remove(List<Bundle> data)
    {
        foreach (Bundle bundle in data)
        {
            tags.RemoveAll(t => bundle.Tags.Any(tag => tag.value == t));
            objects.RemoveAll(o => bundle.GetObjects().Contains(o));
        }
    }
}
