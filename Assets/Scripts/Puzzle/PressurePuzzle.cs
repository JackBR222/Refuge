using UnityEngine;
using UnityEngine.Events;

public class PressurePuzzle : MonoBehaviour
{
    public PressureButton[] buttons;
    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleUnsolved;

    private bool _solved = false;
    public bool keepButtonsPressed = false;

    private AudioSource _audioSource;
    public Camera mainCamera;
    public Camera puzzleSolvedCamera;

    public float cameraSwitchDuration = 5f;
    private float _timer = 0f;
    private bool _isCameraSwitched = false;

    public PlayerMove[] players;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (puzzleSolvedCamera != null) puzzleSolvedCamera.gameObject.SetActive(false);

        if (players.Length == 0)
        {
            Debug.LogError("Nenhum jogador atribuído ao PressurePuzzle!");
        }
    }

    void Update()
    {
        if (!_solved && AreAllButtonsPressed())
        {
            _solved = true;
            Debug.Log("[Puzzle2D] Puzzle resolvido!");

            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            onPuzzleSolved.Invoke();

            foreach (var player in players)
            {
                if (player != null)
                {
                    player.canMove = false;
                }
            }

            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (puzzleSolvedCamera != null) puzzleSolvedCamera.gameObject.SetActive(true);

            _timer = cameraSwitchDuration;
            _isCameraSwitched = true;

            if (keepButtonsPressed)
            {
                foreach (var button in buttons)
                {
                    button.LockButton();
                }
            }
        }
        else if (_solved && !AreAllButtonsPressed() && !keepButtonsPressed)
        {
            _solved = false;
            Debug.Log("[Puzzle2D] Puzzle desfeito.");
            onPuzzleUnsolved.Invoke();

            if (!keepButtonsPressed)
            {
                foreach (var button in buttons)
                {
                    button.UnlockButton();
                }
            }

            foreach (var player in players)
            {
                if (player != null)
                {
                    player.canMove = true;
                }
            }
        }

        if (_isCameraSwitched)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (mainCamera != null) mainCamera.gameObject.SetActive(true);
                if (puzzleSolvedCamera != null) puzzleSolvedCamera.gameObject.SetActive(false);

                foreach (var player in players)
                {
                    if (player != null)
                    {
                        player.canMove = true;
                    }
                }

                _timer = 0f;
                _isCameraSwitched = false;
            }
        }
    }

    private bool AreAllButtonsPressed()
    {
        foreach (var btn in buttons)
        {
            if (!btn.IsPressed)
                return false;
        }
        return true;
    }
}