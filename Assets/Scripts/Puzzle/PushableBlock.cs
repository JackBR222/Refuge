using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PushableBlock : MonoBehaviour
{
    public float moveSpeed = 3f;
    public LayerMask obstacleLayers;
    public string[] ignoredTags;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private bool isBeingPushed = false;
    private Vector2 pushDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        boxCollider = GetComponent<BoxCollider2D>();
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }

    private void FixedUpdate()
    {
        if (isBeingPushed)
        {
            Vector2 desiredPos = (Vector2)transform.position + pushDirection * moveSpeed * Time.fixedDeltaTime;
            if (CanMoveTo(desiredPos))
                rb.MovePosition(desiredPos);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ShouldIgnoreCollision(collision.gameObject))
        {
            isBeingPushed = false;
            return;
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = false;
            return;
        }

        // VETOR DO JOGADOR ? PARA A CAIXA
        Vector2 fromPlayerToBox = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;

        // Define direção de empurrão pelo eixo dominante
        if (Mathf.Abs(fromPlayerToBox.x) > Mathf.Abs(fromPlayerToBox.y))
            pushDirection = new Vector2(Mathf.Sign(fromPlayerToBox.x), 0);
        else
            pushDirection = new Vector2(0, Mathf.Sign(fromPlayerToBox.y));

        // Checa se o jogador está andando realmente NESSA direção
        PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
        if (player != null)
        {
            Vector2 input = player.GetInputDirection();
            // Jogador deve pressionar em direção À CAIXA (não ao contrário)
            if (Vector2.Dot(input, fromPlayerToBox) > 0.5f)
                isBeingPushed = true;
            else
                isBeingPushed = false;
        }
        else
        {
            isBeingPushed = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isBeingPushed = false;
    }

    private bool CanMoveTo(Vector2 targetPos)
    {
        Vector2 boxSize = boxCollider.size * 0.9f;
        Collider2D hit = Physics2D.OverlapBox(targetPos, boxSize, 0f, obstacleLayers);
        return hit == null;
    }

    private bool ShouldIgnoreCollision(GameObject other)
    {
        if (ignoredTags == null || ignoredTags.Length == 0)
            return false;

        foreach (string tag in ignoredTags)
        {
            if (other.CompareTag(tag))
                return true;
        }
        return false;
    }
}