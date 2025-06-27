using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;

namespace ISI_Lab.LBS.Plugin.Components.Bundles
{
    public static class BundleMenuItem
    {
        [MenuItem("Assets/Create/ISILab/LBS/Bundle &b")]
        static void CreateBundle()
        {
            GameObject[] list = Selection.gameObjects;
            if (list is { Length: > 0 })    // Selection not empty
            {
                IEnumerable<GameObject> validPrefabs = list.Where(go => IsPrefab(go));
                CreateBundleFromPrefab(validPrefabs);
                return;
            }
            
            // Create empty bundle
            Bundle obj = ScriptableObject.CreateInstance<Bundle>();
            //ProjectWindowUtil.CreateAsset(obj, "New Bundle.asset");

            AssetDatabase.CreateAsset(obj, "New Bundle.asset");
            AssetDatabase.SaveAssets();
        }

        static void CreateBundleFromPrefab(GameObject prefab)
        {
            Bundle obj = ScriptableObject.CreateInstance<Bundle>();
            obj.AddAsset(new Asset(prefab, 0.5f));
            
            ProjectWindowUtil.CreateAsset(obj, prefab.name + ".asset");
            Debug.Log("Creando Bundle desde un prefab");
        }
        
        static void CreateBundleFromPrefab(IEnumerable<GameObject> prefabs)
        {
            string name = null;
            Bundle obj = ScriptableObject.CreateInstance<Bundle>();
            foreach (GameObject prefab in prefabs)
            {
                if (prefab == null) continue;
                
                name ??= prefab.name;
                obj.AddAsset(new Asset(prefab, 0.5f));
            }

            name ??= "New Bundle";
            name = name.Replace("prefab", "bundle");
            ProjectWindowUtil.CreateAsset(obj, name + ".asset");
        }

        //static void CreateBundleAsset(UnityEngine.Object obj, string pathName)
        //{
        //    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
        //        obj.GetInstanceID(),
        //        () => {
        //            ScriptableObject.CreateInstance<UnityEditor.ProjectWindowCallback.DoCreateNewAsset>();
        //
        //
        //        },
        //        pathName,
        //        AssetPreview.GetMiniThumbnail(obj),
        //        null
        //        );
        //}

        static bool IsPrefab(GameObject go)
        {
            return AssetDatabase.GetAssetPath(go).Contains(".prefab");
        }

    }

    //class EndBundleNameEditAction : UnityEditor.ProjectWindowCallback.EndNameEditAction
    //{
    //    public override void Action(int instanceId, string pathName, string resourceFile)
    //    {
    //        AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
    //        ProjectWindowUtil.FrameObjectInProjectWindow(instanceId);
    //    }
    //}
}
