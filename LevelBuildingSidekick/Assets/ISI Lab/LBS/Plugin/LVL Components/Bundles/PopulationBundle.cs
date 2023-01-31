using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/PopulationBundle")]
[System.Serializable]
public class PopulationBundle : Bundle
{
    [SerializeField, JsonRequired]
    public List<LBSTag> tags = new List<LBSTag>();
    [SerializeReference, JsonRequired] 
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField, JsonRequired]
    public Texture2D icon;
    [SerializeField, JsonRequired]
    public string label = ""; // "ID" or "name"

    public override void Add(List<Bundle> data)
    {
        foreach (Bundle bundle in data)
        {
            tags.AddRange(bundle.GetTags());
            objects.AddRange(bundle.GetObjects());
        }
    }

    public override GameObject GetObject(int index)
    {
        return objects[index];
    }

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        return objects;
    }

    public override LBSTag GetTag(int index)
    {
        return tags[index];
    }

    public override List<LBSTag> GetTags()
    {
        return tags;
    }

    public override void Remove(List<Bundle> data)
    {
        foreach (Bundle bundle in data)
        {
            tags.RemoveAll(bundle.GetTags().Contains);
            objects.RemoveAll(bundle.GetObjects().Contains);
        }
    }
}
