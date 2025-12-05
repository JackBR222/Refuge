using UnityEngine;

public class InactiveCharacterCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMove playerMove = collision.GetComponent<PlayerMove>();
        NPCFollower npcFollower = collision.GetComponent<NPCFollower>();

        if (playerMove != null && !playerMove.enabled && npcFollower != null && npcFollower.IsFollowing)
        {
            npcFollower.StopFollowing();

            if (playerMove.GetInputDirection() == Vector2.zero && playerMove.GetComponent<Rigidbody2D>().linearVelocity == Vector2.zero)
            {
                playerMove.canMove = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerMove playerMove = collision.GetComponent<PlayerMove>();
        NPCFollower npcFollower = collision.GetComponent<NPCFollower>();

        if (playerMove != null && !playerMove.enabled && npcFollower != null && !npcFollower.IsFollowing)
        {
            if (npcFollower.enabled)
            {
                npcFollower.StartFollowing();
            }

            playerMove.canMove = true;
        }
    }
}