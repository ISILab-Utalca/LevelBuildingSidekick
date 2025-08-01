using UnityEngine;
using UnityEngine.UIElements;

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class QuestHistoryEntry : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        /// <summary>
        /// Displays if the node is valid, or not
        /// </summary>
        private VisualElement _nodeState;
        /// <summary>
        /// Node type
        /// </summary>
        private readonly VisualElement _nodeTypeImage;
        /// <summary>
        /// Indicates if a node is valid in its position
        /// </summary>
        private readonly VisualElement _validImage;
        /// <summary>
        /// Displays the node ID
        /// </summary>
        private readonly Label _nodeName;
        
        private readonly Button _buttonGoTo;
        private readonly Button _buttonRemove;
        #endregion

        #region FIELDS
        private QuestNode _quest;
    
        #endregion

        #region EVENTS
       // public Action OnExecute;
        #endregion

        #region PROPERTIES

        public QuestNode Quest
        {
            get => _quest;
            set => _quest = value;
        }
        #endregion
        
        #region EVENTS
        public Action RemoveNode;
        public Action GoToNode;
        #endregion

        #region CONSTRUCTORS
        public QuestHistoryEntry()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestHistoryEntry");
            visualTree.CloneTree(this);
            
            _buttonGoTo = this.Q<Button>("GoToButton");
            if(_buttonGoTo != null) _buttonGoTo.clicked += () =>  GoToNode?.Invoke();
            _buttonRemove = this.Q<Button>("RemoveButton");
            if(_buttonRemove != null) _buttonRemove.clicked += () =>  RemoveNode?.Invoke();
            
            _nodeName = this.Q<Label>("QuestNodeName");
            
            _nodeTypeImage = this.Q<VisualElement>("NodeType");
            _validImage = this.Q<VisualElement>("NodeValid");
            
        }
        #endregion

        #region METHODS
        public void Update()
        {
            SetData(_quest);
        }
        
        public void SetData(QuestNode node)
        {
            if (node == null && _quest == null)
            {
                Debug.LogError("empty quest node");
                return;
            }
            _quest = node;
            _validImage.style.display = node is { ValidGrammar: true } ? DisplayStyle.None : DisplayStyle.Flex;

            BackgroundSize  iconSize = new BackgroundSize(12, 12);
            if (node != null)
            {
                string iconPath;
                Color backgroundColor;
                switch (node.NodeType)
                {
                    case NodeType.Start:
                        iconPath = "Icons/Vectorial/Icon=Start";
                        backgroundColor = LBSSettings.Instance.view.successColor;
                        break;
                    case NodeType.Middle:
                        iconPath = "Icons/Vectorial/Icon=MidNode";
                        backgroundColor = Color.white;
                        iconSize = new BackgroundSize(28, 28);
                        break;
                    case NodeType.Goal:
                        iconPath = "Icons/Vectorial/Icon=Goal";
                        backgroundColor = LBSSettings.Instance.view.errorColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _nodeTypeImage.style.backgroundSize = new StyleBackgroundSize(iconSize);
                _nodeTypeImage.style.backgroundImage = new StyleBackground(Resources.Load<VectorImage>(iconPath));
                _nodeTypeImage.style.unityBackgroundImageTintColor = backgroundColor;
                _nodeName.text = char.ToUpper(node.ID[0]) + node.ID.Substring(1);

            }

            MarkDirtyRepaint();
        }
        
        #endregion


    }
}