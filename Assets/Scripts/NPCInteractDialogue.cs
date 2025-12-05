using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class NPCInteractDialogue : MonoBehaviour
{
    [Header("Configuração de Interação")]
    [SerializeField] private TextAsset inkFile;
    [SerializeField] private InkDialogueManager dialogueManager;

    [Header("Condição de Direção")]
    [Range(10f, 90f)]
    [SerializeField] private float maxFacingAngle = 35f;

    [Header("Indicador Visual")]
    [SerializeField] private GameObject visualCue;

    [Header("Cooldown de Interação")]
    [SerializeField] private float interactionCooldown = 0.5f;

    private bool playerInRange = false;
    private bool isDialogueActive = false;
    private bool canTalk = true;
    private bool isCooldown = false;

    private InputAction interactAction;
    private Transform playerTransform;
    private PlayerMove playerMove;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[NPCInteractDialogue] Jogador não encontrado!");
            return;
        }

        playerTransform = player.transform;
        playerMove = player.GetComponent<PlayerMove>();

        if (InputManager.Instance == null || InputManager.Instance.playerInput == null)
        {
            Debug.LogError("[NPCInteractDialogue] InputManager ou PlayerInput não encontrado!");
            return;
        }

        interactAction = InputManager.Instance.playerInput.actions["Interact"];
        if (interactAction == null)
            Debug.LogError("[NPCInteractDialogue] Ação 'Interact' não encontrada!");

        if (visualCue != null)
            visualCue.SetActive(false);
    }

    private void Update()
    {
        if (!canTalk || isCooldown)
        {
            if (visualCue != null)
                visualCue.SetActive(false);
            return;
        }

        if (interactAction == null || isDialogueActive || !playerMove.canMove)
        {
            if (visualCue != null)
                visualCue.SetActive(false);
            return;
        }

        bool canInteract = playerInRange && IsPlayerFacingNPC();

        if (visualCue != null)
            visualCue.SetActive(canInteract);

        if (canInteract && interactAction.triggered)
        {
            isDialogueActive = true;
            visualCue.SetActive(false);
            dialogueManager.StartDialogue(inkFile, this);
        }
    }

    private bool IsPlayerFacingNPC()
    {
        Vector2 directionToNPC = (transform.position - playerTransform.position).normalized;
        Vector2 playerFacingDirection = playerMove.LastDirection.normalized;
        float angle = Vector2.Angle(directionToNPC, playerFacingDirection);
        return angle <= maxFacingAngle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (visualCue != null)
                visualCue.SetActive(false);
        }
    }

    public void OnDialogueEnd()
    {
        isDialogueActive = false;
        StartCoroutine(InteractionCooldown());
    }

    private IEnumerator InteractionCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(interactionCooldown);
        isCooldown = false;
    }

    public void DisableInteraction()
    {
        canTalk = false;
        if (visualCue != null)
            visualCue.SetActive(false);
    }

    public void SetInkFile(TextAsset newInkFile)
    {
        inkFile = newInkFile;
    }

    public void StartDialogueExternal()
    {
        if (!isDialogueActive && canTalk && !isCooldown)
        {
            isDialogueActive = true;
            if (visualCue != null)
                visualCue.SetActive(false);
            dialogueManager.StartDialogue(inkFile, this);
        }
    }
}
