using LBS;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using Utility;
using static UnityEngine.UI.GridLayoutGroup;

[LBSCustomEditor("Assistant Map Elite", typeof(AssistantMapElite))]
public class AssistantMapEliteEditor : LBSCustomEditor, IToolProvider
{
    MAPEliteConfiguration config;
    MAPEliteContent content;

    private object locker = new object();


    ActOnRect ActOnRect;

    public AssistantMapEliteEditor(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    private void Run()
    {
        var assitant = target as AssistantMapElite;

        if (assitant.Running)
            return;

        content.Reset();
        assitant.LoadPresset(config.GetPresset());
        if(assitant.RawToolRect.width == 0 || assitant.RawToolRect.height == 0)
        {
            Debug.LogError("[ISI Lab]: Selected evolution area height or with < 0");
            return;
        }
        SetBackgorundTexture(assitant.RawToolRect);
        assitant.SetAdam(assitant.RawToolRect);
        assitant.Execute();
        LBSMainWindow.OnWindowRepaint += RepaintContent;
    }

    private void RepaintContent()
    {

        var assitant = target as AssistantMapElite;
        content.UpdateContent();
        if (assitant.Finished)
            LBSMainWindow.OnWindowRepaint -= RepaintContent;
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
        config.OnPressetChange += (p) =>
        {
            ChangePresset();
            ToolKit.Instance.SetActive("Select area to evaluate");
        };

        content.OnSelectOption += assistant.ApplySuggestion;


        ve.Add(content);
        ve.Add(config);
        return ve;
    }

    public void SetTools(ToolKit toolkit)
    {
        toolkit.AddSeparator();

        var assitant = target as AssistantMapElite;
        var icon = Resources.Load<Texture2D>("Icons/Tools/Area_MapElite");
        ActOnRect = new ActOnRect((r) => assitant.RawToolRect = r);
        var t1 = new LBSTool(icon, "Select area to evaluate", ActOnRect);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Assistants");
        t1.Init(assitant.Owner, assitant);
        toolkit.AddTool(t1);
    }

    public void SetBackgorundTexture(Rect rect)
    {
        var assitant = target as AssistantMapElite;
        var behaviours = assitant.Owner.Parent.Layers.SelectMany(l => l.Behaviours);
        var bh = assitant.Owner.Behaviours.Find(b => b is PopulationBehaviour);

        var size = 16;

        var textures = new List<Texture2D>();

        foreach (var b in behaviours)
        {
            if (b == null)
                continue;

            if (bh != null && b.Equals(bh))
                continue;

            var classes = Utility.Reflection.GetClassesWith<DrawerAttribute>();
            if (classes.Count == 0)
                continue;

            var drawers = classes.Where(t => t.Item2.Any(v => v.type == b.GetType()));

            if (drawers.Count() == 0)
                continue;

            var drawer = Activator.CreateInstance(drawers.First().Item1) as Drawer;
            textures.Add(drawer.GetTexture(b, rect, Vector2Int.one * size));
        }

        var texture = new Texture2D((int)(rect.width * size), (int)(rect.height * size));

        for (int j = 0; j < texture.height; j++)
        {
            for (int i = 0; i < texture.height; i++)
            {
                texture.SetPixel(i, j, new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1));
            }
        }

        for (int i = textures.Count - 1; i >= 0; i-- )
        {
            if (textures[i] == null)
                continue;

            //Debug.Log("Texture::: " + t.width + " - " + t.height);
            texture = texture.MergeTextures(textures[i]);
        }

        texture.Apply();


        //texture.FitSquare();

        content.background = texture;

    }
}
