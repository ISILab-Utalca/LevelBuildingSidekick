using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Parche (!!!), esta clase en vez de tener un objeto "LBSDirection" deberia añadirlo dentro de su lista de characteristic
// pero aun no se puede hacer hasta que se sobreescriba la parte visual y se pueda añadir uno especifico
[CreateAssetMenu(fileName = "New exterior bundle", menuName = "ISILab/Bundles/Exterior")]
public class WFCBundle : Bundle  
{
    [Range(0, 1)]
    public float weight = 1;

    public LBSDirection directions = new LBSDirection(); // Foward, Rigth, Bottom, Left

    public List<weightedTile> Pref = new List<weightedTile>();

    public override void Add(List<Bundle> data)
    {
        throw new System.NotImplementedException();
    }

    public override List<LBSCharacteristic> GetCharacteristics()
    {
        return new List<LBSCharacteristic>(this.characteristics);
    }

    public string[] GetConnection(int rotation = 0)
    {
        var conections = directions.Connections;

        for (int i = 0; i < rotation; i++)
        {
            conections = Rotate(conections.ToArray()).ToList();
        }

        return conections.ToArray();
    }

    public override GameObject GetObject(int index)
    {
        return Pref[index].gameObject;
    }

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        return Pref.Select(p => p.gameObject).ToList();
    }

    public override LBSCharacteristic GetTag(int index)
    {
        return this.characteristics[index];
    }

    public override void Remove(List<Bundle> data)
    {
        throw new System.NotImplementedException();
    }

    private string[] Rotate(string[] c)
    {
        if(c.Count() <= 0)
        {
            Debug.LogWarning("[ISI LAB]: El tile tiene '0' conexiones.");
            return c;
        }

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