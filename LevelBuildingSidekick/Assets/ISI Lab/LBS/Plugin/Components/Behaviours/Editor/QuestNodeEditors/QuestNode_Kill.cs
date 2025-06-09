using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace ISILab.LBS.VisualElements
{
        public class QuestNode_Kill : NodeEditor
        {
                private ListView KillList;
                
                public QuestNode_Kill()
                {
                        Clear();

                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Kill");
                        visualTree.CloneTree(this);

                        KillList = this.Q<ListView>("KillList");

                        if (KillList == null) return;
                        KillList.makeItem = () =>
                        {
                                var tilePicker = new VeQuestTilePicker();
                                tilePicker.SetInfo(
                                        "Kill target", 
                                        "The objects in the graph that the player must kill to complete this action node.",
                                        true);
                                return tilePicker;
                        };

                        KillList.bindItem = (element, i) =>
                        {
                                // Optionally bind data here if needed later
                        };

                        // Provide a dummy list to force the ListView to draw items
                        KillList.itemsSource = new List<object> {};
                        KillList.Rebuild();
                        
                }

                public override void SetMyData(BaseQuestNodeData data)
                {
              
                }
        }
}