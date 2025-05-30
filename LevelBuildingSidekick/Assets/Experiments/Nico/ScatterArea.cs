using ISILab.Extensions;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;

namespace Experiments.Nico
{
    [RequireComponent(typeof(SphereCollider))]
    public class ScatterArea : MonoBehaviour
    {
        public Collider baseCollider;
        public int iterations = 10;
        public float radius = 2.4f;
        public Mesh meshToInstance;
        
        public bool clearPrevious = false;
        
        public void RunCommand()
        {
            print("RunCommand");
            if (baseCollider)
            {
                if (clearPrevious)
                {
                    ClearAllSubMesh();
                }
                
                
                //RaycastCommand rayCommand = new RaycastCommand();
                var resultsBuffer = new NativeArray<RaycastHit>(iterations, Allocator.TempJob);
                var commandsBuffer = new NativeArray<RaycastCommand>(iterations, Allocator.TempJob);

                for (int i = 0; i < iterations; i++)
                {
                    Vector3 origin = baseCollider.bounds.center;
                    Vector3 direction = GenerateRandomDirection() * radius;
                    commandsBuffer[i] = new RaycastCommand(origin, direction, QueryParameters.Default);
                }
                
                
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
          
        }

        static Vector3 GenerateRandomDirection()
        {
            Vector3 direction;
            direction.x = UnityEngine.Random.Range(-1f, 1f);
            direction.y = UnityEngine.Random.Range(-1f, 1f);
            direction.z = UnityEngine.Random.Range(-1f, 1f);
            direction.Normalize();
            return direction;
        }


        void InstanceMeshOnPoint(Vector3 point,  Vector3 normal)
        {
            
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter mf = go.GetComponent<MeshFilter>();
            mf.sharedMesh = meshToInstance;
            
            go.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
            go.transform.position = point;
            go.transform.LookAt(point + normal);
            go.SetParent(this.gameObject);
            //Mesh meshCopy = Instantiate(meshToInstance);
            
        }

        void ClearAllSubMesh()
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
