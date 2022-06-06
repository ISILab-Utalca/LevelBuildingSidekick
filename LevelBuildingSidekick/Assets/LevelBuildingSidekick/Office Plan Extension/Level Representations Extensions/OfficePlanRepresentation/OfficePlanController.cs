using System;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;

public class OfficePlanController : LevelRepresentationController
{
    public GraphController Graph { get; set; }
    public BlueprintController Blueprint { get; set; }
    public OfficePlanController(Data data) : base(data)
    {
        View = new OfficePlanView(this);
    }

    public void Translate()
    {
        bool[,] adjacency = Graph.AdjacencyMatrix();
        int[,] matrix = Graph.ToMatrix(out Vector2Int[] indexes);
        List<Vector2Int>[] pulls = new List<Vector2Int>[indexes.Length];

        //Cycle with end condition
        //Apply Pulls
    }

    //TODO
    public void Untangle(bool[,] adjacency, int[,] matrix, Vector2Int[] indexes)
    {
        List<Vector2Int> crossedNodes = new List<Vector2Int>();
        
    }

    public Tuple<Vector2Int,Vector2Int>[] GetPulls(bool[,] adjacency,  Vector2Int[] positions)
    {
        var pulls = new Tuple<Vector2Int, Vector2Int>[positions.Length];

        //TODO check adjacency is square;

        for(int i = 0; i < positions.Length; i++)
        {
            Vector2Int xPull = Vector2Int.zero;
            Vector2Int yPull = Vector2Int.zero;
            for(int j = 0; j < positions.Length; j++)
            {
                if(adjacency[i,j])
                {
                    var pull = positions[i] - positions[j]/2;

                    if(pull.x < 0)
                    {
                        xPull.x += pull.x; 
                    }
                    else
                    {
                        xPull.y += pull.x;
                    }

                    if (pull.y < 0)
                    {
                        yPull.x += pull.y;
                    }
                    else
                    {
                        yPull.y += pull.y;
                    }
                }
            }
            pulls[i] = new Tuple<Vector2Int, Vector2Int>(xPull,yPull);
        }

        return pulls;
    }
}
