using System;
using ISILab.LBS.Components;

namespace ISILab.LBS
{
    [Serializable]
    public class QuestOrBranch : QuestBranch
    {
        public override bool CanAdvance()
        {
            foreach (var child in ChildTriggers)
            {
                var qt = child.GetComponent<QuestTrigger>();
                if (qt != null && qt.Node.QuestState == QuestState.Completed)
                    return true;
            }
            return false;
        }
    }
}