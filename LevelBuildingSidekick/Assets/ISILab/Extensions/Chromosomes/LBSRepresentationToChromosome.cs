using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

public static class LBSRepresentationToChromosome
{
    public static IChromosome ToChromosome(this LBSStampTileMapController controller)
    {
        return new StampTileMapChromosome(controller);
    }
}
