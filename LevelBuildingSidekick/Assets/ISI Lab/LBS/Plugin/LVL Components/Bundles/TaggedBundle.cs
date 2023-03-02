using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/PopulationBundle")]
[System.Serializable]
public class TaggedBundle : SimpleBundle
{
    public LBSCharacteristic idTag;
}
