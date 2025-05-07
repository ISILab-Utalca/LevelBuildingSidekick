using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    /// <summary>
    /// Custom attribute to get a BaseQuestNodeData child class corresponding to an
    /// attribute target class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
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
        private int _num;
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
    [QuestNodeActionTag(" go to ")][Serializable]
    public class QuestNodeDataLocation : BaseQuestNodeData
    {
        [SerializeField][JsonRequired]
        public Vector2Int position;
        
        public QuestNodeDataLocation(QuestNode owner) : base(owner) {}

        public override void SetGoal<T>(object goal)
        {
            if (goal is not Vector2Int locationGoal) return;
            position = locationGoal;
            base.SetGoal<Vector2Int>(goal);
        }
    }
    
    /// <summary>
    /// Uses Tuple(Bundle, Vector2Int) as object
    /// </summary>
    [QuestNodeActionTag(" kill ")][Serializable]
    public class QuestNodeDataKill : BaseQuestNodeData
    {
        
        [SerializeField][JsonRequired]
        public string bundleGuid = "";

        public QuestNodeDataKill(QuestNode owner) : base(owner)
        {
            Num = 1;
        }
    
        public override void SetGoal<T>(object goal)
        {
            if (goal is not string killGoal) return;
            bundleGuid = killGoal;
            base.SetGoal<string>(killGoal);
        }
    }
    
    /// <summary>
    /// Uses Tuple(Bundle, Vector2Int) as object
    /// </summary>
    [QuestNodeActionTag(" steal ")][Serializable]
    public class QuestNodeDataSteal : BaseQuestNodeData
    {
        
        [SerializeField][JsonRequired]
        public string bundleGuid = "";
        [SerializeField][JsonRequired]
        public Vector2Int position;
        
        public QuestNodeDataSteal(QuestNode owner) : base(owner) {}

        public override void SetGoal<T>(object goal)
        {
            if (goal is not Tuple<string, Vector2Int> stealGoal) return;
            bundleGuid = stealGoal.Item1;
            position = stealGoal.Item2;
            base.SetGoal< Tuple<string, Vector2Int>>(stealGoal);

        }
    }
    
    
    
    
    #endregion

    
}