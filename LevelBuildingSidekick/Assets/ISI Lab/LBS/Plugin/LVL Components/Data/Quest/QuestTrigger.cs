using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class QuestTrigger : MonoBehaviour
{
    BoxCollider boxCollider;

    public Action<GameObject> OnEnterZone;
    public Action<GameObject> OnLeaveZone;

    public Func<bool> IsCompleted;

    public virtual void Init(Vector3 size)
    {
        if (size.x == 0 || size.z == 0)
            return;

        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = size;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEnterZone?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        OnLeaveZone?.Invoke(other.gameObject);
    }
}
