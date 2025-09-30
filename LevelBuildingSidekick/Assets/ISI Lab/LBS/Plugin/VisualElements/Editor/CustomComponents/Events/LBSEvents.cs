using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.CustomComponents.Events
{
    
    
    public class LBSBoolEvent: EventBase<LBSBoolEvent>
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


