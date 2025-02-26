
namespace ISI_Lab.DevTools.Gizmos
{
    using System;
    using UnityEngine;
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class Custom3dMeshGizmo : MonoBehaviour
    {
        public Color gizmoColor = new Color(1f, 0.67f, 0.06f);
        public Mesh gizmoMesh;
        [Range(0f,1f)]
        public float meshGizmoScale = 0.3f;
        
        [HideInInspector]
        public Bounds gizmoBounds;
    
    
        private void OnDrawGizmosSelected()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr)
            {
                
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
    }
    
}
