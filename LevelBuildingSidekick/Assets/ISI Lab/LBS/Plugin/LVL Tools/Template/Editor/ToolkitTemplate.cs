using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ISILab/LBS/Toolkit Template")]
public class ToolkitTemplate : ScriptableObject
{
    public List<LBSTool> tools = new List<LBSTool>();
}
