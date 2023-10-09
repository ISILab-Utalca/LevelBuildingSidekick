using LBS;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("QuestBehaviour", typeof(QuestBehaviour))]
public class QuestBehaviourEditor : LBSCustomEditor, IToolProvider
{
    public CreateNewGrammarNode addQuest;
    public RemoveNodeGrammar removeQuest;
    public ConnectQuestNodes connectQuest;
    public RemoveQuestConnection removeConnection;

    private QuestBehaviour questBH;

    //Palletes
    private SimplePallete grammarPallete;

    private ObjectField grammarField;

    private LBSGrammar targetGrammar;

    private object[] options;

    public QuestBehaviourEditor(object target) : base(target)
    {
        this.questBH = target as QuestBehaviour;

        this.CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        
    }

    public void SetTools(ToolKit toolkit)
    {
        Texture2D icon;

        // Add element Tiles
        icon = Resources.Load<Texture2D>("Icons/Quest icon/Add_Node_Quest");
        this.addQuest = new CreateNewGrammarNode();
        var t1 = new LBSTool(icon, "Add quest node", addQuest);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Behaviours");
        t1.Init(questBH.Owner, questBH);
        toolkit.AddTool(t1);

        // Remove element Tiles
        icon = Resources.Load<Texture2D>("Icons/Quest icon/Delete_Node_Quest");
        this.removeQuest = new RemoveNodeGrammar();
        var t2 = new LBSTool(icon, "Remove quest node", removeQuest);
        t2.Init(questBH.Owner, questBH);
        toolkit.AddTool(t2);

        // Connect Nodes    
        icon = Resources.Load<Texture2D>("Icons/Quest icon/Node_connection_Quest");
        this.connectQuest = new ConnectQuestNodes();
        var t3 = new LBSTool(icon, "Connect nodes", connectQuest);
        t3.Init(questBH.Owner, questBH);
        toolkit.AddTool(t3);

        // Remove Connection
        icon = Resources.Load<Texture2D>("Icons/Quest icon/Delete_node_connection_quest");
        this.removeConnection = new RemoveQuestConnection();
        var t4 = new LBSTool(icon, "Remove connection", removeConnection);
        t4.Init(questBH.Owner, questBH);
        toolkit.AddTool(t4);
    }

    protected override VisualElement CreateVisualElement()
    {
        // BundleField
        this.grammarField = new ObjectField();
        grammarField.objectType = typeof(LBSGrammar);
        this.Add(grammarField);

        //this.bundleField.value = this.targetBundle;
        grammarField.RegisterValueChangedCallback(evt =>
        {
            targetGrammar = evt.newValue as LBSGrammar;
            SetBundlePallete(targetGrammar);
        });

        grammarPallete = new SimplePallete();
        this.Add(grammarPallete);
        SetBundlePallete(targetGrammar);
        
        return this;
    }

    private void SetBundlePallete(LBSGrammar grammar)
    {
        grammarPallete.name = "Grammar";

        grammarPallete.ShowGroups = false;
        grammarPallete.ShowAddButton = false;
        grammarPallete.ShowRemoveButton = false;

        if (grammar == null)
            return;

        var actions = grammar.Actions;
        // Set Options
        options = new object[actions.Count];
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i] == null)
                continue;

            options[i] = actions[i];
        }

        grammarPallete.OnSelectOption += (option) =>
        {
            var action = option as ActionTargetDepiction;
            addQuest.actionToSet = action.GrammarElement;
            ToolKit.Instance.SetActive("Add quest node");
        };

        grammarPallete.SetOptions(options, (OptionView, option) =>
        {
            var action = option as ActionTargetDepiction;
            OptionView.Label = action.GrammarElement.ID;
            OptionView.Icon = null;
            OptionView.Color = Color.black;
        });

        grammarPallete.Repaint();
    }
}
