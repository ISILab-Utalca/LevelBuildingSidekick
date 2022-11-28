using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileConections : ScriptableObject
{
    [SerializeField,HideInInspector]
    private GameObject tile; // (!) esto podria ser object bunddle
    [SerializeField,HideInInspector]
    private string[] connection = new string[4] { "", "", "", "" };
    [SerializeField, HideInInspector]
    public float weight = 50; // (?) de 0 a 100 o 0 a 1 ?

    public GameObject Tile => tile;
    public string[] Connections => connection;

    public string GetConnection(int n)
    {
        return connection[n]; // rigth,bot,left,top
    }

    public void SetConnection(int n, string value)
    {
        connection[n] = value;
    }

    public void Init(GameObject tile)
    {
        this.tile = tile;
    }

    public override bool Equals(object other)
    {
        var o = (TileConections)other;

        if (o == null)
            return false;

        for (int i = 0; i < connection.Length; i++)
        {
            if (connection[i] != o.connection[i])
                return false;
        }
        return true;

    }
}
