using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LBSChromosome : ChromosomeBase2D
{
    protected LBSChromosome(Rect rect, int[] immutables = null) : base(rect, immutables) { }

    protected LBSChromosome() : base() { }

}
