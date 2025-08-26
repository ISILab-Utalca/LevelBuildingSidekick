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
using LBS.Bundles;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Weights", typeof(LBSDirection))]
    public class LBSDirectionEditor : LBSCustomEditor
    {
        ObjectField[] fields;
        VisualElement renderView;

        private Button openDirectionToolButton;
        private BundleDirectionEditorWindow directionWindow;

        public LBSDirectionEditor()
        {
            
        }

        public LBSDirectionEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);

            directionWindow = ScriptableObject.CreateInstance<BundleDirectionEditorWindow>();
            directionWindow.target = target as LBSDirection;
        }

        public override void SetInfo(object _paramTarget)
        {
            this.target = _paramTarget;
            LBSDirection target = _paramTarget as LBSDirection;

            if (target == null)
                return;
            
            target.Size = 4;
            var connections = target.Connections;

            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].objectType = typeof(LBSTag);

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
            renderView = this.Q("Render");

            fields[0] = this.Q<ObjectField>(name: "Right");
            fields[1] = this.Q<ObjectField>(name: "Up");
            fields[2] = this.Q<ObjectField>(name: "Left");
            fields[3] = this.Q<ObjectField>(name: "Down");

            openDirectionToolButton = this.Q<Button>("OpenDirectionToolButton");
            openDirectionToolButton.clicked += () => OpenDirectionTool();

            return this;
        }


        public void SetModelRenderThumbnail()
        {

            if (renderView == null) return;


        }

        private void OpenDirectionTool()
        {
            directionWindow.Show();
        }
    }
}