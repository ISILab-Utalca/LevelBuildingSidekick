using System;
using System.Collections;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace LBS.Bundles.Tools
{
    [Serializable]
    public class MicroGenTool
    {
        #region ADITIONAL STRUCTURES
        public enum SpreadType
        {
            Center,
            Side,
            WallSnap,
            Random,
            CenterFloating,
            Scatter
        }

        [Serializable]
        public class Margin
        {
            public float negativeX;
            public float positiveX;
            public float negativeZ;
            public float positiveZ;

            public Margin(float _negativeX, float _positiveX, float _negativeZ, float _positiveZ)
            {
                negativeX = _negativeX;
                positiveX = _positiveX;
                negativeZ = _negativeZ;
                positiveZ = _positiveZ;
            }

            public Vector2 GetPointInside()
            {
                float x = Random.Range(-negativeX, positiveX);
                float y = Random.Range(-negativeZ, positiveZ);
                
                return new Vector2(x, y);
            }
        }
        
        // Helper MonoBehaviour
        public class MonoHelper : MonoBehaviour { }
        #endregion

        #region FIELDS
        [SerializeField]
        private SpreadType spread = SpreadType.Center;
        
        [SerializeField]
        private Margin margin = new(1,1,1,1);
        
        [SerializeField]
        private Mesh mesh = null;
        
        [SerializeField]
        private Material material = null;
        
        #endregion

        #region PROPERTIES
        public SpreadType Spread
        {
            get => spread;
            set => spread = value;
        }
        public Margin GetMargin
        {
            get => margin;
        }
        #endregion

        public Vector3 MicroPosVector(Transform objTransform, Vector2 scale, int rotation)
        {
            //Calculate rotation
            float applicableScale;
            switch (rotation)
            {
                case 0: case 2: applicableScale = scale.x;
                    break;
                case 1: case 3: applicableScale = scale.y;
                    break;
                default: applicableScale = 1;
                    break;
            }
            Quaternion applicableRotation = Quaternion.Euler(0, 90 * (rotation + 1), 0);
            
            //Calculate displacement
            Vector3 vec = Vector3.zero;
            switch (spread)
            {
                case SpreadType.Side:
                    vec =  applicableRotation * Vector3.back * applicableScale * GetMarginSideByRotation(rotation, true);
                    break;
                
                case SpreadType.Random:
                    Vector2 p = margin.GetPointInside();
                    vec = new Vector3(p.x, 0, p.y);
                    break;
                
                case SpreadType.WallSnap:
                    if (Physics.Raycast(objTransform.position, -objTransform.forward, out var hit, applicableScale))
                    {
                        vec = -objTransform.forward * hit.distance;
                    }
                    break;
                
                case SpreadType.CenterFloating:
                    vec = Vector3.up;
                    break;
                
                case SpreadType.Scatter:
                    if (mesh == null || material == null)
                    {
                        Debug.LogError("MicroGenTool: Mesh or Material is missing");
                        break;
                    }
                    
                    var scatter = objTransform.GetComponent<ScatterArea>() ?? objTransform.gameObject.AddComponent<ScatterArea>();
                    scatter.baseCollider = objTransform.GetComponent<Collider>() ?? objTransform.gameObject.AddComponent<Collider>();
                    scatter.meshToInstance = mesh;
                    scatter.materialToInstance = material;

                    var helper = objTransform.gameObject.AddComponent<MonoHelper>();
                    helper.StartCoroutine(StartScatter(scatter, helper));
                    break;
            }

            return vec;
        }

        float GetMarginSideByRotation(int rotation, bool invert = false)
        {
            //rotation 0 = parallels with global x
            //rotation 1 = parallels with global -z
            //rotation 2 = parallels with global -x
            //rotation 3 = parallels with global z
            
            if (!invert)
            {
                switch (rotation)
                {
                    case 0: return margin.positiveX;
                    case 1: return margin.negativeZ;
                    case 2: return margin.negativeX;
                    case 3: return margin.positiveZ;
                    default: return 0;
                }
            }

            switch (rotation)
            {
                case 0: return margin.negativeX;
                case 1: return margin.positiveZ;
                case 2: return margin.positiveX;
                case 3: return margin.negativeZ;
                
                default: return 0;
            }
        }

        private IEnumerator StartScatter(ScatterArea scatter, MonoHelper helper)
        {
            yield return new WaitForSeconds(0);
            scatter.RunCommand();
            helper.StopAllCoroutines();
        }

    }
}
