using System.Collections.Generic;
using System.Linq;
using ISILab.Macros;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;

namespace ISI_Lab.LBS.Plugin.Components.Bundles
{
    public static class BundleMenuItem
    {
        #region UNITY MENU RELATED METHODS
        [MenuItem("Assets/Create/ISILab/LBS/Bundle &b")]
        private static void CreateBundle()
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
            CreateBundleAsset(obj, "New Bundle.asset");
            //ProjectWindowUtil.CreateAsset(obj, "New Bundle.asset");
        }
        
        private static void CreateBundleFromPrefab(IEnumerable<GameObject> prefabs)
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
            var endAction = ScriptableObject.CreateInstance<EndBundleNameEditAction>();
            CreateBundleAsset(obj, name + ".asset"); // New function. Test require for this case.
            //ProjectWindowUtil.CreateAsset(obj, name + ".asset");
        }

        static void CreateBundleAsset(UnityEngine.Object obj, string pathName)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                obj.GetInstanceID(),
                ScriptableObject.CreateInstance<EndBundleNameEditAction>(),
                pathName,
                AssetPreview.GetMiniThumbnail(obj),
                null
                );
        }

        private static bool IsPrefab(GameObject go)
        {
            return AssetDatabase.GetAssetPath(go).Contains(".prefab");
        }
        #endregion
        
        #region PUBLIC METHODS
        public static Bundle CreateBundle(BundleFlags flags, string baseName = "New_Bundle")
        {
            Bundle obj = ScriptableObject.CreateInstance<Bundle>();
            obj.LayerContentFlags = flags;

            string name = baseName;
            int counter = 0;
            while (AssetDatabase.AssetPathExists("Assets/" + name + ".asset"))
            {
                counter++;
                name = baseName + "_" + counter;
            }
            
            AssetDatabase.CreateAsset(obj, "Assets/" + name + ".asset");
            return obj;
        }
        
        public static BundleCollection CreateBundleCollection(string baseName = "New_Collection")
        {
            BundleCollection obj = ScriptableObject.CreateInstance<BundleCollection>();

            string name = baseName;
            int counter = 0;
            while (AssetDatabase.AssetPathExists("Assets/" + name + ".asset"))
            {
                counter++;
                name = baseName + "_" + counter;
            }
            
            AssetDatabase.CreateAsset(obj, "Assets/" + name + ".asset");
            return obj;
        }
        #endregion
    }

    internal class EndBundleNameEditAction : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Bundle bundle = EditorUtility.InstanceIDToObject(instanceId) as Bundle;
            AssetDatabase.CreateAsset(bundle, AssetDatabase.GenerateUniqueAssetPath(pathName));
            bundle.GUID = LBSAssetMacro.GetGuidFromAsset(bundle);
            Debug.Log($"Created new Bundle: '{bundle.name}', '{bundle.GUID}'");
            AssetDatabase.Refresh();
        }

        public override void Cancelled(int instanceId, string pathName, string resourceFile)
        {
            Bundle bundle = EditorUtility.InstanceIDToObject(instanceId) as Bundle;
            DestroyImmediate(bundle, true);
        }
    }
}
