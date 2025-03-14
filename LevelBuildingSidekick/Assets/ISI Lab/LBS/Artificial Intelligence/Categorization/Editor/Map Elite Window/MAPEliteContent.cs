using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Internal;
using ISILab.LBS;

namespace ISILab.LBS.VisualElements
{
    public class MAPEliteContent : VisualElement
    {
        AssistantMapElite assistant;
        
        public ButtonWrapper[] Content = new ButtonWrapper[1];
        public VisualElement Container;
        private int buttonSize = 128;

        public Texture2D background;
        private Texture2D standbyImg;
        private Texture2D loadingImg;
        private Texture2D notFoundImg;

        private Label yAxis;
        private Label xAxis;

        private object locker = new object();

        public event Action<object> OnSelectOption;

        Vector2 partitions = Vector2.one;

        public MAPEliteContent(AssistantMapElite assistant)
        {
            var visualTree = LBSAssetsStorage.Instance.Get<VisualTreeAsset>().Find(e => e.name == "MAPEliteContent");
            visualTree.CloneTree(this);

            var s2 = EditorGUIUtility.Load("DefaultCommonDark.uss") as StyleSheet;
            styleSheets.Add(s2);


            Container = this.Q<VisualElement>("Content");
            xAxis = this.Q<Label>(name: "LabelX");
            yAxis = this.Q<Label>(name: "LabelY");

            this.assistant = assistant;
            CreateGUI();
        }

        public void Reset()
        {
            // set all wrapper to loading icon
            if (assistant.YEvaluator != null)
                yAxis.text = assistant.YEvaluator.GetType().Name;


            if (assistant.XEvaluator != null)
                xAxis.text = assistant.XEvaluator.GetType().Name;


            ChangePartitions(new Vector2(assistant.SampleWidth, assistant.SampleHeight));
        }

        public void ChangePartitions(Vector2 partitions)
        {
            if (partitions == this.partitions)
                return;
            this.partitions = partitions;
            assistant.SampleWidth = (int)partitions.x;
            assistant.SampleHeight = (int)partitions.y;
            
            Content = new ButtonWrapper[assistant.SampleWidth * assistant.SampleHeight];
            Container.Clear();

            Container.style.width = 6 + (buttonSize + 6) * assistant.SampleWidth;

            for (int i = 0; i < Content.Length; i++)
            {
                var b = new ButtonWrapper(null, new Vector2(buttonSize, buttonSize));
                b.clicked += () =>
                {
                    if (b.Data != null)
                    {
                        OnSelectOption?.Invoke(b.Data);
                        DrawManager.ReDraw();

                    }
                };
                Content[i] = b;
                Container.Add(b);
            }

            Content.ToList().ForEach(b => b.style.backgroundImage = standbyImg);
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

        public void UpdateContent()
        {
            for (int i = 0; i < assistant.toUpdate.Count; i++)
            {
                var v = assistant.toUpdate[i];
                var index = (int)(v.y * assistant.SampleWidth + v.x);
                Content[index].Data = assistant.Samples[(int)v.y, (int)v.x];
                Content[index].Text = ((decimal)assistant.Samples[(int)v.y, (int)v.x].Fitness).ToString("f4");
                var t = Content[index].GetTexture();
                if (Content[index].Data != null)
                {
                    Content[index].SetTexture(background.MergeTextures(t).FitSquare());
                }
                else
                {
                    Content[index].SetTexture(loadingImg);
                }
                Content[index].UpdateLabel();
            }
            assistant.toUpdate.Clear();
        }

        private void CreateGUI()
        {
            standbyImg = DirectoryTools.GetAssetByName<Texture2D>("PausedProcess");
            loadingImg = DirectoryTools.GetAssetByName<Texture2D>("LoadingContent");
            notFoundImg = DirectoryTools.GetAssetByName<Texture2D>("ContentNotFound");

            ChangePartitions(new Vector2(2, 2));
        }
    }
}