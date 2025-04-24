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
        
        private bool isStructure;

        // reference to the gameObject containing the gizmo
        private GameObject go;
        
        // other references
        private LBSGenerated lbsComponent;
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
            //Pick an asset
            lbsComponent.AssetIndex++;
            if (lbsComponent.AssetIndex >= lbsComponent.BundleTemp.Assets.Count)
            {
                lbsComponent.AssetIndex = 0;
            }   
            
            //Debug
            Debug.Log("Switching to asset " + (lbsComponent.AssetIndex + 1) + " of " + lbsComponent.BundleTemp.Assets.Count);
            
            //Call SwapObject
            SwapObject(lbsComponent, lbsComponent.BundleTemp.Assets[lbsComponent.AssetIndex].obj);
        }

        private void OnResetClicked()
        {
            bundleField.value = lbsComponent.BundleRef;
        }
        #endregion

        #region FIELD CHANGED METHODS
        // ReSharper disable Unity.PerformanceAnalysis
        private void OnBundleChanged(Object evtNewValue)
        {
            //Replace models if new bundle assigned
            if (lbsComponent.BundleTemp != (Bundle)evtNewValue)
            {
                //Access to asset in the bundle
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
                        Debug.LogError("Can't replace model because bundle " + ((Bundle)evtNewValue).Name + "'s chosen asset " + pick + " is null");    //Exception case
                        bundleField.value = lbsComponent.BundleTemp;
                    }
                }
                else
                {
                    Debug.LogError("Can't replace model because bundle " + ((Bundle)evtNewValue).Name + " has no assets");    //Exception case
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
            else if (isStructure)
            {
                //Get references
                Bundle newBundle = lbsComponent.BundleTemp.Parent().ChildsBundles.Find(b => b.name.Contains(EnumToString((StructureTags)evtNewValue)));

                //Replace models if new type
                if (newBundle && lbsComponent.BundleTemp != newBundle)
                {   
                    if (newBundle.Assets.Count <= 0)
                    {
                        Debug.LogError("Can't replace model because bundle " + newBundle.Name + " doesn't have any assets");    //Exception case
                    }
                    else
                    {
                        //Access to asset in the bundle
                        int pick = UnityEngine.Random.Range(0, newBundle.Assets.Count);
                        Asset asset = newBundle.Assets[pick];

                        if(asset != null)
                        {
                            SwapObject(lbsComponent, asset.obj, newBundle);  //Change asset
                        }
                        else
                        {
                            Debug.LogError("Can't replace model because bundle " + newBundle.Name + "'s asset " + pick + " is null");    //Exception case
                        }
                    }
                }
                else if(!newBundle)
                {
                    //Blocks changing to an unavailable type
                    List<LBSTagsCharacteristic> tags = lbsComponent.BundleTemp.GetCharacteristics<LBSTagsCharacteristic>();
                    typeField.value = tags.Count > 0 ? TagToEnum(tags[0].Value) : StructureTags.None;

                    if ((StructureTags)evtNewValue != StructureTags.None)   //Structures never have a None tag, so it'd be obvious
                    {
                        Debug.LogError("Can't replace model because there's no sibling bundle in " + lbsComponent.BundleTemp.Parent().Name + " named with that type"); //Exception case   
                    }
                }
            }
        }
        #endregion

        //Set all fields according to reference 
        public void SetFields(Bundle bundleRef)
        {
            //Set typeField to tag in characteristics if found
            List<LBSTagsCharacteristic> tags = bundleRef.GetCharacteristics<LBSTagsCharacteristic>();
            typeField.value = tags.Count > 0 ? TagToEnum(tags[0].Value) : StructureTags.None;

            bundleField.value = bundleRef;
            MarkDirtyRepaint();
        }

        //Creates a new object to replace the current one
        private bool SwapObject(LBSGenerated lbs, GameObject prefab, Bundle newBundle = null)
        {
            //Exception case
            if (!prefab)
            {
                Debug.LogError("Can't execute SwapObject because prefab is null.");
                return false;
            }

            //Instantiate new object
#if UNITY_EDITOR
            GameObject ngo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            GameObject ngo = GameObject.Instantiate(prefab);
#endif
            Undo.RegisterCreatedObjectUndo(ngo, "Create replacement object");
            
            //Exception case
            if (!ngo)
            {
                Debug.LogError("Failed to instantiate prefab");
                return false;
            }
            
            //Copy transform
            ngo.transform.SetPositionAndRotation(go.transform.position, go.transform.rotation);
            ngo.transform.localScale = go.transform.localScale;
            ngo.transform.SetParent(go.transform.parent);

            //Copy LBSGenerated component
            LBSGenerated newLbs = ngo.AddComponent<LBSGenerated>();
            newLbs.BundleRef = lbs.BundleRef;
            newLbs.AssetIndex = lbs.AssetIndex;

            //Copy BundleTemp if no newBundle assigned (useful when called from shuffle)
            newLbs.BundleTemp = newBundle ? newBundle : lbs.BundleTemp;

            //Destroy original
            Undo.DestroyObjectImmediate(go);;
            return true;
        }

        //Returns the StructureBundle enum value according to a LBSTag
        private static StructureTags TagToEnum(LBSTag tag)
        {
            return tag.label switch
            {
                "Wall" => StructureTags.Wall,
                "Floor" => StructureTags.Floor,
                "Window" => StructureTags.Window,
                "Door" => StructureTags.Door,
                "Corner" => StructureTags.Corner,
                _ => StructureTags.None
            };
        }

        //Returns a string value according to a StructureBundle enum value
        private static string EnumToString(StructureTags type)
        {
            return type switch
            {
                StructureTags.None => "None",
                StructureTags.Wall => "Wall",
                StructureTags.Floor => "Floor",
                StructureTags.Window => "Window",
                StructureTags.Door => "Door",
                StructureTags.Corner => "Corner",
                _ => "None"
            };
        }
    }

    public enum StructureTags
    {
        None, Wall, Floor, Window, Door, Corner
    }
}