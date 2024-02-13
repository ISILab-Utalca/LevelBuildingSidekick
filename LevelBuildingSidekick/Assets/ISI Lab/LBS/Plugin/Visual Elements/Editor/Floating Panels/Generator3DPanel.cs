using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;
using LBS;
using System;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Generators;

namespace ISILab.LBS.VisualElements.Editor
{
    public class Generator3DPanel : VisualElement
    {
        #region UXMLFACTORY
        public new class UxmlFactory : UxmlFactory<Generator3DPanel, VisualElement.UxmlTraits> { }
        #endregion

        #region VIEW ELEMENTS
        private ClassDropDown dropDownField;
        private Vector3Field positionField;
        private Vector2Field scaleField;
        private Vector2Field resizeField;
        private TextField nameField;
        private Button action;
        private Toggle destroyPrev;
        #endregion

        #region FIELDS
        private Generator3D generator;
        private Generator3D.Settings settings;
        private LBSLayer layer;
        #endregion

        #region EVENTS
        public Action OnExecute;
        #endregion

        #region PROPERTIES
        public Generator3D Generator
        {
            get => generator;
            set => generator = value;
        }
        #endregion

        #region CONSTRUCTORS
        public Generator3DPanel()
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel");
            visualTree.CloneTree(this);

            positionField = this.Q<Vector3Field>(name: "Position");

            resizeField = this.Q<Vector2Field>(name: "Resize");
            resizeField.value = Vector2.one;

            scaleField = this.Q<Vector2Field>(name: "ReferenceSize");
            scaleField.value = new Vector2(2, 2);

            nameField = this.Q<TextField>(name: "ObjName");

            dropDownField = this.Q<ClassDropDown>(name: "Generator");

            dropDownField.label = "Gennerator";
            dropDownField.Type = typeof(Generator3D);

            destroyPrev = this.Q<Toggle>(name: "DestroyPrev");

            action = this.Q<Button>(name: "Action");
            action.clicked += OnExecute;
            action.clicked += Execute;
        }
        #endregion

        public void Init(LBSLayer layer)
        {
            if (layer == null)
            {
                Debug.LogWarning("[ISI Lab] Warning: You don't have any layer selected.");
                return;
            }

            this.layer = layer;
            this.generator = new Generator3D();

            generator.settings = settings;
            this.settings = layer.Settings;


            if (generator != null)
            {
                dropDownField.Value = generator.GetType().Name;
                scaleField.value = settings.scale;
                positionField.value = settings.position;
                nameField.value = layer.Name;
                resizeField.value = settings.resize;
            }
        }

        public void Execute()
        {
            if (this.layer == null)
            {
                Debug.LogError("[ISI LAB]: There is no reference for any layer to generate.");
                return;
            }

            if (destroyPrev.value)
            {
                var prev = GameObject.Find(nameField.value);
                if (prev != null)
                {
                    GameObject.DestroyImmediate(prev);
                }
            }

            if (generator == null)
            {
                generator = dropDownField.GetChoiceInstance() as Generator3D;
            }

            var settings = new Generator3D.Settings
            {
                name = nameField.value,
                position = positionField.value,
                resize = resizeField.value,
                scale = scaleField.value,
            };

            var obj = generator.Generate(this.layer, this.layer.GeneratorRules, settings);
            Undo.RegisterCreatedObjectUndo(obj, "Create my GameObject");
        }


    }
}