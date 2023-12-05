using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManagerEditor : LBSCustomEditor, IToolProvider
{
    CreateQuestNode addNode;
    RemoveQuestNode removeNode;
    ConnectQuestNodes connectNodes;
    RemoveQuestConnection removeConnection;


    public override void SetInfo(object target)
    {
        var quest = (target as LBSQuestManager).selectedQuest;
    }

    public void SetTools(ToolKit toolkit)
    {
        throw new System.NotImplementedException();
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
