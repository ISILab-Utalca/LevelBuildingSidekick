using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents.Events
{
    
    
    
    // Suppose to use it instead of public event<Action> but more complex and debuggable 
    // Recommendation: wrap a current event in a event Action<EventBase<T> an send it as action?.Invoke(_evt)>
    public class LBSBoolEvent: ChangeEvent<LBSBoolEvent>
    {
        public bool value;

        public LBSBoolEvent(IEventHandler _target, bool _value)
        {
            this.target = _target;
            this.value = _value;
        }
        public LBSBoolEvent(bool _value): base()
        {
            value = _value;
        }
        public LBSBoolEvent()
        {
            
        }
    }
}


