using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using LBS.Bundles;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.InputField;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Zone", typeof(Zone))]
    public class ZoneEditor : LBSCustomEditor
    {
        private Zone _target;

        private TextField nameField;
        private ColorField colorField;
        private ObjectField objectField;
        private ObjectField objectField2;

        private ListView bundleList;
        private List<string> bundlesRef;

        public ZoneEditor()
        {
            CreateVisualElement();
        }

        public override void SetInfo(object paramTarget)
        {
            // Set referenced Zone
            _target = paramTarget as Zone;

            // Get bundles
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

            // Set basic values
            nameField.value = _target.ID;
            colorField.value = _target.Color;

            if (_target.InsideStyles.Count > 0)
            {
                foreach (var bundle in bundles)
                {
                    if (bundle.name == _target.InsideStyles[0])
                    {
                        objectField.value = bundle;
                        break;
                    }
                }
            }

            if (_target.OutsideStyles.Count > 0)
            {
                foreach (var bundle in bundles)
                {
                    if (bundle.name == _target.OutsideStyles[0])
                    {
                        objectField2.value = bundle;
                        break;
                    }
                }
            }

            bundlesRef = _target.InsideStyles;
        }

        protected override VisualElement CreateVisualElement()
        {
            // NameField
            nameField = new TextField("Name");
            nameField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                _target.ID = evt.newValue;
            });
            Add(nameField);

            // ColorField
            colorField = new ColorField("Color");
            colorField.RegisterCallback<ChangeEvent<Color>>(evt =>
            {
                _target.Color = evt.newValue;
            });
            Add(colorField);

            // ObjectField (Bundle)
            objectField = new ObjectField("Inside Style");
            objectField.objectType = typeof(Bundle);
            objectField.RegisterValueChangedCallback(v =>
            {
                var bundle = v.newValue as Bundle;
                _target.InsideStyles = new List<string>() { bundle.name };
            });
            Add(objectField);

            // ObjectField (Bundle)
            objectField2 = new ObjectField("Outside Style");
            objectField2.objectType = typeof(Bundle);
            objectField2.RegisterValueChangedCallback(v =>
            {
                var bundle = v.newValue as Bundle;
                _target.OutsideStyles = new List<string>() { bundle.name };
            });
            Add(objectField2);

            return this;

        }

        private VisualElement MakeItem()
        {
            var field = new ObjectField();
            field.objectType = typeof(Bundle);
            return field;
        }

        private void BindItem(VisualElement ve, int index)
        {
            var field = ve as ObjectField;

            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
            bundles = bundles.Where(b => b.Name == bundlesRef[index]).ToList();

            field.value = bundles[index];
        }

    }
}