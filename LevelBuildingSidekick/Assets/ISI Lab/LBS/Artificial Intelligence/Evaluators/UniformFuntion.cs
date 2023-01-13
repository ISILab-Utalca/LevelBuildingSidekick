using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class UniformFuntion : UniformEvaluator
{
    public override string GetName()
    {
        return "Uniform";
    }

    /// <summary>
    /// Returns the custom inspector GUI for this object.
    /// </summary>
    /// <returns>A <see cref="VisualElement"/> representing the custom inspector GUI.</returns>
    public override VisualElement CIGUI()
    {
        return base.CIGUI();
    }


    /// <summary>
    /// Evaluates the progression of a given object in a matrix represented by a <see cref="StampTileMapChromosome"/> object.
    /// </summary>
    /// <param name="stmc">The <see cref="StampTileMapChromosome"/> object representing the matrix.</param>
    /// <param name="id">The identifier of the object being evaluated.</param>
    /// <param name="height">The number of rows in the matrix.</param>
    /// <returns>A value between <see cref="MinValue"/> and <see cref="MaxValue"/> </returns>
    public override float EvaluateUniform(StampTileMapChromosome stmc, int id, int height, int[] data)
    {

        float presence = 0;
        int total = stmc.stamps.Count;
        
        foreach (var i in data)
        {
            if (id == i)
            {
                presence++;
            }
        }

        Debug.Log("Valor id: " + id + ", total: " + total + ", actuales: " + presence);
        //presence /= total;

        presence = (presence * 100 / total) / 100;

        Debug.Log("Valor Final: " + presence);

        //Debug.Log(Mathf.Clamp(presence, MinValue, MaxValue));
        return Mathf.Clamp(presence, MinValue, MaxValue);

        //return Mathf.Clamp(p, MinValue, MaxValue);
    }
}