using System.Collections;
using UnityEngine;


namespace ISI_Lab.LBS.Gameplay
{
    
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    public class SpikesController : MonoBehaviour
    {
        Collider spikeCollider;
        Animator animator;

        enum InitialSpikeState
        {
            SpikesOn,
            SpikesOff
        }
        
        [SerializeField] InitialSpikeState initialSpikeState;
        
        [SerializeField]
        float activeWaitTime = 1.0f;
        
        [SerializeField]
        float inactiveWaitTime = 1.0f;
        
        void Start()
        {
            spikeCollider = GetComponent<BoxCollider>();
            animator = GetComponent<Animator>();
            
            if (initialSpikeState == InitialSpikeState.SpikesOn)
            {
                animator.Play("Spikes_Hide");
            } 
            else if (initialSpikeState == InitialSpikeState.SpikesOff)
            {
                animator.Play("Spikes_Show");
            }
        }
        
        public void OnAnimationHideEvent()
        {
            spikeCollider.enabled = false;
            StartCoroutine(
            WaitAndPlayAnimation(inactiveWaitTime, "Spikes_Show")
                );
        }
        
        
        public void OnAnimationShowEvent()
        {
            spikeCollider.enabled = true;
            StartCoroutine(
            WaitAndPlayAnimation(activeWaitTime, "Spikes_Hide")
            );
        }

        IEnumerator WaitAndPlayAnimation(float _waitTime, string _animationName = "")
        {
            yield return new WaitForSeconds(_waitTime);
            animator.Play(_animationName);
        }
    }
}



