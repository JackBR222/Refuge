using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public PlayerInput playerInput;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(gameObject);
    }
}