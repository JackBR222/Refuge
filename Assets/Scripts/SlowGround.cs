using UnityEngine;

public class SlowGround : MonoBehaviour
{
    [Tooltip("0.5 = metade da velocidade; 0.7 = 30% mais lento.")]
    [SerializeField] private float slowMultiplier = 0.5f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.CompareTag("Player2"))
        {
            var player = collider.gameObject.GetComponent<PlayerMove>();
            if (player != null) player.ModifySpeed(slowMultiplier);

            var npcFollower = collider.gameObject.GetComponent<NPCFollower>();
            if (npcFollower != null) npcFollower.ModifySpeed(slowMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.gameObject.CompareTag("Player2"))
        {
            var player = collider.gameObject.GetComponent<PlayerMove>();
            if (player != null) player.ResetSpeed();

            var npcFollower = collider.gameObject.GetComponent<NPCFollower>();
            if (npcFollower != null) npcFollower.ResetSpeed();
        }
    }
}