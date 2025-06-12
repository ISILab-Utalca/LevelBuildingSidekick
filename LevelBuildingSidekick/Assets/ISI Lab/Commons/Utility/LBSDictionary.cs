using System;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace ISI_Lab.Commons.Utility
{
    [Serializable]
    public class LBSDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public LBSDictionary() : base() {}
        
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        [SerializeField]
        private List<TValue> values = new List<TValue>();
	
        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
	
        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if(keys.Count != values.Count)
                throw new System.Exception(
                    $"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }
    }
}
