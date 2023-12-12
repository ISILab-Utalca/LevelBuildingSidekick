using LBS;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("QuestManagerEditor", typeof(LBSQuestManager))]
public class QuestManagerEditor : LBSCustomEditor, IToolProvider
{
    CreateQuestNode addNode;
    RemoveQuestNode removeNode;
    ConnectQuestNodes connectNodes;
    RemoveQuestConnection removeConnection;

    List<LBSTool> tools = new List<LBSTool>();


    public QuestManagerEditor()
    {

    }

    public QuestManagerEditor(LBSQuestManager target) : base(target) 
    {
        Texture2D icon;

        // Set empty tile
        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.addNode = new CreateQuestNode();
        var t1 = new LBSTool(icon, "Add Quest Node", addNode);
        t1.Init(null, target);

        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.removeNode = new RemoveQuestNode();
        var t2 = new LBSTool(icon, "Add Quest Node", removeNode);
        t2.Init(null, target);

        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.connectNodes = new ConnectQuestNodes();
        var t3 = new LBSTool(icon, "Add Quest Node", connectNodes);
        t3.Init(null, target);

        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.removeConnection = new RemoveQuestConnection();
        var t4 = new LBSTool(icon, "Add Quest Node", removeConnection);
        t4.Init(null, target);

        tools.Add(t1);
        tools.Add(t2);
        tools.Add(t3);
        tools.Add(t4);
    }

    public override void SetInfo(object target)
    {
        var quest = (target as LBSQuestManager).SelectedQuest;
    }

    public void SetTools(ToolKit toolkit)
    {
        var manager = target as LBSQuestManager;

        addNode.Quest = manager.SelectedQuest;
        removeNode.Quest = manager.SelectedQuest;
        connectNodes.Quest = manager.SelectedQuest;
        removeConnection.Quest = manager.SelectedQuest;

        foreach (var t in tools) 
        {
            toolkit.AddTool(t);
        }
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
