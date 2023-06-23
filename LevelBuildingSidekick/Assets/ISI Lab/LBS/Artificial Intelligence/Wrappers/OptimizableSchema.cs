using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizableSchema : IOptimizable
{
    private LBSSchema schema;

    private double fitness;

    public LBSSchema Schema => schema;

    public OptimizableSchema(LBSSchema schema)
    {
        this.schema = schema;
    }

    public double Fitness 
    { 
        get => fitness; 
        set => fitness = value; 
    }

    public object Clone()
    {
        return new OptimizableSchema(schema.Clone() as LBSSchema);
    }

    public IOptimizable CreateNew()
    {
        return Clone() as OptimizableSchema;
    }
}
