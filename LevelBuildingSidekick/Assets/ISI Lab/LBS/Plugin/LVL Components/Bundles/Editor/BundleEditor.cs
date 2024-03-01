using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using UnityEngine.UIElements;
using ISILab.LBS.Components;
using UnityEditor.UIElements;
using LBS.Bundles;
using System;
using ISILab.LBS.VisualElements;
using ISILab.LBS.AI.Categorization;
using System.Reflection;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEditor.TerrainTools;

namespace ISILab.LBS.Bundles.Editor
{
    [CustomEditor(typeof(Bundle))]
    public class BundleEditor : UnityEditor.Editor
    {
        ListView characteristics;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Event.current.commandName == "UndoRedoPerformed")
            {
                this.Repaint();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, this.serializedObject, this);
            var bundle = target as Bundle;

            characteristics = new ListView();
            characteristics.headerTitle = "Characteristics";
            characteristics.showAddRemoveFooter = true;
            characteristics.showBorder = true;
            characteristics.showFoldoutHeader = true;
            characteristics.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            characteristics.makeItem = MakeItem;
            characteristics.bindItem = BindItem;

            characteristics.itemsSource = bundle.characteristics;

            characteristics.itemsAdded += view =>
            {

                var x = bundle.characteristics.Last();
                bundle.characteristics.RemoveAt(bundle.characteristics.Count - 1);

                EditorGUI.BeginChangeCheck();
                Undo.RegisterCompleteObjectUndo(bundle, "Add characteristics");
                bundle.characteristics.Add(x);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
                Undo.undoRedoPerformed += UNDO;
            };

            root.Insert(root.childCount - 1, characteristics);

            //ListView of ChildBundles
            var lv = root.Children().ToList()[5] as PropertyField;
            lv.TrackPropertyValue(serializedObject.FindProperty("childsBundles"), (sp) => 
            { 
                characteristics.RefreshItems(); Repaint(); 
            });

            foreach (var characteristic in bundle.Characteristics)
            {
                characteristic?.Init(bundle);    
            }

            return root;
        }

        private void UNDO()
        {
            characteristics.Rebuild();
            Undo.undoRedoPerformed -= UNDO;
        }

        VisualElement MakeItem()
        {
            var bundle = target as Bundle;
            var v = new DynamicFoldout(typeof(LBSCharacteristic));
            return v;
        }

        void BindItem(VisualElement ve, int index)
        {
            var bundle = target as Bundle;
            if (index < bundle.characteristics.Count)
            {
                var cf = ve.Q<DynamicFoldout>();
                cf.Label = "Characteristic " + index + ":";
                if (bundle.characteristics[index] != null)
                {
                    cf.Data = bundle.characteristics[index];
                    bundle.characteristics[index].Init(bundle);
                }

                cf.OnChoiceSelection = () =>
                {
                    bundle.characteristics[index] = cf.Data as LBSCharacteristic;
                    (cf.Data as LBSCharacteristic).Init(bundle);
                };
            }
        }

        public void Save()
        {
            EditorUtility.SetDirty(target);
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(target);
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(target);
        }

    }
}