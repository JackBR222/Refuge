using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    private bool hasTriggered = false;
    public bool isOnceOnly = true;
    public string[] validTags = { "Player" };

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isValidTag = false;
        foreach (string tag in validTags)
        {
            if (other.CompareTag(tag))
            {
                isValidTag = true;
                break;
            }
        }

        if (isValidTag && (isOnceOnly && !hasTriggered || !isOnceOnly))
        {
            onPlayerEnter.Invoke();

            if (isOnceOnly)
                hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool isValidTag = false;
        foreach (string tag in validTags)
        {
            if (other.CompareTag(tag))
            {
                isValidTag = true;
                break;
            }
        }

        if (!isOnceOnly && isValidTag)
        {
            hasTriggered = false;
        }
    }
}
