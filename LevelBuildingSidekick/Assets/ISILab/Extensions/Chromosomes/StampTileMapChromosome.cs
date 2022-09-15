using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;

public class StampTileMapChromosome : ChromosomeBase2D<int>
{
    private List<StampData> stamps;

    public StampTileMapChromosome(LBSStampTileMapController stampController) : base(0, 0)
    {
        var rawStamps = (stampController.GetData() as LBSStampGroupData).GetStamps();

        var x1 = rawStamps.Min(s => s.Position.x);
        var x2 = rawStamps.Max(s => s.Position.x);

        var y1 = rawStamps.Min(s => s.Position.y);
        var y2 = rawStamps.Max(s => s.Position.y);

        int width = x2 - x1;
        int height = y2 - y1;

        var size = stampController.ToTileCoords(new Vector2(width, height));
        var offset = stampController.ToTileCoords(new Vector2(x1, y1));

        Resize((int)(size.y * size.x));

        MatrixWidth = (int)size.x;

        stamps = rawStamps.Distinct().ToList();

        for(int i = 0; i < Length; i++)
        {
            ReplaceGene(i, -1);
        }

        foreach (var stamp in rawStamps)
        {
            var index = ToIndex(stampController.ToTileCoords(stamp.Position));
            ReplaceGene(index, stamps.FindIndex(s => s == stamp));
        }
    }

    public StampTileMapChromosome(int length, int matrixWidth, List<StampData> stamps) : base(length, matrixWidth)
    {
        this.stamps = stamps.Select(s => s).ToList();
    }
        
    public override IChromosome CreateNewChromosome()
    {
        var c = new StampTileMapChromosome(Length, MatrixWidth, stamps);
        return c;
    }

    public override object GenerateGene(int geneIndex)
    {
        return RandomizationProvider.Current.GetInt(0, stamps.Count);
    }
}
