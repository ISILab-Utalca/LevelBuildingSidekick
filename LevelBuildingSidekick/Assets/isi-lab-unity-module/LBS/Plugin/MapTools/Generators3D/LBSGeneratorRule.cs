using System;
using System.Collections;
using System.Collections.Generic;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Generators
{
    [System.Serializable]
    public abstract class LBSGeneratorRule : ICloneable
    {
        [JsonIgnore, SerializeField]
        internal Generator3D generator3D;

        public LBSGeneratorRule() { }

        /// <summary>
        /// Generate the GameObject for the layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="settings"></param>
        /// <returns>returns a tuple of the generated game object containing all the content, as well as a
        /// string in case the game object is invalid(null)</returns>
        public abstract Tuple<GameObject, string> Generate(LBSLayer layer, Generator3D.Settings settings);

        /// <summary>
        /// Check if the layer is viable to be generated
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public abstract List<Message> CheckViability(LBSLayer layer);

        /// <summary>
        /// Clone this object to obtain a new instance of this object
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }

    public class Message
    {
        public enum Type
        {
            Error,
            Warning,
            Info
        }

        public Type type;
        public string msg;

        public Message(Type type, string msg)
        {
            this.type = type;
            this.msg = msg;
        }
    }
}
