using System;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestNodeBehaviour : LBSBehaviour
    {
        public QuestGraph Graph => OwnerLayer.GetModule<QuestGraph>();
        
        private QuestNode _selectedQuestNode;
        /// <summary>
        /// Assigned from the QuestNodeView On MouseDown event. It will assign the current selected node, allowing to
        /// modify it based on its action type.
        /// </summary>
        public QuestNode SelectedQuestNode
        {
            get => _selectedQuestNode;
            set
            {
                _selectedQuestNode = value;
                OnQuestNodeSelected?.Invoke(_selectedQuestNode);
            }
        }
        
        public event Action<QuestNode> OnQuestNodeSelected;
        
        public QuestNodeBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }

        public override void OnGUI()
        {

        }
        
        public override object Clone()
        {
            return new QuestNodeBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
        }
        
        public void DataChanged(QuestNode node) {OnQuestNodeSelected?.Invoke(node);}
    }
}