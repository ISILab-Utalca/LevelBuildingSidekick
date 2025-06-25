using ISI_Lab.LBS.DevTools;
using ISILab.LBS.Characteristics;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISI_Lab.LBS.Plugin.MapTools.Generators3D
{
    /// <summary>
    /// Component added to any game objects created by the LBS tool.
    /// Main function is to contain a reference to the object's bundle, for different functionalities.
    /// </summary>
    [ExecuteInEditMode] // Allows this to run in editor mode
    public class LBSGenerated : MonoBehaviour
    {
        
        #region FIELDS

        //Layer Id from which the object was created
        [SerializeField]
        private string layerName;
        
        //Original bundle reference
        [SerializeField]
        private Bundle bundleRef;
        
        //Temporal bundle reference (for when it changes using the WorldEditBar)
        [SerializeField]
        private Bundle bundleTemp;

        
        #endregion
        
        #region PROPERTIES
        
        public string LayerName
        {
            get => layerName;
            set => layerName = value;
        }

        public Bundle BundleRef
        {
            get => bundleRef;
            set
            {
                bundleRef = value;
                bundleTemp = value;
            }
        }
        
        public Bundle BundleTemp
        {
            get => bundleTemp;
            set => bundleTemp = value;
        }
        
        public int AssetIndex { get; set; } //Not very accurate, it adjusts when using the switch button in the WorldEditBar


        #endregion

        #region METHODS
 
        private void Reset()
        {
            // This runs when the script is first added to a GameObject
            EnsureGizmoComponent();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Schedule the AddComponent for after the current editor update
            EditorApplication.delayCall += TryAddGizmo;
        }

        private void TryAddGizmo()
        {
            // Check again that the object still exists and doesn't already have the component
            if (this != null && gameObject != null && !GetComponent<Custom3dMeshGizmo>())
            {
                Undo.AddComponent<Custom3dMeshGizmo>(gameObject); // Supports undo in editor
            }
        }
#endif

        private void EnsureGizmoComponent()
        {
            if (!GetComponent<Custom3dMeshGizmo>())
            {
                gameObject.AddComponent<Custom3dMeshGizmo>();
            }
        }
        
        public bool HasLBSTag(string paraTag)
        {
            var characteristics = bundleRef.Characteristics;
            foreach (var lbsChar in characteristics)
            {
                if (lbsChar is not LBSTagsCharacteristic lbsTagsCharacteristic) continue;
                if (lbsTagsCharacteristic.Value == null) continue;
                if (lbsTagsCharacteristic.Value.label != paraTag) continue;
                return true;
            }

            return false;
        }
        
        #endregion
    }
}
