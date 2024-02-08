using ISILab.Commons.Utility.Editor;
using ISILab.LBS.VisualElements;
using LBS;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("QuestBehaviour", typeof(QuestBehaviour))]
public class QuestBehaviourEditor : LBSCustomEditor, IToolProvider
{
    CreateQuestNode addNode;
    RemoveQuestNode removeNode;
    ConnectQuestNodes connectNodes;
    RemoveQuestConnection removeConnection;

    DropdownField grammarDropdown;

    VisualElement actionPallete;

    List<LBSGrammar> grammars = new List<LBSGrammar>();

    public QuestBehaviourEditor(object target) : base(target)
    {
        CreateVisualElement();
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        var behaviour = (target as QuestBehaviour);
        if (behaviour == null)
            return;

        var quest = behaviour.Owner.GetModule<QuestGraph>();

        UpdateDropdown();

        if (quest.Grammar != null)
        {
            grammarDropdown.value = quest.Grammar.name;
        }

        UpdateContent();
    }

    public void SetTools(ToolKit toolkit)
    {
        var ass = target as QuestBehaviour;

        Texture2D icon;

        icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Add_Node_Quest");
        this.addNode = new CreateQuestNode();
        var t1 = new LBSTool(icon, "Add Quest Node", addNode);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
        t1.Init(ass.Owner, target);

        icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Quest");
        this.removeNode = new RemoveQuestNode();
        var t2 = new LBSTool(icon, "Remove Quest Node", removeNode);
        t2.Init(ass.Owner, target);

        icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Node_Connection_Quest");
        this.connectNodes = new ConnectQuestNodes();
        var t3 = new LBSTool(icon, "Connect Quest Node", connectNodes);
        t3.Init(ass.Owner, target);

        icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Connection_Quest");
        this.removeConnection = new RemoveQuestConnection();
        var t4 = new LBSTool(icon, "Remove Quest Connection", removeConnection);
        t4.Init(ass.Owner, target);

        toolkit.AddTool(t1);
        toolkit.AddTool(t2);
        toolkit.AddTool(t3);
        toolkit.AddTool(t4);
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
        visualTree.CloneTree(this);

        grammarDropdown = this.Q<DropdownField>(name: "Grammar");
        actionPallete = this.Q<VisualElement>(name: "Content");

        grammarDropdown.RegisterValueChangedCallback(evt => ChangeGrammar(evt.newValue));

        return this;
    }

    private void UpdateDropdown()
    {
        grammarDropdown.choices.Clear();
        grammars.Clear();
        var options = LBSAssetsStorage.Instance.Get<LBSGrammar>();
        if (options.Count == 0)
            return;
        grammars = options;
        grammarDropdown.choices = options.Select(s => s.name).ToList();
    }


    private void UpdateContent()
    {
        actionPallete.Clear();

        var behaviour = (target as QuestBehaviour);
        if (behaviour == null)
            return;

        var quest = behaviour.Owner.GetModule<QuestGraph>();
        if (quest == null)
            return;

        if (quest.Grammar == null)
            return;

        foreach (var a in quest.Grammar.Actions)
        {
            var b = new ActionButton(a.GrammarElement.Text, () =>
            {
                behaviour.ToSet = a.GrammarElement;
                ToolKit.Instance.SetActive("Add Quest Node");
            });
            actionPallete.Add(b);
        }
    }


    private void ChangeGrammar(string value)
    {
        var behaviour = (target as QuestBehaviour);
        if (behaviour == null)
            throw new Exception("No Assistant");

        var quest = behaviour.Owner.GetModule<QuestGraph>();
        if (quest == null)
            throw new Exception("No Module");

        var grammar = grammars.Find(s => s.name == value);
        if (grammar == null)
            throw new Exception("No Grammar");

        quest.Grammar = grammar;

        UpdateContent();
    }
}
