using UnityEngine;

public class ResetPushableBlock : MonoBehaviour
{
    [SerializeField] private Vector2 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }

    public void ResetToPosition(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }
}