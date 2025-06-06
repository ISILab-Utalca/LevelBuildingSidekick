using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Stealth : NodeEditor
    {
        private ListView observerList;
        private Vector2Field requiredPosition;

        
        public QuestNode_Stealth()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Stealth");
            visualTree.CloneTree(this);
            
            requiredPosition = this.Q<Vector2Field>("RequiredPosition");
            observerList = this.Q<ListView>("ObserverList");

            if (observerList == null) return;
            
            observerList.makeItem = () =>
            {
                var tilePicker = new VeQuestTilePicker();
                tilePicker.SetInfo("Observer target", true);
                return tilePicker;
            };

            observerList.bindItem = (element, i) =>
            {
                // Optionally bind data here if needed later
            };

            // Provide a dummy list to force the ListView to draw items
            observerList.itemsSource = new List<object> {};
            observerList.Rebuild();
            
        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}