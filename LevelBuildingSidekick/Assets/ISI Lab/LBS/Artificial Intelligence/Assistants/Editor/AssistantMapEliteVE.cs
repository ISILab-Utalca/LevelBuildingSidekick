using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(AssistantMapElite))]
public class AssistantMapEliteVE : LBSCustomEditor
{
    MAPEliteConfiguration config;
    MAPEliteContent content;

    private object locker = new object();

    public AssistantMapEliteVE(object target) : base(target)
    {
        Add(CreateVisualElement());
    }

    private void Init()
    {
    }

    private void Run()
    {
        var assitant = target as AssistantMapElite;
        content.Empty();
        assitant.LoadPresset(config.GetPresset());
        assitant.SetAdam(new Rect()); // GET RECT FROM TOOL
        assitant.Execute();
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

        content.OnSelectOption += assistant.ApplySuggestion;

        ve.Add(content);
        ve.Add(config);
        return ve;
    }
}
