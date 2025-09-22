using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;


namespace ISILab.LBS.Tests
{
    [TestFixture]
    public class BenchmarkReport
    {

        [Test, Performance]
        public void MeasureHillClimbing()
        {
            Measure.Method(() =>
            {
                
            }).Run();
        }
    }

}

