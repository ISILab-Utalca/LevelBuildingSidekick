using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
using ISILab.Extensions;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    public class QuestNodeView : GraphElement
    {
        private static VisualTreeAsset view;


        private Label label;
        private VisualElement root;

        public Action<Rect> OnMoving;

        public static Color32 GrammarWrong = Color.yellow;
        public static Color32 MapWrong = Color.red;
        public static Color32 Unchecked = Color.white;
        public static Color32 Correct = Color.blue;

        protected QuestNodeView() { }

        public QuestNodeView(QuestNode node)
        {
            if (view == null)
            {
                view = DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestNodeView");
            }
            view.CloneTree(this);

            // Label
            label = this.Q<Label>();
            root = this.Q<VisualElement>(name: "root");

            SetText(node.QuestAction);
            SetBorder(node);
        }

        private void SetBorder(QuestNode node)
        {
            root.SetBorder(Unchecked);

            if (!node.GrammarCheck)
            {
                root.SetBorder(GrammarWrong);
                return;
            }

            root.SetBorder(Correct);
        }

        public override void SetPosition(Rect newPos)
        {
            // Set new Rect position
            base.SetPosition(newPos);

            // call movement event
            OnMoving?.Invoke(newPos);

            MarkDirtyRepaint();
        }

        public void SetText(string text)
        {
            if (text.Length > 11)
            {
                text = text.Substring(0, 8) + "...";
            }

            label.text = text;
        }

    }
}