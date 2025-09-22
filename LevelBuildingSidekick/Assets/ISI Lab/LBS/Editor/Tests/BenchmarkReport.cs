using ISILab.AI.Optimization;
using ISILab.JsonNet;
using ISILab.LBS.Assistants;
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
                
            })
                .WarmupCount(1)
                .MeasurementCount(20)
                .SetUp(() =>
            {
            })
                .Run();
        }



        [Test, Performance]
        public void LoadLargeMap()
        {
            Measure.Method(() =>
            {
                LBSLevelData loaded = JSONDataManager.LoadDataByGUID<LBSLevelData>("01421564a101235499b4448d3801d494");
                Assert.IsNotNull(loaded);
                
            })
                .WarmupCount(3)
                .MeasurementCount(20)
                .Run();
        }
        
            [Test, Performance]
            public void LoadSmallMap()
            {
                Measure.Method(() =>
                    {
                        LBSLevelData loaded = JSONDataManager.LoadDataByGUID<LBSLevelData>("27e9e2296bf8411458f727b699a7c0fe");
                        Assert.IsNotNull(loaded);
                    
                    })
                    .WarmupCount(3)
                    .MeasurementCount(20)
                    .Run();
            }
    }

}

