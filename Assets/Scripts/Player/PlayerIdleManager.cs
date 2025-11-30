using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Sit Timing")]
    [SerializeField] private float timeToSitMin = 3f;
    [SerializeField] private float timeToSitMax = 5f;

    [SerializeField] private bool canSit = true;

    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInput;

    private float idleTimer = 0f;
    private float currentTimeToSit;

    private int currentIdlePhase = 0;
    private bool isMoving = false;

    private PlayerMove playerMove;
    private InputAction moveAction;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput não atribuído! Atribua o PlayerInput manualmente no Inspector.");
            return;
        }

        moveAction = playerInput.actions["Move"];

        if (animator == null)
            animator = GetComponent<Animator>();

        GenerateRandomIdleTimes();
    }

    private void Update()
    {
        if (playerInput == null)
            return;

        Vector2 currentInput = moveAction.ReadValue<Vector2>();
        isMoving = playerMove.canMove && currentInput.magnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            ResetIdle();
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= currentTimeToSit && currentIdlePhase < 1 && canSit)
            {
                currentIdlePhase = 1;
                animator.SetInteger("IdlePhase", 1);
            }
        }
    }

    private void ResetIdle()
    {
        idleTimer = 0f;
        currentIdlePhase = 0;

        GenerateRandomIdleTimes();

        animator.SetInteger("IdlePhase", 0);
        animator.ResetTrigger("ResetIdle");
    }

    private void GenerateRandomIdleTimes()
    {
        currentTimeToSit = Random.Range(timeToSitMin, timeToSitMax);
    }

    public void SetCanSit(bool value)
    {
        canSit = value;
        animator.SetBool("CanSit", value);
    }
}