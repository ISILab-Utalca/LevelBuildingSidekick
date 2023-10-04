using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
