using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using System.Linq;
using System;
using UnityEngine.UIElements;
using Commons.Optimization;
using System.Threading;

[System.Serializable]
public class MapElites
{
    private int xSampleCount;
    public int XSampleCount
    {
        get
        {
            return xSampleCount;
        }
        set
        {
            if(xSampleCount != value && value > 0)
            {
                xSampleCount = value;
                OnSampleSizeChange();
            }
        }
    }

    private int ySampleCount;
    public int YSampleCount
    {
        get
        {
            return ySampleCount;
        }
        set
        {
            if(ySampleCount != value && value > 0)
            {
                ySampleCount = value;
                OnSampleSizeChange();
            }
        }
    }

    Action OnSampleSizeChanged;

    [Range(0, 0.5f)]
    public double devest = 0.5;

    public IEvaluable Adam { get; set; }
    public IEvaluable[,] BestSamples { get; private set; }
    public List<int> changedSample;

    [SerializeField ,SerializeReference]
    IRangedEvaluator xEvaluator;
    public IRangedEvaluator XEvaluator
    {
        get
        {
            return xEvaluator;
        }
        set
        {
            if(xEvaluator == null || !xEvaluator.Equals(value))
            {
                xEvaluator = value;
                OnEvaluatorChange();
            }
        }
    }

    [SerializeReference]
    IRangedEvaluator yEvaluator;
    public IRangedEvaluator YEvaluator
    {
        get
        {
            return yEvaluator;
        }
        set
        {
            if(yEvaluator == null || !yEvaluator.Equals(value))
            {
                yEvaluator = value;
                OnEvaluatorChange();
            }
        }
    }

    Action OnEvaluatorChanged;

    [SerializeReference]
    IOptimizer optimizer;
    public IOptimizer Optimizer
    {
        get
        {
            return optimizer;
        }
        set
        {
            optimizer = value;
            OnOptimizerChange();
        }
    }

    public System.Action OnOptimizerChanged;

    public Action<Vector2Int> OnSampleUpdated;

    private Thread thread;

    public bool Running => thread != null && thread.IsAlive && thread.ThreadState == ThreadState.Running;

    public MapElites()
    {
        xEvaluator = null;
        yEvaluator = null;
        xSampleCount = 5;
        ySampleCount = 5;
        devest = 0.5;
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
    }

    public MapElites(IRangedEvaluator xEvaluator, IRangedEvaluator yEvaluator)
    {
        this.xEvaluator = xEvaluator;
        this.yEvaluator = yEvaluator;
        xSampleCount = 5;
        ySampleCount = 5;
        devest = 0.5;
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
    }

    public void Run()
    {
        Optimizer.Adam = Adam;
        Optimizer.OnGenerationRan += () => UpdateSamples(Optimizer.LastGeneration);
        Optimizer.OnTerminationReached += () =>
        {
            int c = 0;
            for (int j = 0; j < BestSamples.GetLength(1); j++)
            {
                for (int i = 0; i < BestSamples.GetLength(0); i++)
                {
                    if(BestSamples[i,j] != null)
                    {
                        c++;
                        UpdateSample(i,j,BestSamples[i,j]);
                    }
                }
            }
            if(Running)
            {
                thread.Join();
            }
            Debug.Log("Finished: " + c);

        };
        thread = new Thread(Optimizer.Start);
        thread.Start(); 
    }

    public void UpdateSamples(IEvaluable[] samples)
    {
        
        var evaluables = MapSamples(samples);

        float xStep = Mathf.Abs(XEvaluator.MaxValue - XEvaluator.MinValue) / XSampleCount;
        float yStep = Mathf.Abs(YEvaluator.MaxValue - YEvaluator.MinValue) / YSampleCount;

        foreach (var me in evaluables)
        {
            var xPos = (me.xFitness - XEvaluator.MinValue) / xStep;
            var yPos = (me.yFitness - YEvaluator.MinValue) / yStep;


            var tileXPos = (int)xPos;
            var tileYPos = (int)yPos;

            var dx = Mathf.Abs(0.5f - (xPos - tileXPos));
            var dy = Mathf.Abs(0.5f - (yPos - tileYPos));


            if (dx <= devest && dy <= devest)
            {
                tileXPos = tileXPos >= XSampleCount ? tileXPos - 1 : tileXPos;
                tileYPos = tileYPos >= YSampleCount ? tileYPos - 1 : tileYPos;
                UpdateSample(tileXPos, tileYPos, me.evaluable);
            }

            //Debug.Log(xPos + " - " + yPos);
        }
    }

    public bool UpdateSample(int x, int y, IEvaluable evaluable)
    {
        if (BestSamples[x,y] == null)
        {
            BestSamples[x, y] = evaluable;
            OnSampleUpdated?.Invoke(new Vector2Int(x,y));
            return true;
        }

        if(BestSamples[x,y].Fitness <= evaluable.Fitness)
        {
            BestSamples[x, y] = evaluable;
            OnSampleUpdated?.Invoke(new Vector2Int(x, y));
            return true;
        }
        return false;
    }

    public List<MappedIEvaluable> MapSamples(IEvaluable[] samples)
    {
        List<MappedIEvaluable> evaluables = new List<MappedIEvaluable>();

        foreach (var s in samples)
        {
            evaluables.Add(new MappedIEvaluable(s, XEvaluator.Evaluate(s), YEvaluator.Evaluate(s)));
        }

        return evaluables;
    }

    private void OnSampleSizeChange()
    {
        Clear();
        OnSampleSizeChanged?.Invoke();
    }

    private void OnEvaluatorChange()
    {
        Clear();
        OnEvaluatorChanged?.Invoke();
    }

    private void OnOptimizerChange()
    {
        Clear();
        OnOptimizerChanged?.Invoke();
        //Optimizer.OnGenerationRan += () => UpdateSamples(Optimizer.LastGeneration);
    }

    private void Clear()
    {
        BestSamples = new IEvaluable[xSampleCount, ySampleCount];
    }
}

public struct MappedIEvaluable
{
    public float xFitness;
    public float yFitness;
    public IEvaluable evaluable;

    public MappedIEvaluable(IEvaluable evaluable, float xFitness, float yFitness)
    {
        this.evaluable = evaluable;
        this.xFitness = xFitness;
        this.yFitness = yFitness;
    }
}
