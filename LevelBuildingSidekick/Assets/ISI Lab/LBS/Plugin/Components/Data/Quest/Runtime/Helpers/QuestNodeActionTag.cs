using System;

namespace ISILab.LBS
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class QuestNodeActionTag : Attribute
    {
        public string Tag { get; }
        public QuestNodeActionTag(string tag) => Tag = tag.ToLowerInvariant().Trim();
        
    }
    
}