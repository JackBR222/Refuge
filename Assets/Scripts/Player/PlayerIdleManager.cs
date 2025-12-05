using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Sit Timing")]
    [SerializeField] private float timeToSitMin = 3f;
    [SerializeField] private float timeToSitMax = 5f;

    [SerializeField] private bool canSit = true;

    private float idleTimer = 0f;
    private float currentTimeToSit;

    private int currentIdlePhase = 0;
    private bool isMoving = false;

    private PlayerMove playerMove;
    private InputAction moveAction;

    [SerializeField] private string checkTag = "NoIdleAnim";
    private bool isOnSpecialObject = false;

    [SerializeField] private LayerMask excludedLayerMask;
    private bool isInExcludedLayer = false;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();

        if (InputManager.Instance == null || InputManager.Instance.playerInput == null)
        {
            Debug.LogError("InputManager ou PlayerInput não encontrado na cena!");
            return;
        }

        moveAction = InputManager.Instance.playerInput.actions["Move"];

        if (animator == null)
            animator = GetComponent<Animator>();

        GenerateRandomIdleTimes();
    }

    private void Update()
    {
        if (moveAction == null) return;

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

            if (idleTimer >= currentTimeToSit && currentIdlePhase < 1 && canSit && !isOnSpecialObject && !isInExcludedLayer)
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

    private bool IsInExcludedLayer(Collider2D collider)
    {
        return (excludedLayerMask == (excludedLayerMask | (1 << collider.gameObject.layer)));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(checkTag) && !IsInExcludedLayer(collision.collider))
        {
            isOnSpecialObject = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(checkTag))
        {
            isOnSpecialObject = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(checkTag) && !IsInExcludedLayer(collider))
        {
            isOnSpecialObject = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(checkTag))
        {
            isOnSpecialObject = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(checkTag))
        {
            isInExcludedLayer = IsInExcludedLayer(collision.collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag(checkTag))
        {
            isInExcludedLayer = IsInExcludedLayer(collider);
        }
    }
}