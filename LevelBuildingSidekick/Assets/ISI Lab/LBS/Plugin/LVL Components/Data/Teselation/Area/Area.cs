using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Area : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired, SerializeReference]
    Polygon perimeter;

    [SerializeField, JsonRequired, SerializeReference]
    string id;

    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public int PerimeterVertexCount => perimeter.Points.Count;

    [JsonIgnore]
    public string ID => id;

    [JsonIgnore]
    public Vector2 Position
    {
        get
        {
            var x = perimeter.Points.Min(p => p.x);
            var y = perimeter.Points.Min(p => p.y);
            return new Vector2(x, y);
        }
    }

    [JsonIgnore]
    public Rect Rect
    {
        get
        {
            var pos = Position;
            var width = perimeter.Points.Max(p => p.x) - pos.x;
            var height = perimeter.Points.Max(p => p.y) - pos.y;

            return new Rect(Position, new Vector2(width, height));
        }
    }

    [JsonIgnore]
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

    #endregion

    #region COSNTRUCTOR

    public Area()
    {
        perimeter = new Polygon(new List<Vector2>());
    }

    public Area(string id, List<Vector2> vertexes)
    {
        this.id = id;
        perimeter = new Polygon(vertexes);
    }

    #endregion

    #region METHODS

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

    public virtual bool ContainsPoint(Vector2 point)
    {
        return perimeter.IsInside(point);
    }

    internal void Clear()
    {
        perimeter.Points.Clear();
    }

    public bool Collides(Area other)//Implementar
    {
        //Agregar Collision de Rects primero, Bounding Boxes (!)

        for(int i = 0; i < other.PerimeterVertexCount; i++)
        {
            if(ContainsPoint(other.GetVertex(i)))
            {
                return true;
            }
        }

        return false;
    }

    public object Clone()
    {
        var p = perimeter.Clone() as Polygon;
        return new Area(this.id, p.Points);
    }

    #endregion
}

[System.Serializable]
public class Polygon : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired, SerializeReference]
    public List<Vector2> Points;

    #endregion

    #region CONSTRUCTOR

    public Polygon(List<Vector2> points)
    {
        Points = points;
    }

    #endregion

    #region METHODS

    public bool IsInside(Vector2 point)
    {
        if (Points.Count < 3)
            return false;

        var externalPoint = new Vector2(int.MaxValue, point.y);

        int count = 0;
        int i = 0;
        do
        {

            // Forming a line from two consecutive points of
            // poly
            if (point.LinesIntersect(externalPoint, Points[i], Points[(i + 1) % Points.Count]))
            {

                // If side is intersects exline
                if ( point.IntersectionOrientation(Points[i], Points[(i + 1) % Points.Count]) == Orientation.COLINEAR)
                    return point.IsInLine(Points[i], Points[(i + 1) % Points.Count]);
                count++;
            }
            i = (i + 1) % Points.Count;
        } while (i != 0);

        // When count is odd
        return count % 2 != 0;
    }

    public object Clone()
    {
        return new Polygon(Points.Select(p => new Vector2(p.x,p.y)).ToList());
    }

    #endregion
}