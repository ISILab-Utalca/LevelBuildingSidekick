using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
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
            public static readonly Dictionary<string, Type[]> TagDataTypes = new Dictionary<string, Type[]>
            {
                { "go to", new[] { typeof(DataPosition) } },
                { "explore", new[] { typeof(DataPosition) } },
                { "kill", new[] { typeof(DataBundle) } },
                { "steal", new[] { typeof(DataBundle) } },
                { "take", new[] { typeof(DataBundle) } },
                { "read", new[] { typeof(DataBundle) } },
                { "report", new[] { typeof(DataBundle) } },
                { "gather", new[] { typeof(DataBundle) } },
                { "spy", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { "capture", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { "listen", new[] { typeof(DataPosition), typeof(DataConstrain) } },
                { "empty", new Type[0] }
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
//                    Debug.LogError($"No data types defined for tag: '{tag}'");
                    return new BaseQuestNodeData(owner, tag, new Type[0]);
                }

                var nodeData = new BaseQuestNodeData(owner, tag, requiredDataTypes);

                // Initialize data fields based on required types
                foreach (var dataType in requiredDataTypes)
                {
                    if (dataType == typeof(DataPosition))
                        nodeData.SetPosition(new DataPosition());
                    else if (dataType == typeof(DataBundle))
                        nodeData.SetBundle(new DataBundle());
                    else if (dataType == typeof(DataConstrain))
                        nodeData.SetConstrain(new DataConstrain());
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

            [SerializeField, JsonRequired]
            private int _num = 1;

            [SerializeField]
            private DataPosition _position;

            [SerializeField]
            private DataBundle _bundle;

            [SerializeField]
            private DataConstrain _constrain;

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
            public int Num
            {
                get => _num;
                set => _num = value;
            }
            public DataPosition Position => _position;
            public DataBundle Bundle => _bundle;
            public DataConstrain Constrain => _constrain;
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
                            _position.position = positionGoal;
                            _goal = positionGoal;
                        }
                    }
                    else if (requiredDataTypes.Contains(typeof(DataBundle)) && goal is string bundleGoal)
                    {
                        if (HasBundle())
                        {
                            _bundle.bundleGuid = bundleGoal;
                            _goal = bundleGoal;
                        }
                    }
                    else if (requiredDataTypes.Contains(typeof(DataPosition)) && requiredDataTypes.Contains(typeof(DataConstrain)) && goal is Tuple<DataPosition, DataConstrain> stayGoal)
                    {
                        if (HasPosition() && HasConstraint())
                        {
                            _position = stayGoal.Item1;
                            _constrain = stayGoal.Item2;
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

            public void SetNum(int num) => _num = num;

            public void SetPosition(DataPosition position) => _position = position;
            public void SetBundle(DataBundle bundle) => _bundle = bundle;
            public void SetConstrain(DataConstrain constrain) => _constrain = constrain;

            public bool HasPosition() => _position != null;
            public bool HasBundle() => _bundle != null;
            public bool HasConstraint() => _constrain != null;
        }

        #region DATA CONTAINERS
        [Serializable]
        public class DataPosition
        {
            [SerializeField, JsonRequired]
            public Vector2Int position;
            [SerializeField, JsonRequired]
            public float size = 1;
        }

        [Serializable]
        public class DataBundle
        {
            [SerializeField, JsonRequired]
            public string bundleGuid = "";
        }

        [Serializable]
        public class DataConstrain
        {
            [SerializeField, JsonRequired]
            public float time = 1;
            [SerializeField, JsonRequired]
            public float maxDistance = 1;
        }       
        #endregion
    }
    /* OLD VERSION
    /// <summary>
    /// Custom attribute to get a BaseQuestNodeData child class corresponding to an
    /// attribute target class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false,AllowMultiple = true)]
    public class QuestNodeActionTag: Attribute
    {
        private readonly string _tag;
        public string Tag => _tag;

        public QuestNodeActionTag(string tag)
        {
            _tag = tag;
        }
    }

    /// <summary>
    /// Class that generates a corresponding QuestNodeData depending on the action string 
    /// </summary>
    public static class QuestNodeDataFactory
    {
        public static BaseQuestNodeData CreateByTag(string tag, QuestNode owner)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t =>
                    typeof(BaseQuestNodeData).IsAssignableFrom(t) &&
                    t.GetCustomAttributes(typeof(QuestNodeActionTag), false)
                        .Cast<QuestNodeActionTag>()
                        .Any(attr => attr.Tag == tag));

            if (type != null)
            {
                // The class with the "QuestNodeActionTag" that corresponds to the action
//                return Activator.CreateInstance(type, owner) as BaseQuestNodeData ?? ScriptableObject.CreateInstance<QuestNodeDataEmpty>();
                return Activator.CreateInstance(type, owner) as BaseQuestNodeData ?? new QuestNodeDataEmpty(owner);

            }
            Debug.LogError("Failed to find a QuestNodeData with the NodeActionTag:\n" + "'" + tag +"'");

            // Fallback case: return a default implementation
            
            //return ScriptableObject.CreateInstance<QuestNodeDataEmpty>();
            return  new QuestNodeDataEmpty(owner);
        }
    }

    [Serializable]
    public class BaseQuestNodeData : object 
    {
        #region FIELDS
        [SerializeField][JsonRequired]
        private QuestNode _owner;
        /// <summary>
        /// This object can ideally be a bundle reference or a position
        /// or both in a tuple or a struct
        /// </summary>
        [SerializeReference][JsonRequired]
        private object _goal;
        /// <summary>
        /// By default, is one, but for cases such as
        /// collection or kill of many enemies it can be set by initialize
        /// </summary>
        [SerializeField][JsonRequired]
        private int _num = 1;
        #endregion
        
        #region PROPERTIES

        public bool isValid()
        {
            return _goal is not null;
        }

        public QuestNode Owner => _owner;

        public object Goal
        {
            get => _goal;
            set => _goal = value;
        }

        public int Num 
        { 
            get => _num; 
            set => _num = value; 
        }

        #endregion

        
        protected BaseQuestNodeData(QuestNode owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public virtual GameObject Generation()
        {
            Debug.LogError("Base quest node child generation not implemented");
            return null;
        }
        
        public virtual T GetGoal<T>()
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
        
        // Always call after saving whatever data is set within the overwritten version
        public virtual void SetGoal<T>(object goal)
        {
            try
            {
                _goal = (T)goal;
                _owner.SaveNodeAsJson();
            }
            catch (InvalidCastException)
            {
                Debug.LogError($"[QuestNode] Goal object can't be set to expected type: {typeof(T).Name}");
            }
        }
        
        public void SetNum(int num){_num = num;} 

    }
    
    #region DATA CONTAINERS
    
    [Serializable]
    public class DataPosition : object 
    {
        [SerializeField][JsonRequired]
        public Vector2Int position;
        [SerializeField][JsonRequired]
        public float size = 1; // for the box trigger
    }
    
    [Serializable]
    public class DataBundle : object 
    {
        [SerializeField][JsonRequired]
        public string bundleGuid = "";
    }
    
    [Serializable]
    public class DataConstrain : object 
    {
        [SerializeField][JsonRequired]
        public float time = 1; // by default its 1, so it must keep position for 1 second
        [SerializeField][JsonRequired]
        public float maxDistance = 1; // by default is 0, just being within the area is enoughr

    }
    
    #endregion
    
    #region CHILD CLASSES

    /// <summary>
    /// Should never be used
    /// </summary>
    public class QuestNodeDataEmpty : BaseQuestNodeData
    {
        public QuestNodeDataEmpty(QuestNode owner) : base(owner)
        {
            Debug.LogError("QuestNodeDataEmpty Created!");
        }
    }

    
    /// <summary>
    /// Uses Vector2Int as object
    /// </summary>
    [QuestNodeActionTag(" go to ")]
    [QuestNodeActionTag(" explore ")]
    [Serializable]
    public class QuestNodeDataPosition : BaseQuestNodeData
    {
        [SerializeField]
        public DataPosition _dataPosition = new();
        public QuestNodeDataPosition(QuestNode owner) : base(owner) {}

        public override void SetGoal<T>(object goal)
        {
            if (goal is not Vector2Int locationGoal) return;
            _dataPosition.position = locationGoal;
            base.SetGoal<Vector2Int>(goal);
        }
    }
    
    /// <summary>
    /// Uses Tuple(Bundle, Vector2Int) as object
    /// </summary>
    [QuestNodeActionTag(" kill ")]
    [QuestNodeActionTag(" steal ")]
    [QuestNodeActionTag(" take ")]
    [QuestNodeActionTag(" read ")]
    [QuestNodeActionTag(" report ")]
    [QuestNodeActionTag(" gather ")]
    [Serializable]
    public class QuestNodeDataBundle : BaseQuestNodeData
    {
        [SerializeField] 
        public DataBundle _dataBundle = new();
        public QuestNodeDataBundle(QuestNode owner) : base(owner){}
    
        public override void SetGoal<T>(object goal)
        {
            if (goal is not string killGoal) return;
            _dataBundle.bundleGuid = killGoal;
            base.SetGoal<string>(killGoal);
        }
    }
    
    /// <summary>
    /// Uses Tuple(Bundle, Vector2Int) as object
    /// </summary>
    [QuestNodeActionTag(" spy ")]
    [QuestNodeActionTag(" capture ")]
    [QuestNodeActionTag(" listen ")]
    [Serializable]
    public class QuestNodeDataStayPosition : BaseQuestNodeData
    {
        [SerializeField] 
        public DataPosition _dataPosition = new();
        [SerializeField] 
        public DataConstrain _dataConstrain = new();
        
        public QuestNodeDataStayPosition(QuestNode owner) : base(owner) {}

        public override void SetGoal<T>(object goal)
        {
            if (goal is not Tuple<DataPosition, DataConstrain> stayGoal) return;
            _dataPosition = stayGoal.Item1;
            _dataConstrain = stayGoal.Item2;
            base.SetGoal<Tuple<DataPosition, DataConstrain>>(stayGoal);

        }
    }
    
    
    
    
    #endregion
    */
