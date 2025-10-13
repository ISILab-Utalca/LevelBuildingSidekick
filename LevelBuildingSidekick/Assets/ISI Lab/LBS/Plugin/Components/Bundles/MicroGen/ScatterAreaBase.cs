using System;
using ISILab.Extensions;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Jobs;
using Unity.Collections;

namespace LBS.Bundles.Tools
{
    public class ScatterAreaBase : MonoBehaviour
    {
        
        public Collider baseCollider;
        
        [Header("Mesh")]
        public Mesh meshToInstance;
        public Material materialToInstance;
        public bool saveGeneratedMesh = false;
        public bool clearPrevious = false;
        
        [Header("MeshTransforms")]
        public Vector2 randomRotationRange = new Vector2(-180, 180);
        [Range(0, 4)]
        public float baseScale = 1;
        [Range(0, 1)]
        public float scaleVariation = 0;
        
        
        public virtual void RunCommand()
        {
            print("RunCommand");

            if (baseCollider && clearPrevious)
            {
                ClearAllSubMesh();
            }
        }
        
        
        public void RaycastJobSchedule(int _iterations , float _maxDistance = float.MaxValue)
        {
            //RaycastCommand rayCommand = new RaycastCommand();
            var resultsBuffer = new NativeArray<RaycastHit>(_iterations, Allocator.TempJob);
            var commandsBuffer = new NativeArray<RaycastCommand>(_iterations, Allocator.TempJob);

            for (int i = 0; i < _iterations; i++)
            {
                Vector3 origin = baseCollider.bounds.center;
                Vector3 direction = (GenerateDirection());
                commandsBuffer[i] = new RaycastCommand(origin, direction, QueryParameters.Default, _maxDistance);
            }
                
            // Setup the job schedule (commadBuffer, resultBuffer, number of hits )
            JobHandle jobSchedule = RaycastCommand.ScheduleBatch(
                commandsBuffer,
                resultsBuffer,
                1,
                1,
                default(JobHandle));
                
            jobSchedule.Complete();
            

            
            foreach (RaycastHit hit in resultsBuffer)
            {
                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);
                    //Debug.Log(hit.point.ToString());
                    InstanceMeshOnPoint(hit.point, hit.normal);
                }
            }
            
            resultsBuffer.Dispose();
            commandsBuffer.Dispose();
                
        }
        
        public virtual Vector3 GenerateDirection()
        {
            return Vector3.zero;
        }
        
        public void InstanceMeshOnPoint(Vector3 point,  Vector3 normal)
        {
            
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();

            if (saveGeneratedMesh)
            {
                Mesh mesh = new Mesh();
                mesh = meshToInstance;
                mesh.vertices = meshToInstance.vertices;
                mesh.triangles = meshToInstance.triangles;
                mesh.normals = meshToInstance.normals;
                mesh.uv = meshToInstance.uv;

                Color[] vColor = meshToInstance.colors;
                float offset = Random.value;
                Color randColor = new Color(offset, offset, offset);
                foreach (var _color in vColor)
                {
                    //_color.r = randColor.r;
                    
                }
                mesh.colors = vColor;
                
                mesh.colors = new Color[meshToInstance.colors.Length];
                mesh.RecalculateBounds();
                
                mf.sharedMesh = mesh;
                mr.sharedMaterial = materialToInstance;
            }
            else
            {
                mf.sharedMesh = meshToInstance;
                mr.sharedMaterial = materialToInstance;
            }
            
            
            
            float instanceRandScale = baseScale * ( 1 + Random.Range(-scaleVariation, scaleVariation));
            go.transform.localScale = new Vector3(instanceRandScale,instanceRandScale,instanceRandScale);
            go.transform.position = point;
            //go.transform.LookAt(point + normal);
            go.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal); // set the y.axis align to normal
            go.transform.Rotate(Vector3.up, Random.Range(randomRotationRange.x, randomRotationRange.y));
            //go.transform.Rotate(90, 0, 0);
            go.SetParent(this.gameObject);
            //Mesh meshCopy = Instantiate(meshToInstance);
            
        }

        public void ClearAllSubMesh()
        {
            List<GameObject> children = GetAllChildren(this.gameObject);
            foreach (GameObject child in children)
            {
                DestroyImmediate(child);
            }
        }
        
        List<GameObject> GetAllChildren(GameObject parent)
        {
            List<GameObject> children = new List<GameObject>();

            // Iterate through all child transforms
            foreach (Transform child in parent.transform)
            {
                children.Add(child.gameObject);

                // Recursively get children of this child
                //children.AddRange(GetAllChildren(child.gameObject));
            }

            return children;
        }
    }
}
