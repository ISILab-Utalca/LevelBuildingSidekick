using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/Bundle")]
[System.Serializable]
public class SimpleBundle : Bundle_Old
{
    private List<GameObject> assets = new List<GameObject>();

    public List<GameObject> Assets
    {
        get => new List<GameObject>(assets);
    }

    public void Add(GameObject gameObject)
    {
        assets.Add(gameObject);
    }

    public override void Add(List<Bundle_Old> data)
    {
        foreach (Bundle_Old bundle in data)
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
                if (!assets.Contains(o))
                {
                    assets.Add(o);
                }
            }
        }
    }

    public override GameObject GetObject(int index)
    {
        return assets[index];
    }

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        if(tags == null)
            return assets;
        foreach(var t in tags)
        {
            if (!this.characteristics.Any(c => c.Label == t))
                return new List<GameObject>();
        }
        return assets;
    }

    public override LBSCharacteristic GetTag(int index)
    {
        return characteristics[index];
    }

    public override List<LBSCharacteristic> GetCharacteristics()
    {
        return characteristics;
    }

    public override void Remove(List<Bundle_Old> data)
    {
        foreach (Bundle_Old bundle in data)
        {
            characteristics.RemoveAll(t => bundle.GetCharacteristics().Any(c => c == t));
            assets.RemoveAll(o => bundle.GetObjects().Contains(o));
        }
    }
}
