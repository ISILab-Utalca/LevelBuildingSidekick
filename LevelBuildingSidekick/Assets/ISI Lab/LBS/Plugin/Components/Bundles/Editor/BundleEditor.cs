
using System.Linq;
using UnityEditor;
using UnityEngine;

using ISILab.LBS.Characteristics;
using UnityEngine.UIElements;

using UnityEditor.UIElements;
using LBS.Bundles;
using System;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements;


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

            //Erase empty children
            if (bundle != null)
            {
                bundle.RemoveNullChildren();
            }
                    
            // Element option
            if (bundle != null && bundle.Type == Bundle.TagType.Element)
            {
                SerializedProperty populationTypeProp = serializedObject.FindProperty("populationType");
                if (populationTypeProp != null)
                {
                    PropertyField populationField = new PropertyField(populationTypeProp);
                    populationField.Bind(serializedObject);
                    root.Add(populationField);
                }
                SerializedProperty tileSizeProp = serializedObject.FindProperty("tileSize");
                if (tileSizeProp != null)
                {
                    PropertyField tileSizeField = new PropertyField(tileSizeProp);
                    tileSizeField.Bind(serializedObject);
                    root.Add(tileSizeField);
                }
            }
            
            #region Characteristics

            characteristics = new ListView();
            characteristics.headerTitle = "Characteristics";
            characteristics.showAddRemoveFooter = true;
            characteristics.showBorder = true;
            characteristics.showFoldoutHeader = true;
            characteristics.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            characteristics.makeItem = MakeItem;
            characteristics.bindItem = BindItem;

            characteristics.itemsSource = bundle!.Characteristics;

            characteristics.itemsAdded += view =>
            {
                var x = bundle!.Characteristics.Last();
                bundle.Characteristics.RemoveAt(bundle.Characteristics.Count - 1);

                EditorGUI.BeginChangeCheck();
                Undo.RegisterCompleteObjectUndo(bundle, "Add characteristics");
                bundle.Characteristics.Add(x);

                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                Undo.undoRedoPerformed += UNDO;
            };

            root.Insert(root.childCount, characteristics);

            foreach (var characteristic in bundle!.Characteristics)
            {
                characteristic?.Init(bundle);
            }

            #endregion

            #region Child Bundles

            var lv = root.Children().ToList()[5] as PropertyField;
            lv.TrackPropertyValue(serializedObject.FindProperty("characteristics"), (sp) =>
            {
         
                //foreach (var child in bundle.Characteristics) Debug.Log(child);
                
                characteristics.RefreshItems();
                Repaint();
            });

            // Create ListView for Child Bundles
            childBundles = new ListView();
            childBundles.headerTitle = "Child Bundles";
            childBundles.reorderable = false;
            childBundles.showAddRemoveFooter = false;
            childBundles.showBorder = true;
            childBundles.showFoldoutHeader = true;
            childBundles.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            childBundles.makeItem = MakeChildBundleItem;
            childBundles.bindItem = BindChildBundleItem;

            childBundles.itemsSource = bundle.ChildsBundles;
            
            
            childBundles.itemsAdded += view =>
            {
                if (bundle.ChildsBundles.Count == 0) return;
                var x = bundle.ChildsBundles.Last();
                bundle.ChildsBundles.RemoveAt(bundle.ChildsBundles.Count - 1);
                
                EditorGUI.BeginChangeCheck();
                Undo.RegisterCompleteObjectUndo(bundle, "Add child bundle");
                bundle.AddChild(x);

                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                Undo.undoRedoPerformed += UNDO;
            };
            
            childBundles.itemsRemoved += (view) =>
            {
                if(bundle.ChildsBundles.Count == 0) return;
                
                var x = bundle.ChildsBundles.Last();
                // Remove the child bundle from the parent bundle's child list
                if (bundle.ChildsBundles.Contains(x))
                {
                    EditorGUI.BeginChangeCheck(); 
                    Undo.RegisterCompleteObjectUndo(bundle, "Remove child bundle");
                    bundle.RemoveChild(x);

                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                    Undo.undoRedoPerformed += UNDO;
                }
            };
            
            root.Insert(root.childCount, childBundles);
            
            var addButton = new Button(() =>
            {
                EditorGUI.BeginChangeCheck();
                
                ShowAddChildBundleMenu(bundle);
                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                Undo.undoRedoPerformed += UNDO;
                
            }){ text = "Add Child Bundle" };
            
            
            var removeButton = new Button(() =>
            {
                EditorGUI.BeginChangeCheck();
                
                ShowRemoveChildBundle(bundle);
                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                Undo.undoRedoPerformed += UNDO;
                
            }){ text = "Remove Child Bundle"};
            
            
            var cblv = root.Children().ToList()[6] as PropertyField;
            cblv.TrackPropertyValue(serializedObject.FindProperty("childsBundles"), (sp) =>
            {
                //foreach (var child in bundle.ChildsBundles) Debug.Log(child);
                
                childBundles.itemsSource = bundle.ChildsBundles;
                characteristics.RefreshItems();
                childBundles.RefreshItems();
                Repaint();
            });
            
            
            VisualElement buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row; 
            buttonContainer.style.alignItems = Align.Auto;
            buttonContainer.style.justifyContent = Justify.Center; 
            
            buttonContainer.Add(addButton);
            buttonContainer.Add(removeButton);
            
            root.Add(buttonContainer);
            
            #endregion
    

            
            return root;
        }
        
        private void ShowRemoveChildBundle(Bundle bundle)
        {
            var allBundles = bundle.ChildsBundles;
            
            GenericMenu menu = new GenericMenu();
            
            // Add existing child bundles to the menu
            foreach (var potentialChild in allBundles)
            {
                if (potentialChild != null)
                {
                    menu.AddItem(new GUIContent(potentialChild.name), false, () =>
                    {
                        // Add selected bundle to the child bundles list
                        if (MakeChildBundleItem() is ObjectField cb)
                        {
                            EditorGUI.BeginChangeCheck(); 
                            Undo.RegisterCompleteObjectUndo(bundle, "Remove child bundle");
                            //  cb.SetValueWithoutNotify(potentialChild);
                            bundle.RemoveChild(potentialChild);
                        
                            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                            Undo.undoRedoPerformed += UNDO;
                        }
                  
                    });
                }
            }
            menu.ShowAsContext();
        }
        
        private void ShowAddChildBundleMenu(Bundle bundle)
        {
            // Get all available bundles in the project
            var allBundles = AssetDatabase.FindAssets("t:Bundle")
                .Select(guid => AssetDatabase.LoadAssetAtPath<Bundle>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            GenericMenu menu = new GenericMenu();
            
            // Add valid child bundles to the menu
            foreach (var potentialChild in allBundles)
            {
                if (!bundle.IsBundleValidChild(potentialChild))  continue;
                menu.AddItem(new GUIContent(potentialChild.name), false, () =>
                {
                    // Add selected bundle to the child bundles list
                    if (MakeChildBundleItem() is ObjectField cb)
                    {
                        EditorGUI.BeginChangeCheck(); 
                        Undo.RegisterCompleteObjectUndo(bundle, "Add child bundle");
                        cb.SetValueWithoutNotify(potentialChild);
                        bundle.AddChild(potentialChild);
                        
                        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
                        Undo.undoRedoPerformed += UNDO;
                    }
                  
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
            if (index < bundle!.Characteristics.Count)
            {
                var cf = ve.Q<DynamicFoldout>();
                cf.Label = "Characteristic " + index + ":";
                if (bundle.Characteristics[index] != null)
                {
                    cf.Data = bundle.Characteristics[index];
                    bundle.Characteristics[index].Init(bundle);
                }

                cf.OnChoiceSelection = () =>
                {
                    bundle.Characteristics[index] = cf.Data as LBSCharacteristic;
                    (cf.Data as LBSCharacteristic)?.Init(bundle);
                };
            }
        }

        VisualElement MakeChildBundleItem()
        {
            var bundle = target as Bundle;
            var v = new ObjectField();
            v.objectType = typeof(Bundle);
            v.SetEnabled(false);
            v.RegisterValueChangedCallback(HandleChildBundleChange);
            
            return v;
        }

        private void BindChildBundleItem(VisualElement ve, int index)
        {
            var bundle = target as Bundle;
            if (index < bundle!.ChildsBundles.Count)
            {
                var cb = ve.Q<ObjectField>();
                cb.objectType = typeof(Bundle);
                
                if (bundle.ChildsBundles[index] != null)
                {
                    cb.value = bundle.ChildsBundles[index];
                   // bundle.ChildsBundles[index].Reload();
                }

                //cb.RegisterValueChangedCallback(HandleChildBundleChange);
            }
        }

        private void HandleChildBundleChange(ChangeEvent<UnityEngine.Object> evt)
        {
            var parent = target as Bundle;
            if (parent == null) return;
            
            var newBundle = evt.newValue as Bundle;
            if (newBundle == null) return;

            if (!parent.IsBundleValidChild(newBundle)) return;
            
            parent.AddChild(newBundle);
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