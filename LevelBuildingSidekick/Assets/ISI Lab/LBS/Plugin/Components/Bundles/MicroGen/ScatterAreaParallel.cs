using UnityEngine;



namespace LBS.Bundles.Tools
{
    
    [RequireComponent(typeof(BoxCollider))]
    public class ScatterAreaParallel: ScatterAreaBase
    {
        
        [Header("Scatter Settings")]
        public int iterations = 10;
        public float  deep = 2.4f;

        public override void RunCommand()
        {
            base.RunCommand();
            
        }


        public Vector3 GenerateParallelDirection()
        {
            Vector3 direction;
            direction.x = Random.Range(-1f, 1f);
            direction.y = GetComponent<GameObject>().transform.position.y;
            direction.z = Random.Range(-1f, 1f);
            direction.Normalize();
            return direction;
        }

    }
    
    

    
    
}

