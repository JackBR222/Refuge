using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NPCFollower : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Configurações de movimento")]
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float followDistanceThreshold = 5f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Evento quando estiver longe demais")]
    [SerializeField] private float tooFarDistance = 10f;
    public UnityEvent OnTooFarFromPlayer;

    [Header("Referências do Rigidbody e Animator")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Collider2D npcCollider;
    private Vector2 directionToPlayer;
    private float speed;

    private bool shouldFollow = false;

    public bool IsFollowing => shouldFollow;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        npcCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (shouldFollow)
        {
            FollowPlayer();
        }

        UpdateAnimator();
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > tooFarDistance)
        {
            OnTooFarFromPlayer?.Invoke();
        }

        if (distanceToPlayer > followDistanceThreshold)
        {
            speed = runSpeed;
        }
        else
        {
            speed = followSpeed;
        }

        if (distanceToPlayer > stoppingDistance)
        {
            directionToPlayer = (player.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;

        animator.SetFloat("Speed", currentSpeed);

        if (currentSpeed > 0)
        {
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveX", directionToPlayer.x);
            animator.SetFloat("MoveY", directionToPlayer.y);
            animator.SetBool("IsRunning", speed == runSpeed);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsRunning", false);
        }
    }

    public void StartFollowing()
    {
        shouldFollow = true;
        DisableCollider();
    }

    public void StopFollowing()
    {
        shouldFollow = false;
        rb.linearVelocity = Vector2.zero;
        EnableCollider();
    }

    private void DisableCollider()
    {
        if (npcCollider != null)
        {
            npcCollider.enabled = false;
        }
    }

    private void EnableCollider()
    {
        if (npcCollider != null)
        {
            npcCollider.enabled = true;
        }
    }
}