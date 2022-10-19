using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileConections : ScriptableObject
{
    private GameObject tile;
    private string[] connection = new string[4] { "", "", "", "" };

    public GameObject Tile => tile;

    public string GetConnection(int n)
    {
        return connection[n];
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
