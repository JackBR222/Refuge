using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [Header("Interface do Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private CanvasGroup optionsMenuCanvasGroup;

    [Header("Elementos Interativos")]
    [SerializeField] private List<Button> mainMenuButtons;
    [SerializeField] private List<Button> optionsMenuButtons;
    [SerializeField] private List<Slider> optionsSliders;

    [Header("Cores de Destaque")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightedColor = Color.yellow;
    [SerializeField] private Color adjustingSliderColor = Color.cyan;

    [Header("Configurações do Slider")]
    [SerializeField] private float sliderSpeed = 0.01f;

    [Header("Opções de Navegação")]
    [SerializeField] private bool isReturnButtonInOptionsMenu = true;

    private PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction submitAction;
    private InputAction navigateAction;
    private InputAction returnAction;

    private bool isPaused = false;
    private bool isInOptionsMenu = false;
    private int selectedIndex = 0;
    private bool isAdjustingSlider = false;
    private float sliderRepeatRate = 0.05f;
    private float sliderTimer = 0f;
    private float horizontalInput = 0f;

    private InkDialogueManager inkDialogueManager;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
        if (playerInput == null) return;

        pauseAction = playerInput.actions["Pause"];
        submitAction = playerInput.actions["Submit"];
        navigateAction = playerInput.actions["Navigate"];
        returnAction = playerInput.actions["Return"];

        if (pauseAction == null || submitAction == null || navigateAction == null || returnAction == null) return;

        pauseAction.performed += TogglePauseMenu;
        submitAction.performed += OnSubmit;
        navigateAction.performed += OnNavigate;
        returnAction.performed += OnReturn;

        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        backgroundPanel.SetActive(false);

        SetupButtonCallbacks(mainMenuButtons, MainMenuButtonAction);
        SetupButtonCallbacks(optionsMenuButtons, OptionsMenuButtonAction);

        SetupMouseHighlight(mainMenuButtons);
        SetupMouseHighlight(optionsMenuButtons);

        inkDialogueManager = FindAnyObjectByType<InkDialogueManager>();
    }

    private void SetupButtonCallbacks(List<Button> buttons, System.Action<int> action)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => action(index));
        }
    }

    private void SetupMouseHighlight(List<Button> buttons)
    {
        foreach (var btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) =>
            {
                selectedIndex = buttons.IndexOf(btn);
                HighlightCurrentElement();
            });
            trigger.triggers.Add(entry);
        }
    }

    private void MainMenuButtonAction(int index)
    {
        switch (index)
        {
            case 0: ClosePauseMenu(); break;
            case 1: ShowOptionsMenu(); break;
            case 2: GoToMainMenu(); break;
        }
    }

    private void OptionsMenuButtonAction(int index)
    {
        if (isReturnButtonInOptionsMenu && index == 0)
        {
            ShowMainMenu();
        }
        else if (!isReturnButtonInOptionsMenu)
        {
            Debug.Log("Ação para outro botão do menu de opções.");
        }
    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (inkDialogueManager != null && inkDialogueManager.IsDialogueActive())
            return;

        if (isInOptionsMenu) return;
        if (isPaused) ClosePauseMenu();
        else OpenPauseMenu();
    }

    private void OpenPauseMenu()
    {
        isPaused = true;
        isInOptionsMenu = false;
        selectedIndex = 0;
        Time.timeScale = 0f;
        backgroundPanel.SetActive(true);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        StartCoroutine(FadeIn(pauseMenuCanvasGroup));
        HighlightCurrentElement();
    }

    private void ClosePauseMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        backgroundPanel.SetActive(false);
        StartCoroutine(FadeOut(pauseMenuCanvasGroup));
    }

    private void ShowOptionsMenu()
    {
        isInOptionsMenu = true;
        selectedIndex = 0;
        StartCoroutine(FadeOut(pauseMenuCanvasGroup));
        optionsMenuPanel.SetActive(true);
        StartCoroutine(FadeIn(optionsMenuCanvasGroup));
        HighlightCurrentElement();
    }

    private void ShowMainMenu()
    {
        isInOptionsMenu = false;
        selectedIndex = 0;
        StartCoroutine(FadeOut(optionsMenuCanvasGroup));
        optionsMenuPanel.SetActive(false);
        StartCoroutine(FadeIn(pauseMenuCanvasGroup));
        HighlightCurrentElement();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!isPaused) return;

        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        if (isInOptionsMenu && selectedIndex >= currentButtons.Count)
        {
            isAdjustingSlider = !isAdjustingSlider;
        }
        else
        {
            if (selectedIndex < currentButtons.Count) currentButtons[selectedIndex].onClick.Invoke();
        }

        HighlightCurrentElement();
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isPaused) return;
        Vector2 direction = context.ReadValue<Vector2>();
        horizontalInput = direction.x;

        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;
        int totalElements = currentButtons.Count + (isInOptionsMenu ? optionsSliders.Count : 0);
        if (totalElements == 0) return;

        if (isAdjustingSlider) return;

        if (direction.y != 0 || direction.x != 0)
        {
            selectedIndex = GetNextSelectableIndex(direction, currentButtons);
            HighlightCurrentElement();
        }
    }

    private int GetNextSelectableIndex(Vector2 direction, List<Button> buttons)
    {
        int current = selectedIndex;
        List<SelectableElement> elements = new List<SelectableElement>();

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform rt = buttons[i].GetComponent<RectTransform>();
            elements.Add(new SelectableElement { index = i, pos = rt.position });
        }

        if (isInOptionsMenu)
        {
            for (int i = 0; i < optionsSliders.Count; i++)
            {
                RectTransform rt = optionsSliders[i].GetComponent<RectTransform>();
                elements.Add(new SelectableElement { index = buttons.Count + i, pos = rt.position });
            }
        }

        Vector2 dir = direction.normalized;
        float minAngle = 360f;
        int bestIndex = current;
        Vector2 currentPos = elements[current].pos;

        for (int i = 0; i < elements.Count; i++)
        {
            if (i == current) continue;
            Vector2 toTarget = (elements[i].pos - currentPos).normalized;
            float angle = Vector2.Angle(dir, toTarget);
            if (angle < 90f && angle < minAngle)
            {
                minAngle = angle;
                bestIndex = elements[i].index;
            }
        }

        return bestIndex;
    }

    private void Update()
    {
        if (!isPaused) return;

        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;
        if (isInOptionsMenu && optionsSliders.Count > 0 && selectedIndex >= currentButtons.Count)
        {
            int sliderIndex = selectedIndex - currentButtons.Count;
            Slider currentSlider = optionsSliders[sliderIndex];

            if (isAdjustingSlider && Mathf.Abs(horizontalInput) > 0.01f)
            {
                sliderTimer -= Time.unscaledDeltaTime;
                if (sliderTimer <= 0f)
                {
                    currentSlider.value += horizontalInput * sliderSpeed;
                    sliderTimer = sliderRepeatRate;
                }
            }
            else sliderTimer = 0f;
        }
    }

    private void HighlightCurrentElement()
    {
        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        for (int i = 0; i < currentButtons.Count; i++)
        {
            Image img = currentButtons[i].GetComponent<Image>();
            img.color = (i == selectedIndex) ? highlightedColor : normalColor;
        }

        for (int i = 0; i < optionsSliders.Count; i++)
        {
            Image fill = optionsSliders[i].fillRect.GetComponent<Image>();
            if (selectedIndex == currentButtons.Count + i && isAdjustingSlider)
                fill.color = adjustingSliderColor;
            else
                fill.color = (selectedIndex == currentButtons.Count + i) ? highlightedColor : normalColor;
        }
    }

    private void OnReturn(InputAction.CallbackContext context)
    {
        if (isInOptionsMenu)
        {
            ShowMainMenu();
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(true);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);
    }

    private struct SelectableElement
    {
        public int index;
        public Vector2 pos;
    }
}