using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialHint : MonoBehaviour
{
    public static TutorialHint Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayTime = 3f;

    [Header("Auto Show")]
    [SerializeField] private bool showOnStart = false;
    [SerializeField][TextArea] private string startMessage = "Pressione E para interagir";

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
    }

    private void Start()
    {
        if (showOnStart)
        {
            StartCoroutine(ShowOnStartCoroutine());
        }
    }

    private IEnumerator ShowOnStartCoroutine()
    {
        yield return null;
        Show(startMessage, displayTime);
    }

    public static void Show(string message, float? duration = null)
    {
        if (Instance == null)
        {
            Debug.LogWarning("Nenhum TutorialHint encontrado na cena.");
            return;
        }

        Instance.ShowHint(message, duration ?? Instance.displayTime);
    }

    public void ShowHintEvent(string message)
    {
        Show(message);
    }

    private void ShowHint(string message, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowHintCoroutine(message, duration));
    }

    private IEnumerator ShowHintCoroutine(string message, float duration)
    {
        hintText.text = message;

        yield return Fade(1);

        yield return new WaitForSeconds(duration);

        yield return Fade(0);

        currentRoutine = null;
    }

    private IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}
