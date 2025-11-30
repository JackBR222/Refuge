using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInput;  // Agora podemos configurar manualmente o PlayerInput

    private Animator animator;

    private InputAction moveAction;
    private InputAction sprintAction;

    private Vector2 rawInput;
    private Vector2 lastDirection = Vector2.down;

    public bool canMove = true;
    public Vector2 LastDirection => lastDirection;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Certifique-se de que o PlayerInput foi atribuído no Inspector
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput não atribuído! Atribua o PlayerInput manualmente no Inspector.");
            return;
        }

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];

        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild == null)
        {
            Debug.LogError("Objeto filho 'Sprite' não encontrado!");
        }
        else
        {
            animator = spriteChild.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator não encontrado no objeto filho 'Sprite'.");
            }
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            StopMovement();
            return;
        }

        if (Time.timeScale == 0f)
        {
            StopMovement();
            return;
        }

        rawInput = moveAction.ReadValue<Vector2>().normalized;
        bool isRunning = sprintAction != null && sprintAction.IsPressed();

        UpdateAnimator(isRunning);
    }

    private void FixedUpdate()
    {
        if (!canMove || Time.timeScale == 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        bool isRunning = sprintAction != null && sprintAction.IsPressed();
        float speed = isRunning ? runSpeed : walkSpeed;

        rb.linearVelocity = rawInput * speed;
    }

    private void UpdateAnimator(bool isRunning)
    {
        if (animator == null) return;

        float speed = rawInput.sqrMagnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", isRunning);

        if (rawInput != Vector2.zero)
        {
            lastDirection = rawInput;

            animator.SetFloat("MoveX", rawInput.x);
            animator.SetFloat("MoveY", rawInput.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
            animator.SetBool("IsMoving", false);
        }
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;

        if (animator == null) return;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsRunning", false);
        animator.SetFloat("Speed", 0f);
    }

    public Vector2 GetInputDirection()
    {
        return rawInput;
    }
}
