using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New exterior bundle", menuName = "ISILab/Bundles/Exterior")]
public class WFCBundle : ScriptableObject // Parche
{
    public LBSCharacteristic foward;   // top
    public LBSCharacteristic right;    // right
    public LBSCharacteristic backward; // bottom
    public LBSCharacteristic left;     // left

    [Range(0, 1)]
    public float weight = 1;

    public GameObject Pref;

    public string[] GetConnection(int rotation = 0)
    {
        var conections = new List<string>() {
            right.Label,
            foward.Label,
            left.Label,
            backward.Label
        };

        for (int i = 0; i < rotation; i++)
        {
            conections = Rotate(conections.ToArray()).ToList();
        }

        return conections.ToArray();
    }

    private string[] Rotate(string[] c)
    {
        var temp = c.ToList();
        var last = c.Last();
        temp.RemoveAt(temp.Count - 1);
        var r = new List<string>() { last };
        r.AddRange(temp);

        var toR = new string[c.Length];
        for (int i = 0; i < c.Length; i++)
        {
            toR[i] = r[i];
        }

        return toR;
    }
}