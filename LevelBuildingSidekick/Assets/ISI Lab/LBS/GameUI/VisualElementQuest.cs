using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS
{
    /// <summary>
    /// Visual Element used to indicate a single quest objective
    /// in the QuestVisualTree (TreeView).
    /// Displays the state of each quest step.
    /// </summary>
    [UxmlElement]
    public partial class VisualElementQuest : VisualElement
    {
        private Label _questLabel;
        private VisualElement _state;

        private QuestNode _questNode;

        private const string ActiveIconGuid = "2d0e21afed0e2b948a825ad351292248";
        private const string CompletedIconGuid = "eff207486cc48924b8691a0a3545be17";
        private const string OrIconGuid = "1d6ab847894293148bfa4d70136a75a9";
        private const string AndIconGuid = "84fdb6a97aae79e4eb5eba243b760ee7";
        private const string FailedIconGuid = "19533ec5deae6304ebe6b68e51ddeda1";

        public VisualElementQuest()
        {
            CreateVisualElement();
        }

        private void CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElementQuest");
            visualTree.CloneTree(this);

            _questLabel = this.Q<Label>("Action");
            _state = this.Q<VisualElement>("State");
        }

        /// <summary>
        /// Updates this element to represent the quest's state.
        /// Called from QuestVisualTree.TreeView binding.
        /// </summary>
        public void SetQuest(QuestObjective questObjective)
        {
            if (questObjective?.Trigger?.Node == null)
                return;

            _questNode = questObjective.Trigger.Node;

            Color color;
            bool closed = false;

            VectorImage vecImage;
            switch (_questNode.QuestState)
            {
                case QuestState.Blocked:
                    vecImage = LBSAssetMacro.LoadAssetByGuid<VectorImage>(ActiveIconGuid);
                    color = Color.gray;
                    break;

                case QuestState.Active:
                    vecImage = LBSAssetMacro.LoadAssetByGuid<VectorImage>(ActiveIconGuid);
                    color = Color.white;
                    break;

                case QuestState.Completed:
                    vecImage = LBSAssetMacro.LoadAssetByGuid<VectorImage>(CompletedIconGuid);
                    color = LBSSettings.Instance.view.successColor;
                    closed = true;
                    break;

                case QuestState.Failed:
                    vecImage = LBSAssetMacro.LoadAssetByGuid<VectorImage>(FailedIconGuid);
                    color = LBSSettings.Instance.view.errorColor;
                    closed = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // for branches
            if (questObjective.Trigger.Node != null)
            {
                vecImage = questObjective.Trigger.OwnerBranchNode switch
                {
                    AndNode => LBSAssetMacro.LoadAssetByGuid<VectorImage>(AndIconGuid),
                    OrNode => LBSAssetMacro.LoadAssetByGuid<VectorImage>(OrIconGuid),
                    _ => vecImage
                };
            }
            
            
            // State icon changes
            _state.style.backgroundImage = new StyleBackground(vecImage);
            _state.style.unityBackgroundImageTintColor = color;
            // Label changes
            _questLabel.style.color = new StyleColor(color);
            _questLabel.text = closed
                ? $"<s>{_questNode.QuestAction}</s>"
                : _questNode.QuestAction;
        }
    }
}
