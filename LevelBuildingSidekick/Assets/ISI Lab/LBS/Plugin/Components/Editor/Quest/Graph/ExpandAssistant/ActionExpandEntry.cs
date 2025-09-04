using UnityEngine;
using UnityEngine.UIElements;

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using TreeEditor;

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
    
        public void SetEntryAction(string action, QuestNode.ENodeType nodeType)
        {
            Color backgroundColor = Color.white;
            var iconSize = new BackgroundSize(28, 28);
            var iconPath = nodeType switch
            {
                QuestNode.ENodeType.Start => "Icons/Vectorial/QuestIcons/QuestIcon=StartNode",
                QuestNode.ENodeType.Middle => "Icons/Vectorial/QuestIcons/QuestIcon=MiddleNode",
                QuestNode.ENodeType.Goal => "Icons/Vectorial/QuestIcons/QuestIcon=EndNode",
                _ => throw new ArgumentOutOfRangeException()
            };

            _nodeTypeImage.style.backgroundSize = new StyleBackgroundSize(iconSize);
            _nodeTypeImage.style.backgroundImage = new StyleBackground(Resources.Load<VectorImage>(iconPath));
            _nodeTypeImage.style.unityBackgroundImageTintColor = backgroundColor;
            _nodeName.text = char.ToUpper(action[0]) + action[1..];

            MarkDirtyRepaint();
        }
        
        #endregion


    }
}