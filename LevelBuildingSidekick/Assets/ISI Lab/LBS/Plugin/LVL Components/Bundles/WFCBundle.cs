using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New exterior bundle", menuName = "ISILab/Bundles/Exterior")]
public class WFCBundle : ScriptableObject // Parche
{
    [Range(0, 1)]
    public float weight = 1;

    public LBSDirection directions = new LBSDirection();

    public List<weightedTile> Pref;

    public string[] GetConnection(int rotation = 0)
    {
        var conections = directions.Connections;

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

    [System.Serializable]
    public struct weightedTile
    {
        [Range(0, 1)]
        public float weight;
        public GameObject gameObject;

        public weightedTile(GameObject gameObject, float weight = 1)
        {
            this.weight = weight;
            this.gameObject = gameObject;
        }
    }
}