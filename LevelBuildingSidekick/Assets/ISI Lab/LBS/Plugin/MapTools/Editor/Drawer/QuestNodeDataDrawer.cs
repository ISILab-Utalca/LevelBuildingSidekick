using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Components;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers.Editor
{
    [Drawer(typeof(QuestNodeBehaviour))]
    public class QuestNodeBehaviourDrawer : Drawer
    {
        public QuestNodeBehaviourDrawer() : base() { }
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var behaviour = target as QuestNodeBehaviour;

            var quest = behaviour?.Graph;
            if (quest == null) return;

            var datas = quest.QuestNodes.Select(node => node.NodeData).ToList();
            // When drawing, paint differently depending on the data class
            foreach (var nd in datas)
            {
                if(nd == null) continue;
                Debug.Log("\n --node: " + nd.Owner.ID + "--");
                
                switch (nd)
                {
                    case QuestNodeDataLocation location:
                    {
                        Debug.Log(location.position);
                        break;
                    }
                        
                    case QuestNodeDataKill kill:
                    {
                        Debug.Log(kill.bundleGuid + " -> " + kill.Num);
                        break;
                    }
                
                    case QuestNodeDataSteal steal:
                    {
                      
                        Debug.Log(steal.position);
                        Debug.Log(steal.bundleGuid + " -> " + steal.bundleGuid);
                        break;
                    }
                }
            }
        }
    }
}