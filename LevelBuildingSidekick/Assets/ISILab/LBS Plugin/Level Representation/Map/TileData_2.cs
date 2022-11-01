using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData_2 : ICloneable
{
    // Informacion propia
    [SerializeField, JsonRequired]
    private int x;
    [SerializeField, JsonRequired]
    private int y;
    [SerializeField, JsonRequired]
    private int rotation;

    public TileData_2() { }

    public TileData_2(int x, int y, int rotation )//, Action<TileData> onChange)// = null)
    {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }
}

