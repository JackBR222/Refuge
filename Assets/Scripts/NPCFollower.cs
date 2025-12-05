using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NPCFollower : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Configurações do seguidor")]
    [SerializeField] private bool canFollow = true;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float followDistanceThreshold = 5f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float colliderDisableMargin = 1.5f;

    [Header("Evento quando estiver longe demais")]
    [SerializeField] private float tooFarDistance = 10f;
    public UnityEvent OnTooFarFromPlayer;

    [Header("Referências do Rigidbody e Animator")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Collider2D npcCollider;
    private CircleCollider2D talkCollider;
    private Vector2 directionToPlayer;
    private float speed;

    private bool shouldFollow = false;
    private float currentSpeedMultiplier = 1f;

    [SerializeField] private string npcLayer = "NPC";

    public bool IsFollowing => shouldFollow;

    [SerializeField] private bool shouldPauseForSpecificPlayers = true;
    [SerializeField] private float specificPlayerPauseDistance = 8f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        npcCollider = GetComponent<Collider2D>();
        talkCollider = GetComponentInChildren<CircleCollider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        IgnoreCollisionsWithNPCs();
        IgnorePlayerCollisionOnlyForBox();
    }

    private void IgnoreCollisionsWithNPCs()
    {
        int npcLayerIndex = LayerMask.NameToLayer(npcLayer);
        if (npcLayerIndex != -1)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, npcLayerIndex, true);
        }
    }

    private void IgnorePlayerCollisionOnlyForBox()
    {
        if (npcCollider is BoxCollider2D)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var p in players)
            {
                Collider2D pc = p.GetComponent<Collider2D>();
                if (pc != null)
                    Physics2D.IgnoreCollision(npcCollider, pc, true);
            }
        }
    }

    private void Update()
    {
        if (shouldFollow && canFollow)
            FollowPlayer();

        UpdateAnimator();

        if (shouldPauseForSpecificPlayers)
            HandlePlayerPause();
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > tooFarDistance)
            OnTooFarFromPlayer?.Invoke();

        speed = (distanceToPlayer > followDistanceThreshold) ? runSpeed : followSpeed;
        speed *= currentSpeedMultiplier;

        if (distanceToPlayer > stoppingDistance)
        {
            directionToPlayer = (player.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            EnableCollider();
        }

        HandleCollider(distanceToPlayer);
    }

    private void HandleCollider(float distanceToPlayer)
    {
        if (npcCollider == null) return;

        if (IsCollidingWithPressureButton())
        {
            return;
        }

        if (distanceToPlayer > followDistanceThreshold + colliderDisableMargin)
        {
            DisableCollider();
        }
        else
        {
            if (!npcCollider.enabled && !IsCollidingWithAnything())
                EnableCollider();
        }
    }

    private bool IsCollidingWithAnything()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f) != null;
    }

    private bool IsCollidingWithPressureButton()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npcCollider.bounds.extents.x);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<PressureButton>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;

        animator.SetFloat("Speed", currentSpeed);

        if (currentSpeed > 0f)
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

        if (player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * followSpeed;
        }
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
            npcCollider.enabled = false;
    }

    private void EnableCollider()
    {
        if (npcCollider != null)
            npcCollider.enabled = true;
    }

    public void SetCanFollow(bool value)
    {
        canFollow = value;
        if (canFollow && shouldFollow)
            StartFollowing();
    }

    public void ModifySpeed(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }

    public void ResetSpeed()
    {
        currentSpeedMultiplier = 1f;
    }

    private void HandlePlayerPause()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        players = players.Concat(GameObject.FindGameObjectsWithTag("Player2")).ToArray();

        foreach (var p in players)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, p.transform.position);

            PlayerMove playerMove = p.GetComponent<PlayerMove>();

            if (playerMove == null || !playerMove.enabled)
            {
                Rigidbody2D playerRb = p.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    if (distanceToPlayer > specificPlayerPauseDistance)
                    {
                        if (playerRb.bodyType != RigidbodyType2D.Static)
                        {
                            playerRb.bodyType = RigidbodyType2D.Static;
                        }
                    }
                    else
                    {
                        if (playerRb.bodyType != RigidbodyType2D.Dynamic)
                        {
                            playerRb.bodyType = RigidbodyType2D.Dynamic;
                        }
                    }
                }
            }

            if (p.CompareTag("Player") || p.CompareTag("Player2"))
            {
                Collider2D playerCollider = p.GetComponent<Collider2D>();
                if (playerCollider != null && !playerCollider.enabled)
                {
                    playerCollider.enabled = true;
                }
            }
        }
    }
}