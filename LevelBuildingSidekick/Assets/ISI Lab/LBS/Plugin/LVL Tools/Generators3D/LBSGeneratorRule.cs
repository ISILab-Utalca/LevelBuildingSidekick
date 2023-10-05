using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Generator
{
    [System.Serializable]
    public abstract class LBSGeneratorRule : ICloneable
    {
        [JsonIgnore, SerializeField]
        internal Generator3D generator3D;

        public LBSGeneratorRule() { }

        public abstract GameObject Generate(LBSLayer layer, Generator3D.Settings settings);

        public abstract List<Message> CheckViability(LBSLayer layer);

        public abstract object Clone();
    }
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