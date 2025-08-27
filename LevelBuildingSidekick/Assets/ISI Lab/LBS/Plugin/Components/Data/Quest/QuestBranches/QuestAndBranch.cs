using ISILab.LBS.Components;

namespace ISILab.LBS
{

    public class QuestAndBranch : QuestBranch
    {
        public override bool CanAdvance()
        {
            foreach (var child in ChildTriggers)
            {
                var qt = child.GetComponent<QuestTrigger>();
                if (qt == null || qt.Node.QuestState != QuestState.Completed)
                    return false;
            }
            return true;
        }
    }
}