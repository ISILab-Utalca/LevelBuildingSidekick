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
        ListView childBundles;

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

            #region Characteristics
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

            root.Insert(root.childCount, characteristics);
           
            foreach (var characteristic in bundle.Characteristics)
            {
                characteristic?.Init(bundle);    
            }

            #endregion

            #region Child Bundles
            var lv = root.Children().ToList()[5] as PropertyField;
            lv.TrackPropertyValue(serializedObject.FindProperty("childsBundles"), (sp) => 
            { 
                characteristics.RefreshItems(); Repaint(); 
            });
            
            // Create ListView for Child Bundles
            childBundles = new ListView();
            childBundles.headerTitle = "Child Bundles";
            childBundles.showAddRemoveFooter = true;
            childBundles.showBorder = true;
            childBundles.showFoldoutHeader = true;
            childBundles.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            childBundles.makeItem = MakeChildBundleItem;
            childBundles.bindItem = BindChildBundleItem;

            childBundles.itemsSource = bundle.ChildsBundles;
            
            childBundles.itemsAdded += view =>
            {
                var x = bundle.ChildsBundles.Last();
                bundle.ChildsBundles.RemoveAt(bundle.ChildsBundles.Count - 1);
                
                EditorGUI.BeginChangeCheck();
                Undo.RegisterCompleteObjectUndo(bundle, "Add child bundle");
                bundle.ChildsBundles.Add(x);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
                Undo.undoRedoPerformed += UNDO;
            };

            // Add itemsRemoved handler
            childBundles.itemsRemoved += (view) =>
            {
                ite
                foreach (var removedItem in view)
                {
                    if (childBundles. is Bundle removedBundle)
                    {
                        // Remove the child bundle from the parent bundle's child list
                        if (bundle.ChildsBundles.Contains(removedBundle))
                        {
                            EditorGUI.BeginChangeCheck();
                            Undo.RegisterCompleteObjectUndo(bundle, "Remove child bundle");
                            bundle.ChildsBundles.Remove(removedBundle);

                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                }
            };

           
            // Add a button to trigger the Add Child Bundle context menu
            var button = new Button(() =>
            {
                ShowAddChildBundleMenu(bundle);
            })
            {
                text = "Add Child Bundle"
            };

            
            root.Insert(root.childCount, childBundles);
            root.Insert(root.childCount, button);
            #endregion
            
            return root;
        }

        private void ShowAddChildBundleMenu(Bundle parentBundle)
        {
            // Get all available bundles in the project
            var allBundles = AssetDatabase.FindAssets("t:Bundle")
                .Select(guid => AssetDatabase.LoadAssetAtPath<Bundle>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            GenericMenu menu = new GenericMenu();

            // Get all parent bundles to avoid recursion
            List<Bundle> parents = new List<Bundle>();
            var currentParent = parentBundle;
            while (currentParent != null)
            {
                parents.Add(currentParent);
                currentParent = currentParent.Parent();
            }

            // Add valid child bundles to the menu
            foreach (var potentialChild in allBundles)
            {
                if (!potentialChild.Flags.HasFlag(parentBundle.Flags))
                {
                    Debug.Log("child missing bundle's flag: " + potentialChild.name);
                    continue;
                }
                if (parents.Contains(potentialChild)) 
                {
                    Debug.Log("child is a parent of the current bundle: " + potentialChild.name);
                    continue;
                }
                if (parentBundle.ChildsBundles.Contains(potentialChild))
                {
                    Debug.Log("bundle already contains child: " + potentialChild.name);
                    continue;
                }
                
                Debug.Log("valid for adding:" +potentialChild.name);
                
                menu.AddItem(new GUIContent(potentialChild.name), false, () =>
                {
                    // Add selected bundle to the child bundles list
                    parentBundle.ChildsBundles.Add(potentialChild);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                });
            }

            menu.ShowAsContext();
        }
        
        private void UNDO()
        {
            characteristics.Rebuild();
            childBundles.Rebuild();
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
                    (cf.Data as LBSCharacteristic)?.Init(bundle);
                };
            }
        }

        VisualElement MakeChildBundleItem()
        {
            var bundle = target as Bundle;
            var v = new ObjectField();
            v.objectType = typeof(Bundle);
            
            v.RegisterValueChangedCallback(evt =>
            {
                var newValue = evt.newValue as Bundle;
                if (newValue != null && newValue.Flags.HasFlag(bundle.Flags))
                {
                    v.value = newValue;
                }
           
                //(cf.Data as LBSCharacteristic)?.Init(bundle);
            });

            return v;
        }

        private void BindChildBundleItem(VisualElement ve, int index)
        {
            var bundle = target as Bundle;
            if (index < bundle.ChildsBundles.Count)
            {
                var cb = ve.Q<ObjectField>();
                cb.objectType = typeof(Bundle);
                
                if (bundle.ChildsBundles[index] != null)
                {
                    cb.value = bundle.ChildsBundles[index];
                }
                else{}
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