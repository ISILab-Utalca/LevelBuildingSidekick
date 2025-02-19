#if UNITY_EDITOR  
using UnityEngine;  
using static UnityEngine.Mathf;

namespace ISILab.LBS.Generators
{
    [RequireComponent(typeof(LightProbeGroup))]  
    [RequireComponent(typeof(BoxCollider))]  
    public class LightProbeCubeGenerator : MonoBehaviour  
    {  
        [SerializeField] LightProbeGroup lightProbeComp;  
        [SerializeField] BoxCollider colliderZone;  
        
        public float density = 1; //meters  
        
        void Awake()
        {
            GetFields();
        }

        private void GetFields()
        {
            if (lightProbeComp == null) 
                lightProbeComp = gameObject.GetComponent<LightProbeGroup>() ?? gameObject.AddComponent<LightProbeGroup>();

            if (colliderZone == null) 
                colliderZone = gameObject.GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();
        }

        public void Execute()
        {
            GetFields();
            
            if(lightProbeComp != null){  
                Vector3 _localCenter = gameObject.transform.position - this.transform.position;  
                Vector3 _offset = new Vector3(density,density,density);  
                Vector3 _center  = _localCenter - colliderZone.bounds.size/2 + _offset/2;  
                
                Debug.unityLogger.Log( _center );
                
                int _x =  RoundToInt(colliderZone.bounds.size.x/density);  
                int _y = RoundToInt(colliderZone.bounds.size.y/density);  
                int _z = RoundToInt(colliderZone.bounds.size.z/density);  
                int pointCounts = _x*_y*_z;  
                //Debug.Log("" + pointCounts + "");  
                Vector3[] positions = new Vector3[pointCounts];  
                int _count = 0;   
                for (int i = 0; i < _x; i++){  
                    for (int j = 0; j < _y; j++){  
                        for (int k = 0; k < _z; k++){  
                            positions[_count] = new Vector3(  
                                _center.x + i*density,  
                                _center.y + j*density,  
                                _center.z + k*density);  
                            _count++;                    }                }            }  
                lightProbeComp.probePositions = positions;  
            }    
        }  
    }  

}


#endif