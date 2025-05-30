using ISI_Lab.LBS.DevTools;
using ISILab.LBS.Characteristics;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;

namespace ISI_Lab.LBS.Plugin.MapTools.Generators3D
{
    /// <summary>
    /// Component added to any game objects created by the LBS tool.
    /// Main function is to contain a reference to the object's bundle, for different functionalities.
    /// </summary>
    [ExecuteInEditMode] // Allows this to run in editor mode
    public class LBSGenerated : MonoBehaviour
    {
        #region BUNDLE REFERENCES
        //Original bundle reference
        [SerializeField]
        private Bundle bundleRef;
        public Bundle BundleRef
        {
            get => bundleRef;
            set
            {
                bundleRef = value;
                bundleTemp = value;
            }
        }
        
        //Temporal bundle reference (for when it changes using the WorldEditBar)
        [SerializeField]
        private Bundle bundleTemp;
        public Bundle BundleTemp
        {
            get => bundleTemp;
            set => bundleTemp = value;
        }
        #endregion
        
        public int AssetIndex { get; set; } //Not very accurate, it adjusts when using the switch button in the WorldEditBar

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
            // Check again that the object still exists and doesn’t already have the component
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
        
        public bool HasLBSTag(string tag)
        {
            var characteristics = bundleRef.Characteristics;
            foreach (var lbsChar in characteristics)
            {
                if (lbsChar is not LBSTagsCharacteristic lbstagChar) continue;
                if (lbstagChar.Value == null) continue;
                if (lbstagChar.Value.label != tag) continue;
                return true;
            }

            return false;
        }
    }
}
