using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ISILab.JsonNet;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace ISILab.LBS.Tests
{
    [Serializable]
    struct QuestBenchmarkReportEntry
    {
        public string text;
        public string time;

        public QuestBenchmarkReportEntry(string Text, double Time)
        {
            text = Text;
            time = Time.ToString();
        }
    }

    [TestFixture]
    public class QuestBenchmarkReport
    {
        // Maps node count → list of benchmark results
        private Dictionary<int, List<QuestBenchmarkReportEntry>> resultEntries;

        int[] nodeTestSequence = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        LBSLevelData levelData;
        GrammarAssistant GrammarAssistant;
        QuestAssistant QuestAssistant;
        QuestGraph QuestGraph;

        [SetUp]
        public void Init()
        {
            resultEntries = new Dictionary<int, List<QuestBenchmarkReportEntry>>();
        }

        [Test, Performance]
        public void TestQuest_AllNodeCounts()
        {
            foreach (var nodeCount in nodeTestSequence)
            {
                SetupGrammarTest("e1080717efc4d004b984e0a668717fb8", nodeCount);
                CleanupTest();
            }

            // Dump results as JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(resultEntries, Newtonsoft.Json.Formatting.Indented);
            UnityEngine.Debug.Log(json);
        }

        private void SetupGrammarTest(string _guid, int nodeCount)
        {
            levelData = JSONDataManager.LoadDataByGUID<LBSLevelData>(_guid);
            Assert.IsNotNull(levelData);

            LBSLayer fistLayer = levelData.GetLayer(0);
            Assert.IsNotNull(fistLayer);

            GrammarAssistant = fistLayer.GetAssistant<GrammarAssistant>("");
            Assert.IsNotNull(GrammarAssistant);

            QuestAssistant = fistLayer.GetAssistant<QuestAssistant>("");
            Assert.IsNotNull(QuestAssistant);

            QuestGraph = fistLayer.GetModule<QuestGraph>("");
            Assert.IsNotNull(QuestGraph);
            
            fistLayer.Reload();

            // Generate graph
            QuestAssistant.GenerateRandomNodes(nodeCount);
            QuestAssistant.ConnectAllNodes();

            var nodes = QuestGraph.GetQuestNodes();
            List<QuestBenchmarkReportEntry> entryResults = new List<QuestBenchmarkReportEntry>();

            // Insert Next
            string nextAction = string.Empty;
            QuestNode chosenNode = null;
            while (nextAction == string.Empty)
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var nextActions = GrammarAssistant.GetAllValidNextActionsInsert(chosenNode.QuestAction, QuestGraph);
                if (nextActions.Count > 0)
                    nextAction = nextActions[UnityEngine.Random.Range(0, nextActions.Count)];
            }

            var stopwatch = Stopwatch.StartNew();
            GrammarAssistant.InsertNextAction(nextAction, chosenNode);
            stopwatch.Stop();
            entryResults.Add(new QuestBenchmarkReportEntry("Insert next", stopwatch.ElapsedMilliseconds));

            // Insert Prev
            string prevAction = string.Empty;
            while (prevAction == string.Empty)
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var prevActions = GrammarAssistant.GetAllValidPrevActionsInsert(chosenNode.QuestAction, QuestGraph);
                if (prevActions.Count > 0)
                    prevAction = prevActions[UnityEngine.Random.Range(0, prevActions.Count)];
            }

            stopwatch = Stopwatch.StartNew();
            GrammarAssistant.InsertPreviousAction(prevAction, chosenNode);
            stopwatch.Stop();
            entryResults.Add(new QuestBenchmarkReportEntry("Insert previous", stopwatch.ElapsedMilliseconds));

            // Expand
            List<string> expansion = new List<string>();
            while (!expansion.Any())
            {
                chosenNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
                var expansions = GrammarAssistant.GetAllExpansions(chosenNode.QuestAction);
                if (expansions.Count > 0)
                    expansion = expansions[UnityEngine.Random.Range(0, expansions.Count)];
            }

            stopwatch = Stopwatch.StartNew();
            GrammarAssistant.ExpandAction(expansion, chosenNode);
            stopwatch.Stop();
            entryResults.Add(new QuestBenchmarkReportEntry("Expansion", stopwatch.ElapsedMilliseconds));

            // Store results for this node count
            resultEntries[nodeCount] = entryResults;
        }

        private void CleanupTest()
        {
            if (levelData != null)
            {
                LBSLayer fistLayer = levelData.GetLayer(0);
                fistLayer.RemoveAll();

                GrammarAssistant = null;
                QuestAssistant = null;
                QuestGraph = null;
                levelData = null;
            }
        }
    }
}
