using ISILab.AI.Grammar;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestNodeBehaviour : LBSBehaviour
    {
        [JsonIgnore]
        public GrammarTerminal ToSet { get; set; }

        public QuestGraph Graph => OwnerLayer.GetModule<QuestGraph>();
        public QuestNode SelectedQuestNode { get; set; }

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
    }
}