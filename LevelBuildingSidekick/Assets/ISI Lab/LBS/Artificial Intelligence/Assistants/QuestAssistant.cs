using System;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

namespace ISILab.LBS.Assistants
{
    [Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class QuestAssistant : LBSAssistant
    {
        public QuestAssistant() : base(null,null,Color.black)
        {}
        
        [JsonIgnore]
        public QuestGraph _questGraph => OwnerLayer.GetModule<QuestGraph>();
        
        public QuestAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }

        public override object Clone()
        {
            return new QuestAssistant(Icon, Name, ColorTint);
        }
        
        public override void OnAttachLayer(LBSLayer layer)
        {
            base.OnAttachLayer(layer);
        }

        public override void OnGUI() { }


    }
}