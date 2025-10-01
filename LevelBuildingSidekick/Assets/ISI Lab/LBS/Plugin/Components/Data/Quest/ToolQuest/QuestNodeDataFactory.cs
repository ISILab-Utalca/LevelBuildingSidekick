using System;
using System.Collections.Generic;

namespace ISILab.LBS.Components
{
    /// <summary>
    /// Returns a (<see cref="BaseQuestNodeData"/>) child class, based on a given action<c>string</c> from a <c>LBSGrammar</c>
    /// </summary>
    public static class QuestNodeDataFactory
    {
        // Make sure your Data type has the exact string as your terminals
        private static readonly Dictionary<string, Type> TagDataTypesPerTerminal = new()
        {
            { "go to", typeof(DataGoto) },
            { "explore", typeof(DataExplore) },
            { "kill", typeof(DataKill) } ,
            { "stealth", typeof(DataStealth) },
            { "take",typeof(DataTake) },
            { "read", typeof(DataRead) },
            { "exchange",typeof(DataExchange) },
            { "give",typeof(DataGive) },
            { "report",typeof(DataReport) },
            { "gather", typeof(DataGather) },
            { "spy",  typeof(DataSpy) },
            { "capture", typeof(DataCapture) },
            { "listen", typeof(DataListen) },
            { "empty", null }
        };
        
        public static BaseQuestNodeData CreateByTag(string tag, QuestNode owner)
        {
            if (!TagDataTypesPerTerminal.TryGetValue(tag, out var dataClass))
            {
                return null;
            }

            var nodeData = (BaseQuestNodeData)Activator.CreateInstance(dataClass, owner, tag);
            return nodeData;
        }
    }
    
}