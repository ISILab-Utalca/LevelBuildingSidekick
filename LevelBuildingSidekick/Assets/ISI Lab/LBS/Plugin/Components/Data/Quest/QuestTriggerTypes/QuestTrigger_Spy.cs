using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("spy")]
    public class QuestTriggerSpy : QuestTrigger
    {
        public DataSpy dataSpy;
        public GameObject objectToSpy;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataSpy);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataSpy = (DataSpy)baseData;
            
            var followTrigger = objectToSpy.AddComponent<FollowTrigger>();
            followTrigger.SetTriggerData(
                objectToSpy,
                BoxCollider, 
                dataSpy.resetTimeOnExit, 
                dataSpy.spyTime, 
                this);
        }
        
    }


    [RequireComponent(typeof(BoxCollider))] // change collider if desired
    public class FollowTrigger : MonoBehaviour
    {
        /// <summary>
        /// Whether time is reset when exiting the trigger
        /// </summary>
        private bool _resetOnExit;
        /// <summary>
        /// The seconds the player has stayed within the follow trigger
        /// </summary>
        private float _withinSeconds;
        /// <summary>
        /// The required seconds to complete the spy action
        /// </summary>
        private float _withinRequiredSeconds;
        private QuestTrigger _questTrigger;
        private BoxCollider _boxCollider;

        public void SetTriggerData(GameObject followObject, Collider paramCollider, bool paramResetOnExit, float paramRequiredSeconds, QuestTrigger questTrigger)
        {
            _resetOnExit = paramResetOnExit;
            _withinRequiredSeconds = paramRequiredSeconds;
            _withinSeconds = 0f;
            _questTrigger = questTrigger;

            // add new collider to track follow
            _boxCollider = followObject.AddComponent<BoxCollider>();
            _boxCollider.isTrigger = true;

            // copy size
            if (paramCollider is BoxCollider source)
            {
                _boxCollider.size = source.size;
                _boxCollider.center = source.center;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_questTrigger.IsPlayer(other)) return;

            _withinSeconds += Time.deltaTime;
            if (_withinSeconds >= _withinRequiredSeconds)
            {
                _questTrigger.CheckComplete();
                enabled = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_questTrigger.IsPlayer(other)) return;
            if (_resetOnExit) _withinSeconds = 0f;

        }
    }

}