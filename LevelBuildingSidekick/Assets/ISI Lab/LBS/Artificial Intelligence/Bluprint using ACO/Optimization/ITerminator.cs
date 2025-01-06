using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Terminators
{
    public interface ITerminator
    {
        public bool Execute();
    }

    public class AgregateTermination : ITerminator
    {
        public ITerminator[] terminators;

        public bool Execute()
        {
            foreach (var t in terminators)
            {
                if (t.Execute())
                    return true;
            }
            return false;
        }
    }

    public class IterationTerminator : ITerminator
    {
        public int maxIterations;
        private int currentIteration = 0;

        public bool Execute()
        {
            currentIteration++;
            return currentIteration > maxIterations;
        }
    }

    public class ManualTerminator : ITerminator
    {
        public bool Execute()
        {
            var value = Event.current.keyCode == KeyCode.Escape;
            return value;
            //return Input.GetKeyDown(KeyCode.Escape);
        }
    }

    public class TimeTerminator : ITerminator
    {
        public float maxTime;
        private float startTime;

        private bool inited = false;

        public bool Execute()
        {
            if (inited)
            {
                inited = true;
                startTime = Time.time;
            }

            return Time.time - startTime >= maxTime;
        }
    }
}