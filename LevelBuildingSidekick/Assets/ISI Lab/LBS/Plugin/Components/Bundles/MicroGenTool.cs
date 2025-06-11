using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace LBS.Bundles
{
    [Serializable]
    public class MicroGenTool
    {
        public enum SpreadType
        {
            Center,
            Side,
            WallSnap,
            Random,
            CenterFloating
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
                this.negativeX = _negativeX;
                this.positiveX = _positiveX;
                this.negativeZ = _negativeZ;
                this.positiveZ = _positiveZ;
            }

            public Vector2 GetPointInside()
            {
                float x = Random.Range(-negativeX, positiveX);
                float y = Random.Range(-negativeZ, positiveZ);
                
                return new Vector2(x, y);
            }
        }
        
        [SerializeField]
        private SpreadType spread;
        public SpreadType Spread
        {
            get => spread;
            set => spread = value;
        }

        [SerializeField]
        private Margin margin;
        public Margin GetMargin
        {
            get => margin;
        }

        public MicroGenTool()
        {
            spread = SpreadType.Center;
            margin = new Margin(1,1,1,1);
        }
    
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
                    vec =  applicableRotation * Vector3.back * applicableScale * GetMarginSideByIndex(rotation, true);
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
                case  SpreadType.CenterFloating:
                    vec = Vector3.up;
                    break;
                
            }

            return vec;
        }

        float GetMarginSideByIndex(int index, bool invert = false)
        {
            //rotation 0 = parallels with global x
            //rotation 1 = parallels with global -z
            //rotation 2 = parallels with global -x
            //rotation 3 = parallels with global z
            
            if (!invert)
            {
                switch (index)
                {
                    case 0: return margin.positiveX;
                    case 1: return margin.negativeZ;
                    case 2: return margin.negativeX;
                    case 3: return margin.positiveZ;
                    default: return 0;
                }
            }

            switch (index)
            {
                case 0: return margin.negativeX;
                case 1: return margin.positiveZ;
                case 2: return margin.positiveX;
                case 3: return margin.negativeZ;
                
                default: return 0;
            }
        }

    }
}
