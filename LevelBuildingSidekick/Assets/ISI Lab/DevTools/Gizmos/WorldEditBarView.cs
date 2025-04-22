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
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;

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
        bool isStructure;
        
        // other references
        LBSGenerated lbsComponent;
        #endregion

        public WorldEditBarView(LBSGenerated targetComponent)
        {
            view = DirectoryTools.GetAssetByName<VisualTreeAsset>("WorldEditBarView");
            view.CloneTree(this);

            typeField = this.Q<EnumField>("TypeField");
            bundleField = this.Q<ObjectField>("BundleField");
            resetButton = this.Q<Button>("ResetButton");
            shuffleButton = this.Q<Button>("ShuffleButton");

            typeField.RegisterValueChangedCallback(evt => OnTypeChanged(evt.newValue));
            bundleField.RegisterValueChangedCallback(evt => OnBundleChanged(evt.newValue));
            resetButton.clicked += OnResetClicked;
            shuffleButton.clicked += OnShuffleClicked;
            
            go = targetComponent.gameObject;
            lbsComponent = targetComponent;
            isStructure = lbsComponent.BundleTemp.Type == Bundle.TagType.Structural;
        }

        #region BUTTON METHODS
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
        #endregion

        #region FIELD CHANGED METHODS
        // ReSharper disable Unity.PerformanceAnalysis
        private void OnBundleChanged(Object evtNewValue)
        {
            //Replace models if new bundle assigned
            if (lbsComponent.BundleTemp != (Bundle)evtNewValue)
            {
                //Access to asset in bundle
                List<Asset> assets = ((Bundle)evtNewValue).Assets;

                if(assets.Count > 0)
                {
                    int pick = UnityEngine.Random.Range(0, assets.Count);
                    Asset asset = assets[pick];

                    if (asset != null)
                    {
                        SwapObject(lbsComponent, asset.obj, (Bundle)evtNewValue);    //Change asset
                    }
                    else
                    {
                        Debug.LogWarning("Can't replace model because newbundle asset is null");    //Exception case
                        bundleField.value = lbsComponent.BundleTemp;
                    }
                }
                else
                {
                    Debug.LogWarning("Can't replace model because newbundle has no assets");    //Exception case
                    bundleField.value = lbsComponent.BundleTemp;
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnTypeChanged(Enum evtNewValue)
        {   
            //Don't change if not structure
            if (!isStructure && StructureTags.None != (StructureTags)evtNewValue)
            {
                typeField.value = StructureTags.None;
            }
            else
            {
                //Get references
                typeField.value = evtNewValue;
                Bundle newBundle = lbsComponent.BundleTemp.Parent().ChildsBundles.Find(b => b.name.Contains(EnumToString((StructureTags)evtNewValue)));

                //Replace models if new type
                if (lbsComponent.BundleTemp != newBundle)
                {
                    //Access to asset in bundle
                    Asset asset = newBundle.Assets[0];      //ESTA LINEA PUEDE GENERAR EXCEPCIONES, REVISAR

                    if(asset != null)
                    {
                        SwapObject(lbsComponent, asset.obj, newBundle);  //Change asset
                    }
                    else
                    {
                        Debug.LogWarning("Can't replace model because newbundle asset is null");    //Exception case
                    }
                }
            }
            Debug.Log(evtNewValue.ToString());
        }
        #endregion

        public void SetFields(Bundle bundleRef)
        {
            List<LBSTagsCharacteristic> tags = bundleRef.GetCharacteristics<LBSTagsCharacteristic>();
            if (tags.Count > 0)
            {
                typeField.value = TagToEnum(tags[0].Value);
            }
            else
            {
                typeField.value = StructureTags.None;
            }

            bundleField.value = bundleRef;
            MarkDirtyRepaint();
        }

        private bool SwapObject(LBSGenerated lbs, GameObject prefab, Bundle newBundle = null)
        {
            //Exception case
            if (!prefab)
            {
                Debug.LogWarning("Can't execute SwapObject because prefab is null.");
                return false;
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
            return true;
        }

        //Returns the StructureBundle enum value according to a LBSTag
        StructureTags TagToEnum(LBSTag tag)
        {
            switch(tag.label){
                case "Wall":
                    return StructureTags.Wall;
                case "Floor":
                    return StructureTags.Floor;
                case "Window":
                    return StructureTags.Window;
                case "Door":
                    return StructureTags.Door;
                case "Corner":
                    return StructureTags.Corner;
                default: return StructureTags.None;
            }
        }

        //Returns a string value according to a StructureBundle enum value
        string EnumToString(StructureTags type)
        {
            switch (type){
                case StructureTags.None:
                    return "None";
                case StructureTags.Wall:
                    return "Wall";
                case StructureTags.Floor:
                    return "Floor";
                case StructureTags.Window:
                    return "Window";
                case StructureTags.Door:
                    return "Door";
                case StructureTags.Corner:
                    return "Corner";
                default: return "None";
            }
        }
    }

    public enum StructureTags
    {
        None, Wall, Floor, Window, Door, Corner
    }
}