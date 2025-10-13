using System;
using ISILab.Extensions;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Random = UnityEngine.Random;

namespace LBS.Bundles.Tools
{
    [RequireComponent(typeof(SphereCollider))]
    public class ScatterArea : ScatterAreaBase
    {
       
        [Header("Scatter Settings")]
        public int iterations = 10;
        public float radius = 2.4f;

        
        
        public override void RunCommand()
        {
            base.RunCommand();
            if (baseCollider)
            {
                RaycastJobSchedule(iterations, radius);
            }
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
