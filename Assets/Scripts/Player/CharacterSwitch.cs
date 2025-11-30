using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSwitch : MonoBehaviour
{
    public GameObject character1;
    public GameObject character2;
    public bool followInactiveCharacter = true;
    public bool startFollowingOnStart = true;

    public PlayerCamera playerCamera;
    public Image uiSpriteImage;
    public Sprite spriteForCharacter1;
    public Sprite spriteForCharacter2;

    private GameObject activeCharacter;
    private GameObject inactiveCharacter;

    private PlayerMove activePlayerMove;
    private PlayerInput activePlayerInput;
    private NPCFollower inactiveNpcFollower;

    private PlayerInput playerInput;
    private InputAction switchAction;

    void Start()
    {
        activeCharacter = character1;
        inactiveCharacter = character2;

        playerInput = character1.GetComponent<PlayerInput>();
        switchAction = playerInput.actions["Switch"];
        switchAction.started += OnSwitch;

        InitializeCharacter(activeCharacter, true, false);
        InitializeCharacter(inactiveCharacter, false, true);

        if (startFollowingOnStart && inactiveCharacter.GetComponent<NPCFollower>() != null)
        {
            inactiveCharacter.GetComponent<NPCFollower>().StartFollowing();
        }

        if (startFollowingOnStart && activeCharacter.GetComponent<NPCFollower>() != null)
        {
            activeCharacter.GetComponent<NPCFollower>().StartFollowing();
        }

        if (playerCamera != null)
        {
            playerCamera.SwitchActiveCharacter(activeCharacter);
        }

        UpdateUISprite();
    }

    void InitializeCharacter(GameObject character, bool isActivePlayerMove, bool isActiveNpcFollower)
    {
        PlayerMove playerMoveScript = character.GetComponent<PlayerMove>();
        NPCFollower npcFollowerScript = character.GetComponent<NPCFollower>();

        if (playerMoveScript != null)
            playerMoveScript.enabled = isActivePlayerMove;

        if (npcFollowerScript != null)
            npcFollowerScript.enabled = isActiveNpcFollower;
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SwitchCharacter();
        }
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

        if (inactiveCharacter.GetComponent<NPCFollower>() != null)
        {
            inactiveCharacter.GetComponent<NPCFollower>().enabled = followInactiveCharacter;
        }

        if (playerCamera != null)
        {
            playerCamera.SwitchActiveCharacter(activeCharacter);
        }

        UpdateUISprite();
    }

    private void UpdateUISprite()
    {
        if (uiSpriteImage != null)
        {
            if (activeCharacter == character1)
            {
                uiSpriteImage.sprite = spriteForCharacter1;
            }
            else if (activeCharacter == character2)
            {
                uiSpriteImage.sprite = spriteForCharacter2;
            }
        }
    }

    private void OnDisable()
    {
        switchAction.started -= OnSwitch;
    }
}
