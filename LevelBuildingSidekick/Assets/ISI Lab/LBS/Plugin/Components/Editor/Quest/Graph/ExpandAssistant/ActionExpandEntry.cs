using UnityEngine;
using UnityEngine.UIElements;

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class ActionExpandEntry : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        /// <summary>
        /// Node type
        /// </summary>
        private readonly VisualElement _nodeTypeImage;
        /// <summary>
        /// Displays the node ID
        /// </summary>
        private readonly Label _nodeName;
        #endregion

        #region CONSTRUCTORS
        public ActionExpandEntry()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ActionExpandEntry");
            visualTree.CloneTree(this);
            
            _nodeName = this.Q<Label>("QuestNodeName");
            _nodeTypeImage = this.Q<VisualElement>("NodeType");
 
            
        }
        #endregion

        #region METHODS
    
        public void SetEntryAction(string action, NodeType nodeType)
        {
            BackgroundSize  iconSize = new BackgroundSize(12, 12);
            string iconPath;
            Color backgroundColor = Color.white;
            iconSize = new BackgroundSize(28, 28);
            switch (nodeType)
            {
                case NodeType.Start:
                    iconPath = "Icons/Vectorial/QuestIcons/QuestIcon=StartNode";
                    break;
                case NodeType.Middle:
                    iconPath = "Icons/Vectorial/QuestIcons/QuestIcon=MiddleNode";
                    break;
                case NodeType.Goal:
                    iconPath = "Icons/Vectorial/QuestIcons/QuestIcon=EndNode";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _nodeTypeImage.style.backgroundSize = new StyleBackgroundSize(iconSize);
            _nodeTypeImage.style.backgroundImage = new StyleBackground(Resources.Load<VectorImage>(iconPath));
            _nodeTypeImage.style.unityBackgroundImageTintColor = backgroundColor;
            _nodeName.text = char.ToUpper(action[0]) + action[1..];

            MarkDirtyRepaint();
        }
        
        #endregion


    }
}