using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewIDBundle", menuName = "ISILab/Identifiers Bundle")]
public class LBSIdentifierBundle : ScriptableObject
{
    [SerializeField]
    private List<LBSIdentifier> tags = new List<LBSIdentifier>();

    public List<LBSIdentifier> Tags
    {
        get => new List<LBSIdentifier>(tags);
    }

    public void RemoveAt(int index)
    {
        tags.RemoveAt(index);
    }

    public void Remove (LBSIdentifier tag)
    {
        tags.Remove(tag);
    }

    public void Add(LBSIdentifier tag)
    {
        tags.Add(tag);
    }


    public void Add(List<Bundle> data) // (?) por que existe este metodo?
    {
        var tags = data.Select(b => b.ID);

        foreach (var tag in tags)
        {
            if(!this.tags.Contains(tag))
                this.tags.Add(tag);
        }
    }

    public virtual void Remove(List<Bundle> data) // (?) por que existe este metodo?
    {
        var tags = data.Select(b => b.ID);

        foreach(var tag in tags)
        {
            this.tags.Remove(tag);
        }    
    }
}
