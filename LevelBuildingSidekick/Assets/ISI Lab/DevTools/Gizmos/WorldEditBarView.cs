using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using UnityEditor.UIElements;
using LBS.Bundles;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Runtime.InteropServices;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISI_Lab.LBS.DevTools;

namespace ISILab.LBS.VisualElements
{
    public class WorldEditBarView : GraphElement
    {
        #region VIEW FIELDS
        private static VisualTreeAsset view;
        
        private EnumField typeField;
        private ObjectField bundleField;
        private Button resetButton;
        private Button shuffleButton;

        // reference to the gameObject containing the gizmo
        public GameObject go;
        #endregion

        public WorldEditBarView(GameObject targetComponent)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("WorldEditBarView");
            }
            view.CloneTree(this);
            
            typeField = this.Q<EnumField>("TypeField");

            bundleField = this.Q<ObjectField>("BundleField");
            resetButton = this.Q<Button>("ResetButton");
            shuffleButton = this.Q<Button>("ShuffleButton");
            
            typeField.RegisterValueChangedCallback(evt => OnTypeChanged(evt.newValue));
            bundleField.RegisterValueChangedCallback(evt => OnBundleChanged(evt.newValue));
            resetButton.clicked += OnResetClicked;
            shuffleButton.clicked += OnShuffleClicked;

            go = targetComponent;
            Debug.Log(go.name);
        }

        public void SetFields(Bundle bundleRef)
        {
            OnBundleChanged(bundleRef);
            OnTypeChanged(bundleRef.Type);
            MarkDirtyRepaint();
        }

        private void OnShuffleClicked()
        {
            //Get references
            LBSGenerated lbs = go.GetComponent<LBSGenerated>();
            
            //Call to SwapObject
            int pick = UnityEngine.Random.Range(0, lbs.BundleTemp.Assets.Count);
            SwapObject(lbs, lbs.BundleTemp.Assets[pick].obj);

            //Debug
            Debug.Log("Switching to asset " + (pick + 1) + " of " + lbs.BundleTemp.Assets.Count);
        }

        private void OnResetClicked()
        {
            LBSGenerated lbs = go.GetComponent<LBSGenerated>();
            OnBundleChanged(lbs.BundleRef);
        }

        private void OnBundleChanged(Object evtNewValue)
        {
            //Get references
            LBSGenerated lbs = go.GetComponent<LBSGenerated>();

            //Replace models if new bundle assigned
            if (lbs.BundleTemp != (Bundle)evtNewValue)
            {
                int pick = UnityEngine.Random.Range(0, ((Bundle)evtNewValue).Assets.Count);
                SwapObject(lbs, ((Bundle)evtNewValue).Assets[pick].obj, (Bundle)evtNewValue);
            }
            else
            {
                bundleField.value = evtNewValue;
            }
        }

        private void OnTypeChanged(Enum evtNewValue)
        {
            typeField.Init(evtNewValue);
            Debug.Log(evtNewValue.ToString());
        }

        private void SwapObject(LBSGenerated lbs, GameObject prefab, Bundle newBundle = null)
        {
            //Exception case
            if (prefab == null)
            {
                Debug.LogWarning("Can't execute SwapObject because prefab is null.");
                return;
            }

            //Instantiate new object
#if UNITY_EDITOR
            var ngo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            var ngo = GameObject.Instantiate(prefab);
#endif

            //Copy transform
            ngo.transform.position = go.transform.position;
            ngo.transform.rotation = go.transform.rotation;
            ngo.transform.localScale = go.transform.localScale;
            ngo.transform.SetParent(go.transform.parent);

            //Copy LBSGenerated component
            LBSGenerated nlbs = ngo.AddComponent<LBSGenerated>();
            nlbs.BundleRef = lbs.BundleRef;

            //Copy BundleTemp if no newBundle assigned
            if (newBundle != null)
            {
                nlbs.BundleTemp = newBundle;
            }
            else
            {
                nlbs.BundleTemp = lbs.BundleTemp;
            }

            //Destroy original
            GameObject.DestroyImmediate(go);
        }
    }
}