using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject character1;
    public GameObject character2;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private GameObject activeCharacter;

    void Start()
    {
        activeCharacter = character1;
    }

    void FixedUpdate()
    {
        if (activeCharacter == null)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(activeCharacter.transform.position.x, activeCharacter.transform.position.y, transform.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    public void SwitchActiveCharacter(GameObject newActiveCharacter)
    {
        activeCharacter = newActiveCharacter;
    }
}