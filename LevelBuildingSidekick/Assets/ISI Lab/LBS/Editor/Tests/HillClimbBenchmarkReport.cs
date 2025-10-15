using ISILab.JsonNet;
using ISILab.LBS.Assistants;
using LBS.Components;
using NUnit.Framework;
using Unity.PerformanceTesting;



namespace ISILab.LBS.Tests
{
    [TestFixture]
    public class HillClimbBenchmarkReport
    {
        LBSLevelData levelData ;
        HillClimbingAssistant HCassistant;
        
        
        [Test, Performance]
        [Timeout(3600000)]
        public void MeasureHillClimbing_25_Rooms()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(HCassistant.TryExecute(out string failedLog), true);
                })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupHillClimbTest("f97f322d306c1af43a8eaff23a17d2bd"))
                .CleanUp(CleanUpHillClimbTest)
                .Run();
        }
        
        
        [Test, Performance]
        [Timeout(3600000)]
        public void MeasureHillClimbing_16_Rooms()
        {
            Measure.Method(() =>
            {
                Assert.AreEqual(HCassistant.TryExecute(out string failedLog), true);
            })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupHillClimbTest("4f173a341c99d1a4b83b3a79c90da8e3"))
                .CleanUp(CleanUpHillClimbTest)
                .Run();
        }
        
        
        [Test, Performance]
        [Timeout(3600000)]
        public void MeasureHillClimbing_16_Rooms_Simpler()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(HCassistant.TryExecute(out string failedLog), true);
                })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupHillClimbTest("d9637e558e0b60149983dd9589d35fd5"))
                .CleanUp(CleanUpHillClimbTest)
                .Run();
        }
        
        [Test, Performance]
        public void MeasureHillClimbing_9_Rooms()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(HCassistant.TryExecute(out string failedLog), true);
                })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupHillClimbTest("804037966f1f95945b7e07a51821e4fa"))
                .CleanUp(CleanUpHillClimbTest)
                .Run();
        }
        
        [Test, Performance]
        public void MeasureHillClimbing_5_Rooms()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(HCassistant.TryExecute(out string failedLog), true);
                })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupHillClimbTest("a9427fc4639829544bb55127cacf1b0f"))
                .CleanUp(CleanUpHillClimbTest)
                .Run();
        }

        // [OneTimeSetUp]
        private void SetupHillClimbTest(string _guid)
        {
            levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(_guid);
            Assert.IsNotNull(levelData);
            LBSLayer fistLayer = levelData.GetLayer(0);
            Assert.IsNotNull(fistLayer);
            HCassistant = fistLayer.GetAssistant<HillClimbingAssistant>("");
            Assert.IsNotNull(HCassistant);
            fistLayer.Reload();
        }


        private void CleanUpHillClimbTest()
        {
            if (levelData != null)
            {
                LBSLayer fistLayer = levelData.GetLayer(0);
                fistLayer.RemoveAll();
                
                HCassistant = null;
                levelData = null;
            }
        }
    }

}

