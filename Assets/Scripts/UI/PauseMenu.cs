using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject backgroundPanel;

    [Header("Botões e Sliders")]
    [SerializeField] private List<Button> mainMenuButtons;
    [SerializeField] private List<Button> optionsMenuButtons;
    [SerializeField] private List<Slider> optionsSliders;

    [Header("Cores")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightedColor = Color.yellow;
    [SerializeField] private Color adjustingSliderColor = Color.cyan;

    [Header("Slider Settings")]
    [SerializeField] private float sliderStep = 0.25f; // Passo por clique/input

    private InputAction pauseAction;
    private InputAction submitAction;
    private InputAction navigateAction;
    private InputAction returnAction;
    private InputAction horizontalAction;

    private bool isPaused = false;
    private bool isInOptionsMenu = false;

    private int selectedIndex = 0;
    private bool isAdjustingSlider = false;

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        backgroundPanel.SetActive(false);

        SetupMouseHighlight(mainMenuButtons);
        SetupMouseHighlight(optionsMenuButtons);
        SetupMouseHighlightSliders(optionsSliders, mainMenuButtons.Count);

        HighlightCurrentElement();
        SetupInput();
    }

    private void SetupInput()
    {
        var actions = InputManager.Instance.playerInput.actions;

        pauseAction = actions["Pause"];
        submitAction = actions["Submit"];
        navigateAction = actions["Navigate"];
        returnAction = actions["Return"];
        horizontalAction = actions["Navigate"];

        pauseAction.performed += ctx => TogglePause();
        submitAction.performed += ctx => OnSubmit();
        navigateAction.performed += ctx => OnNavigate(ctx.ReadValue<Vector2>());
        returnAction.performed += ctx => OnReturn();
    }

    #region Mouse UI Highlight

    private void SetupMouseHighlight(List<Button> buttons)
    {
        foreach (var btn in buttons)
        {
            EventTrigger trigger = btn.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((d) =>
            {
                selectedIndex = buttons.IndexOf(btn);
                isAdjustingSlider = false;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(enter);
        }
    }

    private void SetupMouseHighlightSliders(List<Slider> sliders, int offset)
    {
        for (int i = 0; i < sliders.Count; i++)
        {
            int sliderIndex = offset + i;
            Slider s = sliders[i];
            EventTrigger trigger = s.GetComponent<EventTrigger>() ?? s.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((d) =>
            {
                selectedIndex = sliderIndex;
                isAdjustingSlider = false;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(enter);

            EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((d) => { selectedIndex = sliderIndex; isAdjustingSlider = true; HighlightCurrentElement(); });
            trigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((d) => { isAdjustingSlider = false; HighlightCurrentElement(); });
            trigger.triggers.Add(pointerUp);
        }
    }

    #endregion

    #region Highlight Logic

    private void HighlightCurrentElement()
    {
        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        for (int i = 0; i < currentButtons.Count; i++)
            currentButtons[i].image.color = (i == selectedIndex) ? highlightedColor : normalColor;

        for (int i = 0; i < optionsSliders.Count; i++)
        {
            int index = currentButtons.Count + i;
            var fill = optionsSliders[i].fillRect.GetComponent<Image>();
            fill.color = (index == selectedIndex) ? (isAdjustingSlider ? adjustingSliderColor : highlightedColor) : normalColor;
        }
    }

    #endregion

    #region Pause Logic

    private void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;

        backgroundPanel.SetActive(true);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);

        selectedIndex = 0;
        isAdjustingSlider = false;
        HighlightCurrentElement();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        pauseMenuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        isInOptionsMenu = true;
        selectedIndex = 0;

        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);

        HighlightCurrentElement();
    }

    public void BackToPause()
    {
        isInOptionsMenu = false;
        selectedIndex = 0;

        optionsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);

        HighlightCurrentElement();
    }

    #endregion

    #region Input Handling

    private void OnSubmit()
    {
        if (!isPaused) return;

        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        if (isInOptionsMenu && selectedIndex >= currentButtons.Count)
        {
            // Toggle slider adjustment
            isAdjustingSlider = !isAdjustingSlider;
            HighlightCurrentElement();
            return;
        }

        if (selectedIndex < currentButtons.Count)
            currentButtons[selectedIndex].onClick.Invoke();
    }

    private void OnNavigate(Vector2 dir)
    {
        if (!isPaused) return;

        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;
        int total = currentButtons.Count + (isInOptionsMenu ? optionsSliders.Count : 0);

        if (isAdjustingSlider)
        {
            int sliderIndex = selectedIndex - currentButtons.Count;
            if (sliderIndex >= 0 && sliderIndex < optionsSliders.Count)
            {
                if (dir.x > 0.1f) optionsSliders[sliderIndex].value += sliderStep;
                if (dir.x < -0.1f) optionsSliders[sliderIndex].value -= sliderStep;
            }
            HighlightCurrentElement();
            return;
        }

        if (dir.y > 0.1f) selectedIndex--;
        if (dir.y < -0.1f) selectedIndex++;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, total - 1);
        HighlightCurrentElement();
    }

    private void OnReturn()
    {
        if (isInOptionsMenu) BackToPause();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
}