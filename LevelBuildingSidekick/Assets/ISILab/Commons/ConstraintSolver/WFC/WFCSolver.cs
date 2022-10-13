using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System.Linq;

public class WFCSolver
{
    public static WFCTags tags;
    
    public WFCTile[,] Solve(List<WFCTile> samples, WFCTile[,] matrix, Vector2Int[] locked = null)
    {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);
        List<WFCTile>[,] undefinedMatrix = new List<WFCTile>[width, height];

        if(locked == null)
        {
            locked = new Vector2Int[0];
        }

        int prevCounter = 0;
        int counter = 0;

        while(prevCounter != matrix.Length) // Termination should be done with ITermination (!)
        {
            //Can stuck the program in stagnation loop

            do
            {
                prevCounter = counter;
                counter = 0;
                undefinedMatrix = CollapseOnce(samples, matrix, locked);
                foreach (var list in undefinedMatrix)
                {
                    counter += list.Count;
                }
            }
            while (prevCounter != counter);

            foreach(var list in undefinedMatrix)
            {
                //Collapse One Tile -> Should be done with ISelection (!)
                if(list.Count > 1)
                {
                    var sample = list[Random.Range(0, list.Count)];
                    list.Clear();
                    list.Add(sample);
                }
            }
        }

        WFCTile[,] definedMatrix = new WFCTile[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; j++)
            {
                if (undefinedMatrix[i,j].Count == 1)
                {
                    definedMatrix[i, j] = undefinedMatrix[i, j][0];
                }
                else
                {
                    Debug.LogError("Error Somewhere in WFC");
                }
            }
        }

        return definedMatrix;
    }

    public List<WFCTile>[,] CollapseOnce(List<WFCTile> samples, WFCTile[,] matrix, Vector2Int[] locked = null)
    {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);
        List<WFCTile>[,] undefinedMatrix = new List<WFCTile>[width, height];

        for(int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; j++)
            {
                var list = new List<WFCTile>();
                if(locked != null && locked.Any(v => v.x == i && v.y == j))
                {
                    list.Add(matrix[i, j]);
                    undefinedMatrix[i, j] = list;
                    continue;
                }

                Vector2Int[] dir = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
                var neighbors = new List<WFCTile>[4];
                for(int k = 0; k < dir.Length; k++)
                {
                    try
                    {
                        neighbors[i] = undefinedMatrix[i + dir[k].x, j + dir[k].y];
                    }
                    catch
                    {
                        neighbors[i] = null;
                    }
                }

                list = SolveTile(samples, neighbors);

                undefinedMatrix[i, j] = list;
            }
        }

        return undefinedMatrix;
    }

    public List<WFCTile> SolveTile(List<WFCTile> samples, List<WFCTile>[] neighbors)
    {
        List<WFCTile> options = samples;

        for(int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] == null || neighbors[i].Count == 0)
            {
                continue;
            }

            int opposite = (i + (neighbors.Length / 2)) % neighbors.Length;

            WFCTile tile = null;
            foreach (var sample in samples)
            {
                tile = sample;
                if (!(neighbors[i].Any(t => t.tags[i] == sample.tags[opposite])))
                {
                    options.Remove(tile);
                }
            }
        } 

        return options;
    }
}
