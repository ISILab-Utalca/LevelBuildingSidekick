using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;


public class LayoutEvaluator : IRangedEvaluator
{

    float min = 0;
    float max = 1;
    int Space = 1;
    public float MaxValue => max;

    public float MinValue => min;


    /// <summary>
    /// Evaluates the given evaluable object.
    /// </summary>
    /// <param name="evaluable">The evaluable object to evaluate.</param>
    /// <returns>A float value representing the evaluation of the given object.</returns>
    public float Evaluate(IEvaluable evaluable)
    {
        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;
        int matrixWidth = (evaluable as ITileMap).MatrixWidth;

        var data = stmc.GetGenes<int>();
        var total = stmc.stamps.Count;
        var list = stmc.stamps;
        int height = data.Length / matrixWidth;

        float Fitness = 0;
        

        List<Vector2> pos = new List<Vector2>();
        float elements = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < matrixWidth / 2; i++)
            {
                if ((int)(object)data[matrixWidth * j + i] == -1) continue;
                
                else
                {
                    pos.Add(new Vector2(j, i));
                    elements++;
                }
            }
        }

        foreach (var pos_ in pos)
        {
            var pos_stamp = pos_;
            float temp_fit = 0;
            int count = 0;
            var Edge = 0;
        
            for (int i = (int)pos_stamp.x - Space; i <= (pos_stamp.x + Space); i++)
            {
                for (int j = (int)pos_stamp.y - Space; j <= pos_stamp.y + Space; j++)
                {

                    if (i == pos_stamp.x && j == pos_stamp.y) continue;

                    if ((int)stmc.ToIndex(new Vector2(i, j)) <= data.Length && (int)stmc.ToIndex(new Vector2(i, j)) >= 0)
                    {
                        Edge = data[stmc.ToIndex(new Vector2(i, j))];

                        if (Edge < 0)
                        {
                            temp_fit++;
                        }
                    }

                    count++;
                }
            }

            temp_fit /= count;

            if(temp_fit > 1) { temp_fit = 1 / temp_fit; }
            Fitness += temp_fit;

        }

        Fitness /= (elements*Space);

        Debug.Log("Final: " + Fitness);

        return Mathf.Clamp(Fitness, MinValue, MaxValue);
    }

    public string GetName()
    {
        return "Layout Evaluator";
    }

    /// <summary>
    /// Creates a visual element for the current object.
    /// </summary>
    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });


        var intField = new IntegerField("Space: ");
        intField.value = Space;
        intField.RegisterCallback<ChangeEvent<int>>((e) => {
            if (e.newValue >= 1)
            {
                Space = e.newValue;
            }
            else
            {
                intField.value = 1;
                Space = 1;
            }
        });

        content.Add(intField);

        content.Add(v2);

        return content;
    }
}
