using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class QuestEntry : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private VisualElement nodeState; // wether the node is valid
        private VisualElement nodeTypeImage;
        private VisualElement validImage;
        
        private Label nodeName;
        
        private Button buttonGoTo;
        private Button buttonRemove;
        #endregion

        #region FIELDS
        private QuestNode quest;
    
        #endregion

        #region EVENTS
       // public Action OnExecute;
        #endregion

        #region PROPERTIES

        public QuestNode Quest
        {
            get => quest;
            set => quest = value;
        }
        #endregion
        
        #region EVENTS
        public Action RemoveNode;
        public Action GoToNode;
        #endregion

        #region CONSTRUCTORS
        public QuestEntry()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestEntry");
            visualTree.CloneTree(this);
            
            buttonGoTo = this.Q<Button>("GoToButton");
            if(buttonGoTo != null) buttonGoTo.clicked += () =>  GoToNode?.Invoke();
            buttonRemove = this.Q<Button>("RemoveButton");
            if(buttonRemove != null) buttonRemove.clicked += () =>  RemoveNode?.Invoke();
            
            nodeName = this.Q<Label>("QuestNodeName");
            
            nodeTypeImage = this.Q<VisualElement>("NodeType");
            validImage = this.Q<VisualElement>("NodeValid");
            
        }
        #endregion

        #region METHODS
        public void Update()
        {
            SetData(quest);
        }
        
        public void SetData(QuestNode node)
        {
            if (node == null && quest == null)
            {
                //Debug.LogError("empty quest node");
                return;
            }
            quest = node;
            validImage.style.display = node.GrammarCheck ? DisplayStyle.None : DisplayStyle.Flex;

            string iconPath;
            Color backgroundColor;
            BackgroundSize  iconSize = new BackgroundSize(12, 12);
            switch (node.NodeType)
            {
                case NodeType.start:
                    iconPath = "Icons/Vectorial/Icon=Start";
                    backgroundColor = LBSSettings.Instance.view.successColor;
                    break;
                case NodeType.middle:
                    iconPath = "Icons/Vectorial/Icon=MidNode";
                    backgroundColor = Color.white;
                    iconSize = new BackgroundSize(28, 28);
                    break;
                case NodeType.goal:
                    iconPath = "Icons/Vectorial/Icon=Goal";
                    backgroundColor = LBSSettings.Instance.view.errorColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            nodeTypeImage.style.backgroundSize = new StyleBackgroundSize(iconSize);
            nodeTypeImage.style.backgroundImage = new StyleBackground(Resources.Load<VectorImage>(iconPath));
            nodeTypeImage.style.unityBackgroundImageTintColor = backgroundColor;
            nodeName.text = node.ID;
            
            MarkDirtyRepaint();
        }
        
        #endregion


    }
}