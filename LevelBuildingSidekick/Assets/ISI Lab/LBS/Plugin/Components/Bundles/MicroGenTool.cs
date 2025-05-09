using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LBS.Bundles
{
    [Serializable]
    public class MicroGenTool
    {
        [SerializeField]
        private SpreadType _spread;
        public SpreadType Spread
        {
            get => _spread;
            set => _spread = value;
        }

        [SerializeField]
        private Margin _margin;
        public Margin GetMargin
        {
            get => _margin;
        }

        public MicroGenTool()
        {
            _spread = SpreadType.Center;
            _margin = new Margin(1,1,1,1);
        }
    
        public Vector3 MicroPosVector(Vector2 scale, int rotation)
        {
            //rotation 0 = parallels with global x
            //rotation 1 = parallels with global -z
            //rotation 2 = parallels with global -x
            //rotation 3 = parallels with global z
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
            
            _margin = new Margin(1,0.5f,1,1);

            Vector3 vec = Vector3.zero;
            switch (_spread)
            {
                case SpreadType.Side:
                    vec += Quaternion.Euler(0, 90 * (rotation + 1), 0) * Vector3.back * applicableScale;
                    break;
                case SpreadType.Random:
                    break;
            }

            return vec;
        }

        public enum SpreadType
        {
            Center,
            Side,
            Random
        }

        [Serializable]
        public class Margin
        {
            public float negativeX;
            public float positiveX;
            public float negativeZ;
            public float positiveZ;

            public Margin(float negativeX, float positiveX, float negativeZ, float positiveZ)
            {
                this.negativeX = negativeX;
                this.positiveX = positiveX;
                this.negativeZ = negativeZ;
                this.positiveZ = positiveZ;
            }
        }
    }
}
