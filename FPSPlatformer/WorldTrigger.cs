using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldTrigger : MonoBehaviour
{
    public bool isReset = false;

    public UnityEvent OnTriggerEntered;
    public UnityEvent OnTriggerExited;
    public UnityEvent OnTriggerStayed;

    private void OnTriggerEnter(Collider other)
    {
        if (OnTriggerEntered.GetPersistentEventCount() > 0)
        {
            OnTriggerEntered.Invoke();
        }

        if (isReset)
            ResetTriggerManager.instance.SetPlayerPosition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnTriggerExited.GetPersistentEventCount() > 0)
            OnTriggerExited.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnTriggerExited.GetPersistentEventCount() > 0)
            OnTriggerStayed.Invoke();
    }
}
