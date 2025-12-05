using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup optionsMenuCanvasGroup;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;

    [Header("Botões e Sliders")]
    [SerializeField] private List<Button> mainMenuButtons;
    [SerializeField] private List<Button> optionsMenuButtons;
    [SerializeField] private List<Slider> optionsSliders;

    [Header("Cores")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightedColor = Color.yellow;
    [SerializeField] private Color adjustingSliderColor = Color.cyan;

    [Header("Configurações do Slider")]
    [SerializeField] private float sliderSpeed = 0.01f;

    private InputAction submitAction;
    private InputAction navigateAction;
    private InputAction returnAction;

    private int selectedIndex = 0;
    private bool isInOptionsMenu = false;
    private bool isAdjustingSlider = false;

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
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

        submitAction = actions["Submit"];
        navigateAction = actions["Navigate"];
        returnAction = actions["Return"];

        submitAction.performed += ctx => OnSubmit();
        navigateAction.performed += ctx => OnNavigate(ctx.ReadValue<Vector2>());
        returnAction.performed += ctx => OnReturn();
    }

    private void SetupMouseHighlight(List<Button> buttons)
    {
        foreach (var btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>() ??
                                   btn.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            EventTrigger.Entry enter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enter.callback.AddListener((data) =>
            {
                selectedIndex = buttons.IndexOf(btn);
                isAdjustingSlider = false;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(enter);
        }
    }

    private void SetupMouseHighlightSliders(List<Slider> sliders, int startIndex)
    {
        for (int i = 0; i < sliders.Count; i++)
        {
            int sliderIndex = startIndex + i - 1;
            Slider s = sliders[i];

            EventTrigger trigger = s.gameObject.GetComponent<EventTrigger>() ??
                                   s.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            EventTrigger.Entry enter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enter.callback.AddListener((d) =>
            {
                selectedIndex = sliderIndex;
                isAdjustingSlider = false;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(enter);

            EventTrigger.Entry pointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDown.callback.AddListener((d) =>
            {
                selectedIndex = sliderIndex;
                isAdjustingSlider = true;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUp.callback.AddListener((d) =>
            {
                isAdjustingSlider = false;
                HighlightCurrentElement();
            });
            trigger.triggers.Add(pointerUp);
        }
    }

    private void HighlightCurrentElement()
    {
        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        for (int i = 0; i < currentButtons.Count; i++)
        {
            currentButtons[i].image.color =
                (i == selectedIndex) ? highlightedColor : normalColor;
        }

        for (int i = 0; i < optionsSliders.Count; i++)
        {
            int sliderIndex = currentButtons.Count + i;
            Slider s = optionsSliders[i];
            var fill = s.fillRect.GetComponent<Image>();
            var handle = s.handleRect.GetComponent<Image>();

            if (sliderIndex == selectedIndex)
            {
                var c = isAdjustingSlider ? adjustingSliderColor : highlightedColor;
                fill.color = c;
                handle.color = c;
            }
            else
            {
                fill.color = normalColor;
                handle.color = normalColor;
            }
        }
    }

    private void OnNavigate(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.1f) return;

        List<Selectable> elements = new List<Selectable>();
        if (!isInOptionsMenu)
            elements.AddRange(mainMenuButtons);
        else
        {
            elements.AddRange(optionsMenuButtons);
            elements.AddRange(optionsSliders);
        }

        if (isAdjustingSlider && selectedIndex >= optionsMenuButtons.Count)
        {
            int sliderIndex = selectedIndex - optionsMenuButtons.Count;
            Slider s = optionsSliders[sliderIndex];
            s.value += dir.x * sliderSpeed;
            HighlightCurrentElement();
            return;
        }

        Selectable current = elements[selectedIndex];
        Vector2 direction = dir.normalized;
        Vector2 currentPos = ((RectTransform)current.transform).position;
        float bestScore = Mathf.NegativeInfinity;
        int bestIndex = selectedIndex;

        for (int i = 0; i < elements.Count; i++)
        {
            if (i == selectedIndex) continue;
            RectTransform rt = (RectTransform)elements[i].transform;
            Vector2 pos = rt.position;
            Vector2 toTarget = (pos - currentPos).normalized;
            float dot = Vector2.Dot(direction, toTarget);
            if (dot <= 0.3f) continue;
            float dist = Vector2.Distance(currentPos, pos);
            float score = dot - dist * 0.05f;
            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        if (bestIndex != selectedIndex)
        {
            selectedIndex = bestIndex;
            isAdjustingSlider = false;
            HighlightCurrentElement();
        }
    }

    private void OnSubmit()
    {
        List<Button> currentButtons = isInOptionsMenu ? optionsMenuButtons : mainMenuButtons;

        if (isInOptionsMenu && selectedIndex >= currentButtons.Count)
        {
            isAdjustingSlider = !isAdjustingSlider;
            HighlightCurrentElement();
            return;
        }

        if (selectedIndex < currentButtons.Count)
            currentButtons[selectedIndex].onClick.Invoke();
    }

    private void OnReturn()
    {
        if (isInOptionsMenu)
            CloseOptionsMenu();
    }

    public void OpenOptionsMenu()
    {
        isInOptionsMenu = true;
        optionsMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        selectedIndex = 0;
        isAdjustingSlider = false;
        HighlightCurrentElement();
    }

    public void CloseOptionsMenu()
    {
        isInOptionsMenu = false;
        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        selectedIndex = 0;
        isAdjustingSlider = false;
        HighlightCurrentElement();
    }

    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}