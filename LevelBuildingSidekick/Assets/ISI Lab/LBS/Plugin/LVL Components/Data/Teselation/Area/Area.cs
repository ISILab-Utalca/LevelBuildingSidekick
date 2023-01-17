using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Area
{
    //FIELDS
    [SerializeField, JsonRequired, SerializeReference]
    Polygon perimeter;

    [SerializeField, JsonRequired, SerializeReference]
    string id;

    //PROPERTIES
    [JsonIgnore]
    public int PerimeterVertexCount => perimeter.Points.Count;

    [JsonIgnore]
    public string ID => id;

    public Vector2 Centroid
    {
        get
        {
            var x = perimeter.Points.Sum(p => p.x) / perimeter.Points.Count;
            var y = perimeter.Points.Sum(p => p.y) / perimeter.Points.Count;
            return new Vector2(x,y);
        }
        set
        {
            var x = perimeter.Points.Min(p => p.x);
            var y = perimeter.Points.Min(p => p.y);

            var delta = new Vector2(value.x - x, value.y - y);

            for(int i = 0; i < PerimeterVertexCount; i++)
            {
                perimeter.Points[i] += delta;
            }
        }
    }

    //COSNTRUCTOR
    public Area()
    {
        perimeter = new Polygon(new List<Vector2>());
    }

    public Area(string id, List<Vector2> vertexes)
    {
        this.id = id;
        perimeter = new Polygon(vertexes);
    }

    //METHODS
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

    internal void Clear()
    {
        perimeter.Points.Clear();
    }

    public bool Collides(Area other)//Implementar
    {
        return false;
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