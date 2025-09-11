using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS
{
    public static class QuestTagRegistry
    {
        #region FIELDS
        private static readonly Dictionary<string, Type> TagDataTypes = new();
        #endregion

        #region CONSTRUCTOR
        static QuestTagRegistry()
        {
            var triggerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypesSafely())
                .Where(t => typeof(QuestTrigger).IsAssignableFrom(t) && !t.IsAbstract && t.HasQuestNodeActionTag());

            foreach (var type in triggerTypes)
            {
                RegisterTriggerType(type);
            }
        }
        #endregion

        #region METHODS
        private static IEnumerable<Type> GetTypesSafely(this System.Reflection.Assembly assembly)
        {
            // Safely retrieves types from an assembly, handling potential reflection errors
            try { return assembly.GetTypes(); }
            catch { return Array.Empty<Type>(); }
        }

        private static bool HasQuestNodeActionTag(this Type type)
        {
            // Checks if a type has the QuestNodeActionTag attribute
            return type.GetCustomAttributes(typeof(QuestNodeActionTag), false).Length > 0;
        }

        private static void RegisterTriggerType(Type type)
        {
            // Registers trigger types by their tags
            // Note: Warns about duplicate tags to prevent conflicts
            var attributes = type.GetCustomAttributes(typeof(QuestNodeActionTag), false)
                .Cast<QuestNodeActionTag>();

            foreach (var attr in attributes)
            {
                var tag = attr.Tag;
                if (!TagDataTypes.TryGetValue(tag, out var dataType))
                {
                    TagDataTypes.Add(tag, type);
                }
                else
                {
                    Debug.LogWarning($"Duplicate tag '{tag}' found on {type.Name} and {dataType.Name}.");
                }
            }
        }

        public static Type GetTriggerTypeForTag(string tag)
        {
            // Retrieves the trigger type for a given tag
            return TagDataTypes.GetValueOrDefault(tag.ToLowerInvariant().Trim());
        }
        #endregion
    }
}