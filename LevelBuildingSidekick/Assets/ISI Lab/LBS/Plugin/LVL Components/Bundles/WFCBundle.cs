using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New exterior bundle", menuName = "ISILab/Bundles/Exterior")]
public class WFCBundle : ScriptableObject // Parche
{
    public LBSTag foward;   // top
    public LBSTag right;    // right
    public LBSTag backward; // bottom
    public LBSTag left;     // left

    [Range(0, 1)]
    public float weight = 1;

    public GameObject Pref;
}