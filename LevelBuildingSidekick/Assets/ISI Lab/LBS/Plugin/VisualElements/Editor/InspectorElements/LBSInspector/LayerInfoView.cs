using ISILab.Commons.Utility.Editor;
using LBS.Components;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;


namespace LBS.VisualElements
{
    [UxmlElement]
    public partial class LayerInfoView : VisualElement
    {
        #region FACTORY
        //public new class UxmlFactory : UxmlFactory<LayerInfoView, VisualElement.UxmlTraits> { }
        #endregion

        private Foldout foldout;
        private VisualElement content;

        private TextField textField;
        private ObjectField objectField;
        private Vector2IntField sizeField;

        public LayerInfoView()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayerInfoView");
            visualTree.CloneTree(this);

            foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                content.SetDisplay(evt.newValue);
            });

            content = this.Q<VisualElement>("Content");

            textField = this.Q<TextField>();
            textField.SetEnabled(false);

            objectField = this.Q<ObjectField>();
            objectField.SetEnabled(false);

            sizeField = this.Q<Vector2IntField>();
            sizeField.SetEnabled(false);
        }

        public void SetInfo(LBSLayer layer)
        {
            if (layer == null)
            {
                content.style.display = DisplayStyle.None;
                return;
            }
            content.style.display = DisplayStyle.Flex;
            
            textField.value = layer.Name;
            objectField.value = AssetDatabase.LoadAssetAtPath<Texture2D>(layer.iconGuid);
            sizeField.value = layer.TileSize;
        }
    }
}
