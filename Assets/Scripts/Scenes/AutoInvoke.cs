using UnityEngine;
using UnityEngine.Events;

public class EventInvokerScript : MonoBehaviour
{
    public UnityEvent OnEventTriggered;

    public float delay = 1f;

    void Start()
    {
        Invoke("TriggerEvent", delay);
    }

    void TriggerEvent()
    {
        if (OnEventTriggered != null)
        {
            OnEventTriggered.Invoke();
        }
    }
}
