using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using UnityEditor.Playables;
using ISILab.Commons.Utility.Editor;
using UnityEditor.UIElements;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Weigths", typeof(LBSDirection))]
    public class LBSDirectionEditor : LBSCustomEditor
    {
        ObjectField[] fields;

        public LBSDirectionEditor()
        {

        }

        public LBSDirectionEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }

        public override void SetInfo(object obj)
        {
            this.target = obj;
            var target = obj as LBSDirection;

            if (target == null)
                return;

            var connections = target.Connections;

            for(int i = 0; i < fields.Length; i++)
            {
                fields[i].objectType = typeof(LBSTag);

                if (connections.Count <= i)
                {
                    //Debug.Log(connections.Count);
                    continue;
                }

                var tag = DirectoryTools.GetAssetByName<LBSTag>(connections[i]);

                fields[i].value = tag;

                var index = i;

                fields[i].RegisterValueChangedCallback(evt =>
                {
                    target.SetConnection(evt.newValue as LBSTag, index);
                });
            }
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSDirectionEditor");
            visualTree.CloneTree(this);

            fields = new ObjectField[4];

            fields[0] = this.Q<ObjectField>(name: "Right");
            fields[1] = this.Q<ObjectField>(name: "Up");
            fields[2] = this.Q<ObjectField>(name: "Left");
            fields[3] = this.Q<ObjectField>(name: "Down");

            return this;
        }
    }
}