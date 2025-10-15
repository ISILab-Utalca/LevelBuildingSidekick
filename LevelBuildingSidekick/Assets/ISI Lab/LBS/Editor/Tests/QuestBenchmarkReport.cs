using System.Collections.Generic;
using System.Linq;
using ISILab.AI.Grammar;
using ISILab.JsonNet;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using NUnit.Framework;
using Unity.PerformanceTesting;

[TestFixture]
public class QuestBenchmarkReport
{
    private const int MeasureCount = 100;
    private const string Guid = "e1080717efc4d004b984e0a668717fb8";

    private LBSLevelData _levelData;
    private GrammarAssistant _grammarAssistant;
    private QuestAssistant _questAssistant;
    private QuestGraph _questGraph;

    #region NODE COUNT 10
    [Test, Performance] public void AddNextNode_10() => AddNextNodeBenchmark(10);
    [Test, Performance] public void AddPreviousNode_10() => AddPreviousNodeBenchmark(10);
    [Test, Performance] public void ExpandNode_10() => ExpandNodeBenchmark(10);
    #endregion

    #region NODE COUNT 20
    [Test, Performance] public void AddNextNode_20() => AddNextNodeBenchmark(20);
    [Test, Performance] public void AddPreviousNode_20() => AddPreviousNodeBenchmark(20);
    [Test, Performance] public void ExpandNode_20() => ExpandNodeBenchmark(20);
    #endregion

    #region NODE COUNT 30
    [Test, Performance] public void AddNextNode_30() => AddNextNodeBenchmark(30);
    [Test, Performance] public void AddPreviousNode_30() => AddPreviousNodeBenchmark(30);
    [Test, Performance] public void ExpandNode_30() => ExpandNodeBenchmark(30);
    #endregion

    #region METHODS
    private void AddNextNodeBenchmark(int nodeCount)
    {
        Measure.Method(() =>
        {
            var nodes = _questGraph.GetQuestNodes();
            QuestNode chosenNode = null;
            string nextAction = null;
            int attempts = 0;

            while (string.IsNullOrEmpty(nextAction) && attempts++ < 100)
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var nextActions = _grammarAssistant.GetAllValidNextActionsInsert(chosenNode.QuestAction, _questGraph);
                if (nextActions.Count > 0)
                    nextAction = nextActions[UnityEngine.Random.Range(0, nextActions.Count)];
            }

            Assert.IsFalse(string.IsNullOrEmpty(nextAction), $"No valid next action after {attempts} attempts (nodes={nodeCount})");
            _grammarAssistant.InsertNextAction(nextAction, chosenNode);
        })
        .WarmupCount(1)
        .MeasurementCount(MeasureCount)
        .IterationsPerMeasurement(1)
        .SetUp(() => SetupTestEnvironment(nodeCount))
        .CleanUp(CleanupTest)
        .Run();
    }

    private void AddPreviousNodeBenchmark(int nodeCount)
    {
        Measure.Method(() =>
        {
            var nodes = _questGraph.GetQuestNodes();
            QuestNode chosenNode = null;
            string prevAction = null;
            int attempts = 0;

            while (string.IsNullOrEmpty(prevAction) && attempts++ < 100)
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var prevActions = _grammarAssistant.GetAllValidPrevActionsInsert(chosenNode.QuestAction, _questGraph);
                if (prevActions.Count > 0)
                    prevAction = prevActions[UnityEngine.Random.Range(0, prevActions.Count)];
            }

            Assert.IsFalse(string.IsNullOrEmpty(prevAction), $"No valid previous action after {attempts} attempts (nodes={nodeCount})");
            _grammarAssistant.InsertPreviousAction(prevAction, chosenNode);
        })
        .WarmupCount(1)
        .MeasurementCount(MeasureCount)
        .IterationsPerMeasurement(1)
        .SetUp(() => SetupTestEnvironment(nodeCount))
        .CleanUp(CleanupTest)
        .Run();
    }

    private void ExpandNodeBenchmark(int nodeCount)
    {
        Measure.Method(() =>
        {
            var nodes = _questGraph.GetQuestNodes();
            QuestNode chosenNode = null;
            List<string> expansion = null;
            int attempts = 0;

            while ((expansion == null || !expansion.Any()) && attempts++ < 100)
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var expansions = _grammarAssistant.GetAllExpansions(chosenNode.QuestAction);
                if (expansions.Count > 0)
                    expansion = expansions[UnityEngine.Random.Range(0, expansions.Count)];
            }

            Assert.IsTrue(expansion != null && expansion.Any(), $"No valid expansion after {attempts} attempts (nodes={nodeCount})");
            _grammarAssistant.ExpandAction(expansion, chosenNode);
        })
        .WarmupCount(1)
        .MeasurementCount(MeasureCount)
        .IterationsPerMeasurement(1)
        .SetUp(() => SetupTestEnvironment(nodeCount))
        .CleanUp(CleanupTest)
        .Run();
    }
    #endregion

    #region SETUP - CLEANUP
    private void SetupTestEnvironment(int nodeCount)
    {
        _levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(Guid);
        Assert.IsNotNull(_levelData, $"Level data could not be loaded for GUID={Guid}");

        LBSLayer firstLayer = _levelData.GetLayer(0);
        Assert.IsNotNull(firstLayer, "First layer not found in level data");

        _questGraph = firstLayer.GetModule<QuestGraph>();
        _questAssistant = firstLayer.GetAssistant<QuestAssistant>();
        _grammarAssistant = firstLayer.GetAssistant<GrammarAssistant>();

        Assert.IsNotNull(_questGraph, "QuestGraph not found");
        Assert.IsNotNull(_questAssistant, "QuestAssistant not found");
        Assert.IsNotNull(_grammarAssistant, "GrammarAssistant not found");

        _questGraph.OwnerLayer = firstLayer;

        if (_questGraph.Grammar == null)
        {
            _questGraph.Grammar = LBSAssetMacro.LoadAssetByGuid<LBSGrammar>("63ab688b53411154db5edd0ec7171c42");
        }

        _questGraph.GraphNodes.Clear();
        _questAssistant.GenerateRandomNodes(nodeCount);
        _questAssistant.ConnectAllNodes();

        firstLayer.Reload();
    }

    private void CleanupTest()
    {
        if (_levelData != null)
        {
            var firstLayer = _levelData.GetLayer(0);
            firstLayer.RemoveAll();
            _questGraph = null;
            _questAssistant = null;
            _grammarAssistant = null;
            _levelData = null;
        }
    }
    #endregion
}
