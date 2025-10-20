using UnityEngine;
using UnityEngine.UIElements;

using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using ISILab.Macros;

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
        
        private const string startIconGuid = "f3d5f865060356e478b82ca9c9f186ad";
        private const string middleIconGuid = "f6e87932ed95e884199f001318ed0f76";
        private const string goalIconGuid = "fbb664e94acc4824eb24b56a5c6c9084";
        private const string singleIconGuid = "522548510e771274b90a7044b5e86531";
        
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

        public void SetEntryAction(string action, QuestNode.ENodeType nodeType, bool SingleEntry)
        {
            Color backgroundColor = Color.white;
            BackgroundSize iconSize = new BackgroundSize(28, 28);
            string iconPath = singleIconGuid;

            if (!SingleEntry)
            {
                iconPath = nodeType switch
                {
                    QuestNode.ENodeType.Start => startIconGuid,
                    QuestNode.ENodeType.Middle => middleIconGuid,
                    QuestNode.ENodeType.Goal => goalIconGuid,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            _nodeTypeImage.style.backgroundSize = new StyleBackgroundSize(iconSize);
            _nodeTypeImage.style.backgroundImage = new StyleBackground(LBSAssetMacro.LoadAssetByGuid<VectorImage>(iconPath));
            _nodeTypeImage.style.unityBackgroundImageTintColor = backgroundColor;
            _nodeName.text = char.ToUpper(action[0]) + action[1..];

            MarkDirtyRepaint();
        }
        
        #endregion


    }
}