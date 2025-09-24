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
        
        
        
        
        private void SetupHillClimbTest(string _guid)
        {
            levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(_guid);
            Assert.IsNotNull(levelData);
            LBSLayer fistLayer = levelData.GetLayer(0);
            Assert.IsNotNull(fistLayer);
            WFCassistant = fistLayer.GetAssistant<AssistantWFC>("");
            Assert.IsNotNull(WFCassistant);
            fistLayer.Reload();
        }
        
        private void CleanUpHillClimbTest()
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
