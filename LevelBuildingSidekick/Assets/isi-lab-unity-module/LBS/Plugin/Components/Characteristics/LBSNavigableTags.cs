using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISI_Lab.Commons.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS.Characteristics
{
    [System.Serializable]
    [LBSCharacteristic("Navigable Tags", "")]
    public class LBSNavigableTags : LBSCharacteristic
    {
        [SerializeField]
        private List<LBSTag> tags = new List<LBSTag>();

        [SerializeField]
        private List<bool> navigable = new List<bool>();

        private Dictionary<LBSTag, bool> navigableTags = new Dictionary<LBSTag, bool>();

        public List<LBSTag> Tags => new List<LBSTag>(tags);
        public List<bool> Navigable => navigable;
        public Dictionary<LBSTag, bool> NavigableTagsRef => navigableTags;

        public void SetTags()
        {
            List<LBSTag> identifierTags = LBSAssetsStorage.Instance.Get<LBSTag>();
            List<LBSDirection> connections = Owner.GetChildrenCharacteristics<LBSDirection>();
            List<string> tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
            List<LBSTag> idents = tags.Select(s => identifierTags.Find(i => s == i.Label)).ToList().RemoveEmpties();

            bool tagsChanged = !this.tags.SequenceEqual(idents);

            this.tags = new List<LBSTag>(idents);
            if (tagsChanged)
            {
                navigable.Clear();
                for (int i = 0; i < idents.Count; i++)
                    navigable.Add(false);
            }
            navigableTags.Clear();

            for(int i = 0; i < idents.Count; i++)
            {
                LBSTag tag = idents[i];
                if (tag == null) continue;
                navigableTags.Add(tag, navigable[i]);
                //if(!navigableTags.ContainsKey(tag))
                //{
                //    navigableTags.Add(tag, false);
                //};
            }
        }

        public List<string> GetNavigableTags()
        {
            return navigableTags.Keys.Where(t => navigableTags[t]).Select(t => t.Label).ToList();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override List<string> Validate()
        {
            return new List<string>();
        }
    }
}
