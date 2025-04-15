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
        [SerializeField]
        private Bundle bundleRef;

        public Bundle BundleRef
        {
            get => bundleRef;
            set => bundleRef = value;
        }

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
                var gizmo = Undo.AddComponent<Custom3dMeshGizmo>(gameObject); // Supports undo in editor
            }
        }
#endif

        private void EnsureGizmoComponent()
        {
            if (!GetComponent<Custom3dMeshGizmo>())
            {
                var gizmo = gameObject.AddComponent<Custom3dMeshGizmo>();
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
