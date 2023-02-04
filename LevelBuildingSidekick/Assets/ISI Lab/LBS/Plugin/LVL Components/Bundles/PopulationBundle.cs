using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New bundle", menuName = "ISILab/PopulationBundle")]
[System.Serializable]
public class PopulationBundle : SimpleBundle
{
    [JsonRequired, SerializeField]
    private Texture2D icon;
    [JsonRequired, SerializeField]
    private string label = ""; // "ID" or "name" 
    [JsonIgnore]
    public string Label => label;

}
