using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Navigable Tags", typeof(LBSNavigableTags))]
    public class LBSNavigableTagsEditor : LBSCustomEditor
    {
        public VisualElement content;

        public LBSNavigableTagsEditor(object target) : base(target) 
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object paramTarget)
        {
            target = paramTarget;
            LBSNavigableTags navigableTags = target as LBSNavigableTags;

            if (navigableTags == null) return;

            navigableTags.SetTags();

            content = new VisualElement();
            Add(content);
            if(navigableTags.Tags.Count <= 0)
            {
                var wp = new WarningPanel("This bundle does not contain any child with Direction characteristics.");
                content.Add(wp);
                return;
            }

            List<LBSTag> tags = navigableTags.Tags;
            Dictionary<LBSTag, bool> navTags = navigableTags.NavigableTagsRef;
            for(int i = 0; i < tags.Count; i++)
            {
                var box = new VisualElement();
                content.Add(box);

                int index = i;
                LBSTag tag = tags[index];
                var toggle = new Toggle(tags[i].Label);
                toggle.SetValueWithoutNotify(navTags[tag]);
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if(evt.newValue != evt.previousValue)
                    {
                        navTags[tag] = evt.newValue;
                        navigableTags.Navigable[index] = evt.newValue;
                        //Debug.Log(string.Join("\n", navigableTags.NavigableTagsRef.Select(kv => kv.Key.Label + "=" + kv.Value).ToArray()));
                    }
                });
                //Debug.Log(string.Join("\n", navigableTags.NavigableTagsRef.Select(kv => kv.Key.Label + "=" + kv.Value).ToArray()));
                
                box.Add(toggle);
            }
        }

        protected override VisualElement CreateVisualElement()
        {
            return new VisualElement();
        }
    }
}
