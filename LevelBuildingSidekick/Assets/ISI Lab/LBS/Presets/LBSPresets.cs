using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS
{
    //[CreateAssetMenu(menuName = "LBS/Internal/BundlePresset(*)")]
    public class LBSPresets : ScriptableObject
    {
        public List<bundlePresset> bundlePresets;

        [Serializable]
        public struct bundlePresset
        {
            public string name;
            public ScriptableObject target;
        }
    }
}