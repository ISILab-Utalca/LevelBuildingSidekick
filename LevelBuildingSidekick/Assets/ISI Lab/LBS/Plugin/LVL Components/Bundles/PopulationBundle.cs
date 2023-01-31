using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/PopulationBundle")]
[System.Serializable]
public class PopulationBundle : ScriptableObject, IBundle
{
    [SerializeField, JsonRequired]
    public List<LBSTag> tags = new List<LBSTag>();
    [SerializeReference, JsonRequired] 
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField, JsonRequired]
    public Texture2D icon;
    [SerializeField, JsonRequired]
    private string label = ""; // "ID" or "name"

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

    public List<GameObject> GetObjects(List<string> tags = null)
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
