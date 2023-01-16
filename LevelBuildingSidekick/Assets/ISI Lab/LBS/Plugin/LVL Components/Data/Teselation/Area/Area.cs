using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Area
{
    [SerializeField, JsonRequired, SerializeReference]
    Polygon perimeter;

    [JsonIgnore]
    public int PerimeterVertexCount => perimeter.Points.Count;

    public bool AddVertex(Vector2 vertex)
    {
        perimeter.Points.Add(vertex);
        return true;
    }

    public Vector2 GetVertex(int index)
    {
        return perimeter.Points[index];
    }

    public bool Remove(Vector2 vertex)
    {
        return perimeter.Points.Remove(vertex);
    }

    public Vector2 RemoveAt(int index)
    {
        var v = perimeter.Points[index];
        perimeter.Points.Remove(v);
        return v;
    }
}

[System.Serializable]
struct Polygon
{
    [SerializeField, JsonRequired, SerializeReference]
    public List<Vector2> Points;

    public Polygon(List<Vector2> points)
    {
        Points = points;
    }
}