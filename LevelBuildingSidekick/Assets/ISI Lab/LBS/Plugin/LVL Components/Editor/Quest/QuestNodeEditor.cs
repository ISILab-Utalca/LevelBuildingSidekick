using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestNode", typeof(QuestNode))]
    public class QuestNodeEditor : LBSCustomEditor
    {
        RectField rect;
        ListView tags;
        Label label;

        public QuestNodeEditor()
        {
            CreateVisualElement();
        }

        public QuestNodeEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            var node = target as QuestNode;
            this.target = target;
            if (node == null)
                return;

            label.text = node.ID;
            rect.RegisterValueChangedCallback(evt => node.Target.Rect = evt.newValue);

            tags.itemsSource = node.Target.Tags;
            tags.Rebuild();

            rect.value = node.Target.Rect;
        }

        protected override VisualElement CreateVisualElement()
        {
            var node = target as QuestNode;

            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestNodeEditor");
            visualTree.CloneTree(this);

            label = this.Q<Label>();

            rect = this.Q<RectField>();

            tags = this.Q<ListView>();

            tags.itemsSource = new List<LBSIdentifier>();

            tags.makeItem = MakeTagItem;
            tags.bindItem = BindTagItem;

            return this;
        }

        public VisualElement MakeTagItem()
        {
            var of = new ObjectField("Tag: ");
            of.objectType = typeof(LBSIdentifier);

            return of;
        }

        public void BindTagItem(VisualElement visualElement, int index)
        {
            var node = target as QuestNode;

            if (index >= node.Target.Tags.Count)
                return;

            var of = visualElement as ObjectField;

            of.value = node.Target.Tags[index];

            of.RegisterValueChangedCallback(evt => node.Target.Tags[index] = evt.newValue as LBSIdentifier);
        }


    }
}