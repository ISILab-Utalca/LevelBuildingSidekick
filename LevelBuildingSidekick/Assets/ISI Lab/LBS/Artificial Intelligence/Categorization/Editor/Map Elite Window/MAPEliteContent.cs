using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class MAPEliteContent : VisualElement
{
    AssistantMapElite assistant;

    private List<Vector2Int> toUpdate = new List<Vector2Int>();
    public ButtonWrapper[] Content = new ButtonWrapper[1];
    public VisualElement Container;
    private int buttonSize = 128; //Should be a RangeSlider field(!!!)

    private Texture2D background;
    private Texture2D standbyImg;
    private Texture2D loadingImg;
    private Texture2D notFoundImg;

    private object locker = new object();

    public Action<object> OnSelectOption;

    Vector2 partitions = Vector2.one;

    public MAPEliteContent(AssistantMapElite assistant)
    {
        var visualTree = LBSAssetsStorage.Instance.Get<VisualTreeAsset>().Find(e => e.name == "MAPEliteContent");
        //var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MAPEliteContent");
        visualTree.CloneTree(this);

        var s2 = EditorGUIUtility.Load("DefaultCommonDark.uss") as StyleSheet;
        styleSheets.Add(s2);


        this.Container = this.Q<VisualElement>("Content");

        this.assistant = assistant;
        CreateGUI();
    }

    public void Reset()
    {
        toUpdate.Clear();

        // set all wrapper to loading icon
        ChangePartitions(new Vector2(assistant.SampleWidth, assistant.SampleHeight));
    }

    public void ChangePartitions(Vector2 partitions)
    {
        //ButtonBackground = BackgroundTexture(layer.GetModule<LBSModule>(BackgroundField.value));
        if (partitions == this.partitions)
            return;
        this.partitions = partitions;
        assistant.SampleWidth = (int)partitions.x;
        assistant.SampleHeight = (int)partitions.y;

        Content = new ButtonWrapper[assistant.SampleWidth * assistant.SampleHeight];
        Container.Clear();

        Container.style.width = 6 + (buttonSize + 6) * assistant.SampleWidth; // & es un padding que le asigna de forma automatica, no se de donde saca el valor

        for (int i = 0; i < Content.Length; i++)
        {
            var b = new ButtonWrapper(null, new Vector2(buttonSize, buttonSize));
            b.clicked += () =>
            {
                if (b.Data != null)
                {
                    OnSelectOption?.Invoke(b.Data);
                }
            };
            Content[i] = b;
            Container.Add(b);
        }

        Content.ToList().ForEach(b => b.style.backgroundImage = standbyImg);
    }

    public void UpdateContent()
    {
        for (int i = 0; i < toUpdate.Count; i++)
        {
            var v = toUpdate[i];
            var index = (v.y * assistant.SampleWidth + v.x);
            var t = Content[index].GetTexture();
            if (Content[index].Data != null)
            {
                Content[index].SetTexture(t);
            }
            else
            {
                Content[index].SetTexture(loadingImg);
            }
            Content[index].UpdateLabel();
        }
        toUpdate.Clear();
    }

    public void MarkEmpties()
    {
        foreach (ButtonWrapper bw in Content)
        {
            if (bw.Data == null)
            {
                bw.style.backgroundImage = notFoundImg;
            }
        }
    }

    public void UpdateSample(Vector2Int coords)
    {
        var index = (coords.y * assistant.SampleWidth + coords.x);
        if (Content[index].Data != null && (Content[index].Data as IOptimizable).Fitness > assistant.Samples[coords.y, coords.x].Fitness)
        {
            return;
        }
        Content[index].Data = assistant.Samples[coords.y, coords.x];
        Content[index].Text = ((decimal)assistant.Samples[coords.y, coords.x].Fitness).ToString("f4");



        lock (locker)
        {
            if (!toUpdate.Contains(coords))
                toUpdate.Add(coords);
        }
    }

    private void CreateGUI()
    {
        standbyImg = DirectoryTools.SearchAssetByName<Texture2D>("PausedProcess");
        loadingImg = DirectoryTools.SearchAssetByName<Texture2D>("LoadingContent");
        notFoundImg = DirectoryTools.SearchAssetByName<Texture2D>("ContentNotFound");

        ChangePartitions(new Vector2(assistant.SampleWidth, assistant.SampleHeight));
    }
}
