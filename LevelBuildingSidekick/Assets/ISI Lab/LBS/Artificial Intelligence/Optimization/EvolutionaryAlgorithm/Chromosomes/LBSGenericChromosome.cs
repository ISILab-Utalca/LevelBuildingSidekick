using GeneticSharp.Domain.Chromosomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSGenericChromosome : ChromosomeBase
{
    Rect rect;


    public LBSGenericChromosome(Rect rect)
    {

    }

    public override ChromosomeBase CloneChromosome()
    {
        throw new System.NotImplementedException();
    }

    public override ChromosomeBase CreateNewChromosome()
    {
        throw new System.NotImplementedException();
    }

    public override object GenerateGene()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public override void SetDeafult(int index)
    {
        throw new System.NotImplementedException();
    }
}


public class ModuleData
{
    string moduleID;
    object[] data;

    public ModuleData(string moduleID, object[] data)
    {
        this.moduleID = moduleID;
        this.data = data;
    }
}