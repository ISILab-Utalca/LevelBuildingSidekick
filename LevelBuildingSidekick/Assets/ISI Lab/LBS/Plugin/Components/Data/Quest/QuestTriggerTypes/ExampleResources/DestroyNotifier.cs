using UnityEngine;
using System;

namespace ISILab.LBS
{
    public class DestroyNotifier : MonoBehaviour
    {
        // Event to notify when this GameObject is destroyed used by multiple trigger quest check on complete
        public event Action<GameObject> OnDestroyed;

        private void OnDestroy()
        {
            // Notify subscribers that this GameObject is destroyed
            OnDestroyed?.Invoke(gameObject);
        }
    }
}