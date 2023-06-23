using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileConnectWFC
{
    /*
    private TileConections tile;
    private int rotation; // max 3 (*90)

    public TileConections Tile => tile;
    public int Rotation => rotation; // el 90 puede ser parametrizado o calculado con el 4

    public TileConnectWFC(TileConections tile, int rotation)
    {
        this.tile = tile;
        this.rotation = rotation;
    }

    public string GetConnection(int n)
    {
        return tile.GetConnection((n + rotation) % 4); // el 4 puede ser un parametro (??)
    }

    public string Print(int i)
    {
        var s0 = GetConnection(0)[0] == 'p' ? 'O' : 'X';
        var s1 = GetConnection(1)[0] == 'p' ? 'O' : 'X';
        var s2 = GetConnection(2)[0] == 'p' ? 'O' : 'X';
        var s3 = GetConnection(3)[0] == 'p' ? 'O' : 'X';
        var c = rotation;//(new List<char> { s0, s1, s2, s3 }).Contains('O') ? 'O' : 'X';
        if (i == 0)
        {
            return ("X " + s3 + " X");
        } 
        else if (i == 1)
        {
            return (s2 + " " + c + " " + s0);
        }
        else if (i == 2)
        {
            return ("X " + s1 + " X");
        }
        return "";
    }
    */
}

public class WFCSolver
{
    /*
    public static WFCTags tags;

    // samples son las piezas que se puden utilizar
    // tilw conection ?
    // locked ? (sobra)
    public TileConnectWFC[,] Solve(List<TileConections> samples, TileConnectWFC[,] matrix, Vector2Int[] locked = null)
    {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);
        List<TileConnectWFC>[,] undefinedMatrix = InitUndefMatrix(samples, matrix);

        if (locked == null)
        {
            locked = new Vector2Int[0];
        }

        while (!IsDefined(undefinedMatrix))
        {
            undefinedMatrix = Collapse(undefinedMatrix);
        }

        var definedMatrix = new TileConnectWFC[width,height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
            
                if (undefinedMatrix[i, j].Count == 1)
                {
                    definedMatrix[i, j] = undefinedMatrix[i, j][0];
                }
                else
                {
                    definedMatrix[i, j] = null;
                }
            }
        }

        return definedMatrix;
    }

    private bool IsDefined(List<TileConnectWFC>[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if(matrix[i,j].Count > 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private List<TileConnectWFC>[,] InitUndefMatrix(List<TileConections> samples, TileConnectWFC[,] matrix)
    {
        var prev = new List<TileConnectWFC>[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < prev.GetLength(0); i++)
        {
            for (int j = 0; j < prev.GetLength(1); j++)
            {
                if (matrix[i,j] != null)
                {
                    prev[i, j] = new List<TileConnectWFC>() { matrix[i, j] };
                }
                else
                {
                    var sampleWFC = GenerateRotateSamples(samples, 4); // el 4 podria estar parametrizado (?)
                    prev[i, j] = new List<TileConnectWFC>(sampleWFC);
                }
            }
        }
        return prev;
    }

    private List<TileConnectWFC> GenerateRotateSamples(List<TileConections> sample, int maxRotation)
    {
        var toReturn = new List<TileConnectWFC>();
        for (int i = 0; i < maxRotation; i++)
        {
            for (int j = 0; j < sample.Count; j++)
            {
                toReturn.Add(new TileConnectWFC(sample[j], i));
            }
        }
        return toReturn;
    }

    private List<TileConnectWFC>[,] Collapse(List<TileConnectWFC>[,] undefMatrix)
    {
        var toReturn = undefMatrix;
        var preValue = 0;
        var posValue = 0;
        var scape = 0;
        do 
        {
            preValue = Posibilities(toReturn);
            for (int i = 0; i < toReturn.GetLength(0); i++)
            {
                for (int j = 0; j < toReturn.GetLength(1); j++)
                {
                    var odds = new List<TileConnectWFC>(undefMatrix[i, j]);
                    var neighbors = GetNeighbors(i, j, undefMatrix);

                    toReturn[i, j] = GetCompatibles(odds, neighbors);
                }
            }
            posValue = Posibilities(toReturn);
            scape++;
        } while (preValue > posValue && scape < 1000);

        toReturn = CollapseOnce(toReturn);
        return toReturn;
    }

    private int Posibilities(List<TileConnectWFC>[,] matrix)
    {
        var value = 0;
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                value += matrix[i, j].Count;
            }
        }
        return value;
    }

    // colapsa uno aleatorio
    private List<TileConnectWFC>[,] CollapseOnce(List<TileConnectWFC>[,] undefMatrix) // ref (??)
    {
        if (IsDefined(undefMatrix))
            return undefMatrix;

        var toDef = new List<Vector2Int>();
        var v = int.MaxValue;
        for (int i = 0; i < undefMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < undefMatrix.GetLength(1); j++)
            {
                if (undefMatrix[i, j].Count() <= 1)
                {
                    continue;
                }

                if (undefMatrix[i, j].Count() < v)
                {
                    toDef.Clear();
                    toDef.Add(new Vector2Int(i, j));
                    v = undefMatrix[i, j].Count();
                }
                else if(undefMatrix[i, j].Count() == v)
                {
                    toDef.Add(new Vector2Int(i, j));
                }
            }
        }

        //Implement weight (???)
        var pos = toDef[Random.Range(0, toDef.Count())];
        var undefTile = undefMatrix[pos.x, pos.y];
        undefMatrix[pos.x, pos.y] = new List<TileConnectWFC>() {FortuneWheel(undefTile)};
        return undefMatrix;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private TileConnectWFC FortuneWheel(List<TileConnectWFC> list)
    {
        
        float sumWeight = 0;
        
        for (int i = 0; i < list.Count(); i++){ sumWeight += list[i].Tile.weight; }

        var r = Random.Range(0, (int)sumWeight);
        var pivot = 0;

        for (int i = 0; i < list.Count(); i++)
        {
            if (r >= pivot && r <= pivot + list[i].Tile.weight)
            {
                return list[i];
            }
            pivot += (int)list[i].Tile.weight;
        }
        return null;
    } 


    private List<TileConnectWFC> GetCompatibles(List<TileConnectWFC> odds, List<TileConnectWFC>[] neighbors)
    {
        var toReturn = new List<TileConnectWFC>(odds);
        int dir = 0;
        var temp = new List<TileConnectWFC>[neighbors.Length];
        foreach (var neigh in neighbors)
        {
            temp[dir] = new List<TileConnectWFC>();
            if (neigh == null || neigh.Count <= 0)
            {
                temp[dir] = new List<TileConnectWFC>(odds);
                dir++;
                continue;
            }

            var otherDir = (int)(dir + (neighbors.Count() / 2f)) % neighbors.Count();
            temp[dir] = new List<TileConnectWFC>(odds).Where(o => CheckCompatible(o, dir, neigh, otherDir)).ToList();
            dir++;
        }

        for (int i = 0; i < temp.Count(); i++)
        {
            toReturn = toReturn.Intersect(temp[i]).ToList();
        }
        return toReturn;
    }

    private bool CheckCompatible(TileConnectWFC tile, int tileDir, List<TileConnectWFC> other, int otherDir)
    {
        var t = tile.GetConnection(tileDir);
        var o = new List<string>();
        foreach (var os in other)
        {
            var s = os.GetConnection(otherDir);
            if(s == t)
            {
                return true;
            }
        }
        return false;
    }

    private List<TileConnectWFC>[] GetNeighbors(int x,int y, List<TileConnectWFC>[,] matrix)
    {
        Vector2Int[] dir = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
        var neighbors = new List<TileConnectWFC>[dir.Length];
        for (int i = 0; i < dir.Length; i++)
        {
            var posX = x + dir[i].x;
            var posY = y + dir[i].y;
            if (posX >= matrix.GetLength(0) || posX < 0 || posY >= matrix.GetLength(1) || posY < 0)
            {
                neighbors[i] = null;
                continue;
            }

            neighbors[i] = new List<TileConnectWFC>(matrix[posX, posY]);
        }
        return neighbors;
    }
    */
}
