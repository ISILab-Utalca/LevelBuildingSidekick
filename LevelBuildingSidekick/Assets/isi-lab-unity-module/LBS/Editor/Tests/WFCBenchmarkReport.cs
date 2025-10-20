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
            SampleGroup sg_01 = new SampleGroup("Run Time", SampleUnit.Millisecond); 
            SampleGroup sg_02 = new SampleGroup("GC Call", SampleUnit.Undefined); 
            
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(false));
                })
                .WarmupCount(1)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupWFCTest("27e9e2296bf8411458f727b699a7c0fe"))
                .GC()
                .CleanUp(CleanUpWFCTest)
                .Run();
        }
        
        [Test, Performance]
        public void TestMap_10x10()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(false));
                })
                .WarmupCount(1)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupWFCTest("4da2c1388b315494ab46c235d9727bbe"))
                .CleanUp(CleanUpWFCTest)
                .Run();
        }
        
        [Test, Performance]
        public void TestMap_20x20()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(false));
                })
                .WarmupCount(0)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupWFCTest("0e41f853d3bab334db4db772531db63c"))
                .CleanUp(CleanUpWFCTest)
                .Run();
        }
        
        [Test, Performance]
        [Timeout(3600000)]
        public void TestMap_40x40()
        {
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(false));
                })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetupWFCTest("01421564a101235499b4448d3801d494"))
                .CleanUp(CleanUpWFCTest)
                .Run();
        }
        
        
        [Test, Performance]
        public void TestMap_5x5_SameMap()
        {
            SetupWFCTest("27e9e2296bf8411458f727b699a7c0fe");
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(true));
                })
                .WarmupCount(1)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
            
            CleanUpWFCTest();
        }
        
        [Test, Performance]
        public void TestMap_10x10_SameMap()
        {
            SetupWFCTest("4da2c1388b315494ab46c235d9727bbe");
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(true));
                })
                .WarmupCount(1)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1)
                .Run();
            
            CleanUpWFCTest();
        }
        
        [Test, Performance]
        public void TestMap_20x20_SameMap()
        {
            SetupWFCTest("0e41f853d3bab334db4db772531db63c");
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(true));
                })
                .WarmupCount(0)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .Run();
            
            CleanUpWFCTest();
        }
        
        [Test, Performance]
        [Timeout(600000)]
        public void TestMap_40x40_SameMap()
        {
            SetupWFCTest("01421564a101235499b4448d3801d494");
            Measure.Method(() =>
                {
                    Assert.AreEqual(true, WFCassistant.ExecuteTest(true));
                })
                .WarmupCount(0)
                .MeasurementCount(10)
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
