using Commons.Optimization.Evaluator;
using ISILab.AI.Categorization;
using ISILab.JsonNet;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Assistants;
using LBS.Components;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEditor;

namespace ISILab.LBS.Tests
{
    [TestFixture]
    public class MAPElitesBenchmarkReport
    {
        LBSLevelData levelData;
        AssistantMapElite assistant;
        MAPElitesPreset preset;
        IRangedEvaluator og_optimizer;
        IRangedEvaluator og_xEvaluator;
        IRangedEvaluator og_yEvaluator;

        const string level4Rooms = "d26957894fd4ddb43b3eba81012a128c";
        const string level20Rooms = "ecb7a13f44837d845b00a5a19660369d";

        const string dungeonPresetPath = "Assets/ISI Lab/LBS/Presets/Assistants/DungeonPreset.asset";

        [Test, Performance]
        public void MeasureMAPElites_4_Rooms_Exploration()
        {
            Measure.Method(() =>
            {
                assistant.Execute(true);
            })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetUpMAPElitesTest(level4Rooms, dungeonPresetPath, new DCExploration(), new DCResourceSafety(), new DCSafeArea()))
                .CleanUp(CleanUpMAPElitesTest)
                .Run();
        }

        [Test, Performance]
        public void MeasureMAPElites_4_Rooms_ResourceSafety()
        {
            Measure.Method(() =>
            {
                assistant.Execute(true);
            })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetUpMAPElitesTest(level4Rooms, dungeonPresetPath, new DCResourceSafety(), new DCSafeArea(), new DCExploration()))
                .CleanUp(CleanUpMAPElitesTest)
                .Run();
        }

        [Test, Performance]
        public void MeasureMAPElites_4_Rooms_SafeArea()
        {
            Measure.Method(() =>
            {
                assistant.Execute(true);
            })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetUpMAPElitesTest(level4Rooms, dungeonPresetPath, new DCSafeArea(), new DCExploration(), new DCResourceSafety()))
                .CleanUp(CleanUpMAPElitesTest)
                .Run();
        }

        [Test, Performance]
        public void MeasureMAPElites_20_Rooms_Exploration()
        {
            Measure.Method(() =>
            {
                assistant.Execute(true);
            })
                .WarmupCount(0)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .SetUp(() => SetUpMAPElitesTest(level20Rooms, dungeonPresetPath, new DCExploration(), new DCResourceSafety(), new DCSafeArea()))
                .CleanUp(CleanUpMAPElitesTest)
                .Run();
        }

        private void SetUpMAPElitesTest(string _guid, string presetPath, IRangedEvaluator optimizer, IRangedEvaluator xEvaluator, IRangedEvaluator yEvaluator)
        {
            levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(_guid);
            Assert.IsNotNull(levelData, "Could not load level.");
            LBSLayer fistLayer = levelData.GetLayer("Population");
            Assert.IsNotNull(fistLayer, "Layer was null.");
            assistant = fistLayer.GetAssistant<AssistantMapElite>();
            Assert.IsNotNull(assistant, "Assistant Map Elite was null");
            fistLayer.Reload();

            assistant.Testing = true; // Prevents the algorithm from trying to use the window

            //Assert.IsNotNull(assistant.LayerPopulation, "Cannot get Population Behaviour through assistant.");
            //Assert.IsNotNull(assistant.LayerPopulation.OwnerLayer, "Cannot get layer through assistant.");
            //Assert.IsNotNull(assistant.LayerPopulation.OwnerLayer.Parent, "Layer parent was null."); // <--- Falla
            //Assert.IsNotNull(assistant.Data, "Could not read level data through assistant.");
            //Assert.IsNotNull(assistant.Data.ContextLayers, "Could not read level context layers.");
            assistant.LayerPopulation.OwnerLayer.Parent = levelData;
            Assert.IsTrue(assistant.Data.ContextLayers.Count > 0, "No context layers found.");
            assistant.AutoSelectArea(out _);
            Assert.IsTrue(assistant.RawToolRect.width > 0 && assistant.RawToolRect.height > 0, "Area selection is 0.");

            preset = AssetDatabase.LoadAssetAtPath<MAPElitesPreset>(presetPath);
            Assert.IsNotNull(preset, "Could not load preset.");
            Assert.IsNotNull(preset.Optimizer, "Optimizer was null");
            //Assert.IsNotNull(preset.Optimizer.Evaluator, "Optimizer evaluator was null");
            //Assert.IsNotNull(preset.XEvaluator)

            og_optimizer = preset.Optimizer.Evaluator as IRangedEvaluator;
            og_xEvaluator = preset.XEvaluator;
            og_yEvaluator = preset.YEvaluator;

            preset.Optimizer.Evaluator = optimizer;
            preset.XEvaluator = xEvaluator;
            preset.YEvaluator = yEvaluator;

            assistant.InitializeEvaluator(preset.Optimizer.Evaluator);
            assistant.InitializeEvaluator(preset.XEvaluator);
            assistant.InitializeEvaluator(preset.YEvaluator);

            assistant.LoadPresset(preset);
            assistant.SetAdam(assistant.RawToolRect, levelData.ContextLayers);
        }

        private void CleanUpMAPElitesTest()
        {
            preset.Optimizer.Evaluator = og_optimizer;
            preset.XEvaluator = og_xEvaluator;
            preset.YEvaluator = og_yEvaluator;

            if (levelData is not null)
            {
                LBSLayer firstLayer = levelData.GetLayer(0);
                firstLayer.RemoveAll();

                assistant = null;
                levelData = null;
            }
        }
    }
}
