using ISILab.JsonNet;
using ISILab.LBS.Assistants;
using LBS.Components;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace ISILab.LBS.Tests

{
    [TestFixture]
    public class WFCBenchmarkReport
    {
        LBSLevelData levelData ;
        AssistantWFC WFCassistant;

        [Test, Performance]
        public void TestMap_5x5()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest());
                })
                .WarmupCount(1)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupWFCTest("27e9e2296bf8411458f727b699a7c0fe"))
                .CleanUp(CleanUpWFCTest)
                .Run();
        }
        
        [Test, Performance]
        public void TestMap_5x5_SameMap()
        {
            SetupWFCTest("27e9e2296bf8411458f727b699a7c0fe");
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest());
                })
                .WarmupCount(1)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1)
                .Run();
            
            CleanUpWFCTest();
        }

        private void SetupWFCTest(string _guid)
        {
            levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(_guid);
            Assert.IsNotNull(levelData);
            LBSLayer fistLayer = levelData.GetLayer(0);
            Assert.IsNotNull(fistLayer);
            WFCassistant = fistLayer.GetAssistant<AssistantWFC>("");
            Assert.IsNotNull(WFCassistant);
            fistLayer.Reload();
        }
        
        private void CleanUpWFCTest()
        {
            if (levelData != null)
            {
                LBSLayer fistLayer = levelData.GetLayer(0);
                fistLayer.RemoveAll();
                
                WFCassistant = null;
                levelData = null;
            }
        }
    }
}
