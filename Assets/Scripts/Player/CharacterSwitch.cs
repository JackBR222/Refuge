using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSwitch : MonoBehaviour
{
    [Header("Personagens")]
    public GameObject character1;
    public GameObject character2;

    [Header("Configurações")]
    public bool followInactiveCharacter = true;
    public bool startFollowingOnStart = true;

    [Header("UI")]
    public PlayerCamera playerCamera;
    public Image uiSpriteImage;
    public Sprite spriteForCharacter1;
    public Sprite spriteForCharacter2;

    [Header("Sistema de Troca")]
    [SerializeField] private bool systemEnabled = false;

    private GameObject activeCharacter;
    private GameObject inactiveCharacter;

    private InputAction switchAction;

    void Start()
    {
        activeCharacter = character1;
        inactiveCharacter = character2;

        if (InputManager.Instance == null || InputManager.Instance.playerInput == null)
        {
            Debug.LogError("InputManager ou PlayerInput não encontrado na cena!");
            return;
        }

        switchAction = InputManager.Instance.playerInput.actions["Switch"];
        switchAction.started += OnSwitch;

        InitializeCharacter(activeCharacter, true, false);
        InitializeCharacter(inactiveCharacter, false, true);

        if (startFollowingOnStart && inactiveCharacter.GetComponent<NPCFollower>() != null)
            inactiveCharacter.GetComponent<NPCFollower>().StartFollowing();

        if (startFollowingOnStart && activeCharacter.GetComponent<NPCFollower>() != null)
            activeCharacter.GetComponent<NPCFollower>().StartFollowing();

        if (playerCamera != null)
            playerCamera.SwitchActiveCharacter(activeCharacter);

        UpdateUISprite();
        UpdateUIVisibility();
    }

    void InitializeCharacter(GameObject character, bool isActivePlayerMove, bool isActiveNpcFollower)
    {
        PlayerMove playerMoveScript = character.GetComponent<PlayerMove>();
        NPCFollower npcFollowerScript = character.GetComponent<NPCFollower>();

        if (playerMoveScript != null)
        {
            if (isActivePlayerMove)
            {
                playerMoveScript.canMove = true;
                playerMoveScript.enabled = true;
            }
            else
            {
                playerMoveScript.enabled = false;

                if (!followInactiveCharacter)
                {
                    playerMoveScript.canMove = false;
                }
                else
                {
                    playerMoveScript.canMove = true;
                }
            }
        }

        if (npcFollowerScript != null)
            npcFollowerScript.enabled = isActiveNpcFollower;
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!systemEnabled) return;
        if (context.started) SwitchCharacter();
    }

    void SwitchCharacter()
    {
        InitializeCharacter(activeCharacter, false, false);
        InitializeCharacter(inactiveCharacter, false, false);

        GameObject temp = activeCharacter;
        activeCharacter = inactiveCharacter;
        inactiveCharacter = temp;

        InitializeCharacter(activeCharacter, true, false);
        InitializeCharacter(inactiveCharacter, false, true);

        NPCFollower npcFollower = inactiveCharacter.GetComponent<NPCFollower>();
        if (npcFollower != null)
        {
            npcFollower.enabled = followInactiveCharacter;

            if (followInactiveCharacter && npcFollower.enabled)
            {
                npcFollower.StartFollowing();
            }
        }

        if (playerCamera != null)
            playerCamera.SwitchActiveCharacter(activeCharacter);

        UpdateUISprite();
    }

    private void UpdateUISprite()
    {
        if (uiSpriteImage != null)
        {
            uiSpriteImage.sprite = (activeCharacter == character1) ? spriteForCharacter1 : spriteForCharacter2;
        }
    }

    private void UpdateUIVisibility()
    {
        if (uiSpriteImage != null)
            uiSpriteImage.gameObject.SetActive(systemEnabled);
    }

    public void EnableSwitchSystem()
    {
        systemEnabled = true;
        UpdateUIVisibility();
    }

    public void DisableSwitchSystem()
    {
        systemEnabled = false;
        UpdateUIVisibility();
    }

    private void OnDisable()
    {
        if (switchAction != null)
            switchAction.started -= OnSwitch;
    }
}