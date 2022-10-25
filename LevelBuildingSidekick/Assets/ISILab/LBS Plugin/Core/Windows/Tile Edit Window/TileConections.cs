using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileConections : ScriptableObject
{
    [SerializeField,HideInInspector]
    private GameObject tile;
    [SerializeField,HideInInspector]
    private string[] connection = new string[4] { "", "", "", "" };

    public GameObject Tile => tile;

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
}
