using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class QuestTagAttribute : Attribute
        {
            public string Tag { get; }

            public QuestTagAttribute(string tag)
            {
                Tag = tag;
            }
        }

    
        /// <summary>
        /// Custom attribute to specify action tag and required data types.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        public class QuestNodeActionTag : Attribute
        {
            private readonly string _tag;
            private readonly Type[] _requiredDataTypes;

            public string Tag => _tag;
            public Type[] RequiredDataTypes => _requiredDataTypes;

            public QuestNodeActionTag(string tag, params Type[] requiredDataTypes)
            {
                _tag = tag;
                _requiredDataTypes = requiredDataTypes;
            }

            // Static mapping of tags to required data types
            public static readonly Dictionary<string, Type[]> TagDataTypes = new()
            {
                { " go to ", new[] { typeof(DataPosition) } },
                { " explore ", new[] { typeof(DataPosition) } },
                { " kill ", new[] { typeof(DataBundle) } },
                { " stealth ", new[] { typeof(DataPosition) } },
                { " take ", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { " read ", new[] { typeof(DataBundle) } },
                { " exchange ", new[] { typeof(DataBundle),typeof(DataBundle) } },
                { " give ", new[] { typeof(DataBundle),typeof(DataBundle) } },
                { " report ", new[] { typeof(DataBundle) } },
                { " gather ", new[] { typeof(DataBundle) } },
                { " spy ", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { " capture ", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { " listen ", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { " empty ", new Type[0] }
            };
        }

        /// <summary>
        /// Factory to create QuestNodeData based on tags.
        /// </summary>
        public static class QuestNodeDataFactory
        {
            public static BaseQuestNodeData CreateByTag(string tag, QuestNode owner)
            {
                if (!QuestNodeActionTag.TagDataTypes.TryGetValue(tag, out var requiredDataTypes))
                {
            //        Debug.LogError($"No data types defined for tag: '{tag}'");
                    return new BaseQuestNodeData(owner, tag, new Type[0]);
                }

                var nodeData = new BaseQuestNodeData(owner, tag, requiredDataTypes);

                // Initialize data fields based on required types
                foreach (var dataType in requiredDataTypes)
                {
                    if (dataType == typeof(DataPosition))
                        nodeData.AddPosition(new DataPosition());
                    else if (dataType == typeof(DataBundle))
                        nodeData.AddBundle(new DataBundle());
                    else if (dataType == typeof(DataConstrain))
                        nodeData.AddConstrain(new DataConstrain());
                }

                return nodeData;
            }
        }

        [Serializable]
        public class BaseQuestNodeData
        {
            #region FIELDS
            [SerializeField, JsonRequired]
            private QuestNode _owner;

            [SerializeField, SerializeReference, JsonRequired]
            private object _goal;

            [SerializeField]
            private List<DataPosition> _position = new();

            [SerializeField]
            private List<DataBundle> _bundle = new();

            [SerializeField]
            private List<DataConstrain> _constrain = new();

            [SerializeField, JsonRequired]
            private string _tag;
            #endregion

            #region PROPERTIES
            public QuestNode Owner => _owner;
            public string Tag => _tag;
            public object Goal
            {
                get => _goal;
                set => _goal = value;
            }
            
            public List<DataPosition> Position => _position;
            public List<DataBundle> Bundle => _bundle;
            public List<DataConstrain> Constrain => _constrain;
            #endregion

            public BaseQuestNodeData(QuestNode owner, string tag, Type[] requiredDataTypes)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _tag = tag;
            }

            public bool IsValid() => _goal != null;

            public virtual GameObject Generation()
            {
                Debug.LogError($"Generation not implemented for tag: {_tag}");
                return null;
            }

            public T GetGoal<T>()
            {
                try
                {
                    return (T)_goal;
                }
                catch (InvalidCastException)
                {
                    Debug.LogError($"[QuestNode] Goal object is not of expected type: {typeof(T).Name}");
                    return default;
                }
            }

            public void SetGoal<T>(object goal)
            {
                if (!QuestNodeActionTag.TagDataTypes.TryGetValue(_tag, out var requiredDataTypes))
                {
                    Debug.LogError($"Tag '{_tag}' not found in configuration.");
                    return;
                }

                try
                {
                    if (requiredDataTypes.Contains(typeof(DataPosition)) && goal is Vector2Int positionGoal)
                    {
                        if (HasPosition())
                        {
                            _position.FirstOrDefault()!.position = positionGoal;
                            _goal = positionGoal;
                        }
                    }
                    else if (requiredDataTypes.Contains(typeof(DataBundle)) && goal is string bundleGoal)
                    {
                        if (HasBundle())
                        {
                            _bundle.FirstOrDefault()!.bundleGuid = bundleGoal;
                            _goal = bundleGoal;
                        }
                    }
                    else if (requiredDataTypes.Contains(typeof(DataConstrain)) && goal is Tuple<int, float> stayGoal)
                    {
                        if (HasPosition() && HasConstraint())
                        {
                            _constrain.FirstOrDefault()!.areaSize = stayGoal.Item1;
                            _constrain.FirstOrDefault()!.time = stayGoal.Item2;
                            _goal = stayGoal;
                        }
                    }
                    else
                    {
                        Debug.LogError($"[QuestNode] Invalid goal type for tag '{_tag}': {goal?.GetType().Name}");
                        return;
                    }

                    _owner.SaveNodeAsJson();
                }
                catch (InvalidCastException)
                {
                    Debug.LogError($"[QuestNode] Goal object can't be set to expected type: {typeof(T).Name}");
                }
            }

            public void AddPosition(DataPosition position) => _position.Add(position);
            public void AddBundle(DataBundle bundle) => _bundle.Add(bundle);
            public void AddConstrain(DataConstrain constrain) => _constrain.Add(constrain);

            public bool HasPosition() => _position.Any();
            public bool HasBundle() => _bundle.Any();
            public bool HasConstraint() => _constrain.Any();
        }

        #region DATA CONTAINERS
        
        public class DataContainer{}
        
        [Serializable]
        public class DataPosition : DataContainer
        {
            [SerializeField, JsonRequired]
            public Vector2Int position;
            [SerializeField, JsonRequired]
            public float size = 1;
        }

        [Serializable]
        public class DataBundle: DataContainer
        {
            [SerializeField, JsonRequired]
            public int num = 1;
            [SerializeField, JsonRequired]
            public string bundleGuid = "";
        }

        [Serializable]
        public class DataConstrain: DataContainer
        {
            [SerializeField, JsonRequired]
            public float time = 1;
            [SerializeField, JsonRequired]
            public int areaSize = 1;
        }       
        
        #endregion

} 