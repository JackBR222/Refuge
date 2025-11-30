using UnityEngine;
using System.Collections;

public class TutorialHint : MonoBehaviour
{
    public static TutorialHint Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] tutorialPanels;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayTime = 3f;

    [Header("Auto Show")]
    [SerializeField] private bool showOnStart = false;
    [SerializeField] private int defaultPanelIndex = 0;

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
            ShowPanel(defaultPanelIndex, displayTime);
        }
    }

    public static void ShowPanel(int panelIndex, float? duration = null)
    {
        if (Instance == null)
        {
            Debug.LogWarning("Nenhum TutorialHint encontrado na cena.");
            return;
        }

        Instance.ShowPanelCoroutine(panelIndex, duration ?? Instance.displayTime);
    }

    public void ShowPanelEvent(int panelIndex)
    {
        ShowPanel(panelIndex);
    }

    private void ShowPanelCoroutine(int panelIndex, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowPanelCoroutineRoutine(panelIndex, duration));
    }

    private IEnumerator ShowPanelCoroutineRoutine(int panelIndex, float duration)
    {
        foreach (var panel in tutorialPanels)
        {
            panel.SetActive(false);
        }

        if (panelIndex >= 0 && panelIndex < tutorialPanels.Length)
        {
            tutorialPanels[panelIndex].SetActive(true);
            yield return Fade(1);

            yield return new WaitForSeconds(duration);

            yield return Fade(0);
            tutorialPanels[panelIndex].SetActive(false);
        }

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