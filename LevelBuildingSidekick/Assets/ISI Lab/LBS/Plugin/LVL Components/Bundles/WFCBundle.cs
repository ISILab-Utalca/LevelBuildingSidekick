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

    public string[] GetConnection(int rotation = 0)
    {
        var conections = new List<string>() {
            right.value,
            backward.value,
            left.value,
            foward.value
        };

        return conections.ToArray();
    }
}