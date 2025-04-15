
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using LBS.Bundles;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.DevTools
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class Custom3dMeshGizmo : MonoBehaviour
    {
        public Color gizmoColor = new Color(1f, 0.67f, 0.06f);
        public Mesh gizmoMesh;
        [Range(0f,1f)]
        public float meshGizmoScale = 0.3f;
        public Vector3 worldPosition = Vector3.zero;
        
        [HideInInspector]
        public Bounds gizmoBounds;

        //Original object references
        [SerializeField]
        private Bundle ogBundleRef;

        public void SetOriginalReference()
        {
            if (ogBundleRef != null)
            {
                Debug.LogWarning("ogBundleRef already set");
                return;
            }

            LBSGenerated lbs = GetComponent<LBSGenerated>();
            //Save object original references
            if (lbs != null && lbs.BundleRef != null)
            {
                ogBundleRef = lbs.BundleRef;
                Debug.Log("ogBundleRef: " + ogBundleRef.name);
            }
            else
            {
                Debug.LogWarning("Custom3dMeshGizmo couldnt set ogBundleRef");
            }
        }

        private void OnDrawGizmosSelected()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr)
            {
                worldPosition = mr.bounds.center;
                gizmoBounds = mr.bounds;
                Gizmos.color = this.gizmoColor;
                Gizmos.DrawWireCube(mr.bounds.center, mr.bounds.size);      
                
                Gizmos.DrawWireMesh(
                    gizmoMesh,
                    mr.bounds.center,
                    Quaternion.identity,
                    new Vector3(meshGizmoScale,meshGizmoScale,meshGizmoScale)
                    );
            }
        }

        public Bundle GetOgBundleRef()
        {
            return ogBundleRef;
        }
    }
    
}
