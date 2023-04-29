using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewIDBundle", menuName = "ISILab/Identifiers Bundle")]
public class LBSIdentifierBundle : ScriptableObject
{
    [SerializeField]
    [ScriptableToString(typeof(LBSIdentifier))]
    protected List<string> tags = new List<string>();

    public List<LBSIdentifier> Tags
    {
        get
        {
            var tt = Utility.DirectoryTools.GetScriptables<LBSIdentifier>().ToList();
            List<LBSIdentifier> toR = new List<LBSIdentifier>();
            foreach (var tag in tags)
            {
                var x = tt.Find(v => v.Label == tag);
                if (x != null)
                    toR.Add(x);
            }
            return toR;
        }
    }

    public void RemoveAt(int index)
    {
        tags.RemoveAt(index);
    }

    public void Remove (LBSIdentifier tag)
    {
        tags.Remove(tag.Label);
    }

    public void AddTag(LBSIdentifier tag)
    {
        tags.Add(tag.Label);
    }

    public LBSIdentifier GetTag(int index)
    {
        var current = tags[index];

        var ttags = Utility.DirectoryTools.GetScriptables<LBSIdentifier>();  // esto lagea la interfaz de GlobalTag (!!!)
        var toR = ttags.Find(t => t.Label == current);

        return toR;
        //return (t == null)? null : Utility.DirectoryTools.GetScriptable<LBSIdentifier>(t);
    }

    public List<LBSIdentifier> GetTags()
    {
        return Tags;
    }

    public void Add(List<Bundle_Old> data)
    {
        var tags = data.Select(b => b.ID);

        foreach (var t in tags)
        {
            if(!this.tags.Contains(t.Label))
                this.tags.Add(t.Label);
        }
    }

    public virtual void Remove(List<Bundle_Old> data)
    {
        var tags = data.Select(b => b.ID);

        foreach(var t in tags)
        {
            this.tags.Remove(t.Label);
        }    
    }
}
