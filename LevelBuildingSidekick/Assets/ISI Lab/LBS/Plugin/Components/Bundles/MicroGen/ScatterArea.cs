using UnityEngine;
using Random = UnityEngine.Random;
using System.Diagnostics;


namespace LBS.Bundles.Tools
{
    [RequireComponent(typeof(SphereCollider))]
    public class ScatterArea : ScatterAreaBase
    {
       
        [Header("Scatter Settings")]
        public int iterations = 10;
        private float radius = 2.4f;
        
        
        public float Radius
        {
            get {
                return radius;
            }
            set {
                radius = value;
                if (baseCollider is SphereCollider)
                {
                    SphereCollider collider = baseCollider as SphereCollider;
                    collider.radius = radius;
                }
            }
        }

        public override void RunCommand()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            base.RunCommand();
            if (baseCollider)
            {
                RaycastJobSchedule(iterations, Radius);
            }
            
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Scatter Tool Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
        

        public override Vector3 GenerateDirection()
        {
            Vector3 direction;
            direction.x = Random.Range(-1f, 1f);
            direction.y = Random.Range(-1f, 1f);
            direction.z = Random.Range(-1f, 1f);
            direction.Normalize();
            return direction;
        }
        
        
    }
    
}
