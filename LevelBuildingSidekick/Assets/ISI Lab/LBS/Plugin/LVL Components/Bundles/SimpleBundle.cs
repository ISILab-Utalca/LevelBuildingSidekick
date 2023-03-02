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
            var characts = bundle.GetCharacteristics();
            foreach (LBSCharacteristic c in characts)
            {
                if(!characteristics.Contains(c))
                {
                    characteristics.Add(c);
                }
            }

            var objs = bundle.GetObjects();
            foreach (GameObject o in objs)
            {
                if (!objects.Contains(o))
                {
                    objects.Add(o);
                }
            }
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
            if (!this.characteristics.Any(c => c.Label == t))
                return new List<GameObject>();
        }
        return objects;
    }

    public override LBSCharacteristic GetTag(int index)
    {
        return characteristics[index];
    }

    public override List<LBSCharacteristic> GetCharacteristics()
    {
        return characteristics;
    }

    public override void Remove(List<Bundle> data)
    {
        foreach (Bundle bundle in data)
        {
            characteristics.RemoveAll(t => bundle.GetCharacteristics().Any(c => c == t));
            objects.RemoveAll(o => bundle.GetObjects().Contains(o));
        }
    }
}
