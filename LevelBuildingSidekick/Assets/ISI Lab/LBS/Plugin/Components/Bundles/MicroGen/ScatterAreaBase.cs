using System;
using ISILab.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Assertions;
using UnityEngine.Rendering;


namespace LBS.Bundles.Tools
{
    [ExecuteAlways]
    public class ScatterAreaBase : MonoBehaviour
    {
        public enum GenerationMode{Instances, SingleCachedMesh, GpuBach}
        
        public GenerationMode generationMode = GenerationMode.Instances;
        public Collider baseCollider;
        
        [Header("Mesh")]
        public Mesh meshToInstance;
        public Material materialToInstance;
        public bool clearPrevious = false;
        
        [Header("MeshTransforms")]
        public Vector2 randomRotationRange = new Vector2(-180, 180);
        [Range(0, 4)]
        public float baseScale = 1;
        [Range(0, 1)]
        public float scaleVariation = 0;
        
        
        // hold transforms matrix
        NativeArray<Vector3> positions;
        NativeArray<Quaternion> rotations;
        NativeArray<Vector3> scales;
        
        
        NativeArray<Matrix4x4> matrices;
        private RenderParams renderParams = new RenderParams();


        public virtual void RunCommand()
        {
            print("RunCommand");

            if (baseCollider && clearPrevious)
            {
                ClearAllSubMesh();
            }
        }
        
        
        public void Update()
        {
            if (generationMode == GenerationMode.GpuBach && matrices.Length > 0)
            {
                //Graphics.RenderMeshInstanced(renderParams, meshToInstance, 0, matrices );
                Graphics.DrawMeshInstanced(meshToInstance, 0, materialToInstance, matrices.ToArray());
            }
        }

        public void RaycastJobSchedule(int _iterations , float _maxDistance = float.MaxValue)
        {
            //RaycastCommand rayCommand = new RaycastCommand();
            NativeArray<RaycastHit> resultsBuffer = new NativeArray<RaycastHit>(_iterations, Allocator.TempJob);
            NativeArray<RaycastCommand> commandsBuffer = new NativeArray<RaycastCommand>(_iterations, Allocator.TempJob);

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

            if (resultsBuffer.Length <= 0)
            {
                resultsBuffer.Dispose();
                commandsBuffer.Dispose();
                return;
            }
            
            int meshCount = 0 ;
            List<Vector3> points =  new List<Vector3>();
            List<Vector3> normals =  new List<Vector3>();
            foreach (RaycastHit hit in resultsBuffer)
            {
                if (hit.collider != null)
                {
                    meshCount++;
                    points.Add(hit.point);
                    normals.Add(hit.normal);
                }
            }
            
            
            switch (generationMode)
            {
                case GenerationMode.Instances:
                {
                    TryDisposeMatrix();
                    for (int i = 0; i < points.Count; i++)
                    {
                        InstanceMeshOnPoint(points[i], normals[i]);
                    }
                    break;
                }
                case GenerationMode.SingleCachedMesh:
                {
                    resultsBuffer.Dispose();
                    commandsBuffer.Dispose();
                    TryDisposeMatrix();
                    throw new NotImplementedException();
                    break;
                }
                case GenerationMode.GpuBach:
                {
                    BatchMesh(meshCount, points, normals);
                    break;
                }
                default:
                    break;
            }
            
            //Debug.Log($"meshCount: {meshCount}");
            resultsBuffer.Dispose();
            commandsBuffer.Dispose();
                
        }

        private void TryDisposeMatrix()
        {
            if (matrices.IsCreated)
            {
                matrices.Dispose();
            }
        }

        public virtual Vector3 GenerateDirection()
        {
            return Vector3.down;
        }
        
        public void InstanceMeshOnPoint(Vector3 point,  Vector3 normal)
        {
            
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            
            mf.sharedMesh = meshToInstance;
            mr.sharedMaterial = materialToInstance;
            
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
        
        public void BatchMesh(int _meshCount , List<Vector3> _points, List<Vector3> _normals)
        {
            positions = NatArrayFromList<Vector3>(_points);
            NativeArray<Vector3> normals =  NatArrayFromList<Vector3>(_normals);
            rotations = new NativeArray<Quaternion>(_meshCount, Allocator.TempJob);
            scales = new NativeArray<Vector3>(_meshCount, Allocator.TempJob);
            
            TryDisposeMatrix();
            matrices = new NativeArray<Matrix4x4>(_meshCount, Allocator.Persistent);
            
            
            MeshInstanceJob job = new MeshInstanceJob(positions, rotations, scales, matrices, normals);
            
            JobHandle handle = job.Schedule(positions.Length, 64, default(JobHandle));
            
            handle.Complete();
            
            matrices = job.Matrices;
            Matrix4x4[] matrix =  matrices.ToArray();
            
            renderParams = new RenderParams
            {
                material = materialToInstance,
                shadowCastingMode = ShadowCastingMode.On,
                receiveShadows = true,
                lightProbeUsage = LightProbeUsage.Off
            };
            
            
            //Graphics.RenderMeshInstanced(renderParams, meshToInstance, 0, matrices);
            Graphics.DrawMeshInstanced(meshToInstance, 0, materialToInstance, matrix);
            
            
            positions.Dispose();
            rotations.Dispose();
            scales.Dispose();
            normals.Dispose();
            //matrices.Dispose();
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

        void OnDestroy()
        {
            if (matrices.IsCreated)
            {
                matrices.Dispose();
            }
        }

        static NativeArray<T> NatArrayFromList<T>(List<T> input) where T: struct
        {
            if (input.Count <= 0)
            {
                throw new InvalidOperationException("List is empty");
            }
            NativeArray<T> natArray = new NativeArray<T>(input.Count, Allocator.TempJob);
            for (int i = 0; i < input.Count; i++)
            {
                natArray[i] = input[i];
            }
            return natArray;
        }
        
        
        
    }
    
    [BurstCompile]
    public struct MeshInstanceJob : IJobParallelFor
    {
        
        [ReadOnly] public NativeArray<Vector3> Positions;
        
        public NativeArray<Quaternion> Rotations;
        
        public NativeArray<Vector3> Scales;
        
        public NativeArray<Matrix4x4> Matrices;
        
        [ReadOnly] public NativeArray<Vector3> Normals;

        public MeshInstanceJob(
            NativeArray<Vector3> _positions,
            NativeArray<Quaternion> _rotations,
            NativeArray<Vector3> _scales,
            NativeArray<Matrix4x4> _matrices,
            NativeArray<Vector3> _normals)
        {
            Positions = _positions;
            Rotations = _rotations;
            Scales = _scales;
            Normals = _normals;
            Matrices = _matrices;
        }

        public void Execute(int _index)
        {
            Rotations[_index] =  Quaternion.FromToRotation(Vector3.up, Normals[_index]);
            Scales[_index] = Vector3.one;
            Matrices[_index] = Matrix4x4.TRS(Positions[_index], Rotations[_index], Scales[_index]);
        }
        
    }
    
    [BurstCompile]
    public struct MyInstanceData
    {
        Matrix4x4 objectToWorld; // We must specify object-to-world transformation for each instance
        uint renderingLayerMask; // In addition we also like to specify rendering layer mask per instance.
    };
    
}
