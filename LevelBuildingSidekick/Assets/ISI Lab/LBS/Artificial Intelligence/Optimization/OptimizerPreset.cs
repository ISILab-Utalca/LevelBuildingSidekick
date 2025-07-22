using System;
using UnityEngine;

namespace ISILab.LBS.AI.Categorization
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ISILab/LBS/OptimizerPreset")]
    public class OptimizerPreset : ScriptableObject, ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
