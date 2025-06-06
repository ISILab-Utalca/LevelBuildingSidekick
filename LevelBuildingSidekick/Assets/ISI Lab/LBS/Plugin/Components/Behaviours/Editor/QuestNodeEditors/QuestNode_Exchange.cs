using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Exchange : NodeEditor
    {
        private VeQuestTilePicker pickerGive;
        private IntegerField giveAmount;
        private VeQuestTilePicker pickerReceive;
        private IntegerField receiveAmount;
        
        public QuestNode_Exchange()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Exchange");
            visualTree.CloneTree(this);
            
            pickerGive = this.Q<VeQuestTilePicker>("ExchangeGiveTarget");
            pickerGive.SetInfo("Object to give", false); 
            giveAmount = this.Q<IntegerField>("ExchangeGiveAmount");
            
            pickerReceive = this.Q<VeQuestTilePicker>("ExchangeReceiveTarget");
            pickerReceive.SetInfo("Object to receive", false); 
            receiveAmount = this.Q<IntegerField>("ExchangeReceiveAmount");
            
 
        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}