using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("AssistantMapElite", typeof(AssistantMapElite))]
public class AssistantMapEliteVE : LBSCustomEditor, IToolProvider
{
    MAPEliteConfiguration config;
    MAPEliteContent content;

    private object locker = new object();


    ActOnRect ActOnRect;

    public AssistantMapEliteVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    private void Run()
    {
        var assitant = target as AssistantMapElite;
        content.Reset();
        assitant.LoadPresset(config.GetPresset());
        assitant.SetAdam(assitant.Rect); // GET RECT FROM TOOL
        assitant.Execute();
    }

    public void ChangePresset()
    {
        var assitant = target as AssistantMapElite;
        assitant.LoadPresset(config.GetPresset());
        content.Reset();
    }

    private void Continue()
    {
        var assitant = target as AssistantMapElite;
        assitant.Continue();
    }

    private void UpdateContent()
    {
        var assitant = target as AssistantMapElite;
        lock (locker)
        {
            content.UpdateContent();
        }

        if (assitant.Finished)
        {
            content.MarkEmpties();
        }
    }


    public override void SetInfo(object target)
    {
    }

    

    protected override VisualElement CreateVisualElement()
    {
        var assistant = target as AssistantMapElite;

        var ve = new VisualElement();
        config = new MAPEliteConfiguration();
        content = new MAPEliteContent(assistant);


        config.OnCalculate += Run;
        config.OnContinue += Continue;
        config.OnPressetChange += (p) => ChangePresset();

        content.OnSelectOption += assistant.ApplySuggestion;

        ve.Add(content);
        ve.Add(config);
        return ve;
    }

    public void SetTools(ToolKit toolkit)
    {
        var assitant = target as AssistantMapElite;
        var icon = Resources.Load<Texture2D>("Icons/Select");
        ActOnRect = new ActOnRect((r) => assitant.Rect = r);
        var t1 = new LBSTool(icon, "Paint Zone", ActOnRect);
        t1.Init(assitant.Owner, assitant);
        toolkit.AddTool(t1);
    }
}
