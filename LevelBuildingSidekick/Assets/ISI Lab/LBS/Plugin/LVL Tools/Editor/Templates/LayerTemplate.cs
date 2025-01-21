using LBS.Components;
using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Template
{
    [Serializable]
    [CreateAssetMenu(menuName = "ISILab/LBS/Layer Template")]
    public class LayerTemplate : ScriptableObject
    {
        [JsonRequired, SerializeField]
        public LBSLayer layer;

        public void Clear()
        {
            layer = new LBSLayer();
        }

        private void OnValidate()
        {
            if (layer == null) return;
            foreach (var behaviour in layer.Behaviours)
            {
                behaviour.OnGUI();
                // invoke
            }

            foreach (var assistant in layer.Assistants)
            {
                // invoke
                assistant.OnGUI();
            }
        }
    }
}