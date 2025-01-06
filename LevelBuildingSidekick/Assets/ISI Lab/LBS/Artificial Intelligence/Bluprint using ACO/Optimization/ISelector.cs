using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Optimization.Selector
{
    public interface ISelector
    {
        // neigbor, fitness, move
        public (object, float, string) Execute(List<(object, float, string)> population);
    }

    public class FirstBestSelector : ISelector
    {
        public (object, float, string) Execute(List<(object, float, string)> population)
        {
            return population.OrderByDescending(p => p.Item2).First();
        }
    }

    public class RandomBestSelector : ISelector
    {
        public (object, float, string) Execute(List<(object, float, string)> population)
        {
            var best = population.OrderByDescending(p => p.Item2).First();
            var bests = population.Where(p => p.Item2 == best.Item2).ToList();
            return bests[Random.Range(0, bests.Count)];
        }
    }

    public class RandomBestPercentil : ISelector
    {
        float percentil = 0.1f;

        public (object, float, string) Execute(List<(object, float, string)> population)
        {
            var sorted = population.OrderByDescending(p => p.Item2);
            var amount = (int)(population.Count * percentil);
            var bests = sorted.Take(amount).ToList();
            return bests[Random.Range(0, bests.Count)];
        }
    }

    public class RandomSelector : ISelector
    {
        public (object, float, string) Execute(List<(object, float, string)> population)
        {
            return population[Random.Range(0, population.Count)];
        }
    }
}