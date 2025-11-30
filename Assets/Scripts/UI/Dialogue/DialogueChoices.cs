using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueChoices : MonoBehaviour
{
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private float choiceCooldown = 0.3f;

    private InkDialogueManager dialogueManager;
    private Story currentStory;
    private List<Button> currentButtons = new List<Button>();
    private int selectedIndex = 0;

    private PlayerInput playerInput;
    private InputAction navigateAction;
    private InputAction submitAction;

    private bool canSelectChoice = false;

    public void Initialize(Story story, InkDialogueManager manager, PlayerInput playerInput)
    {
        currentStory = story;
        dialogueManager = manager;

        this.playerInput = playerInput;
        if (this.playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing.");
            return;
        }

        navigateAction = playerInput.actions["Navigate"];
        submitAction = playerInput.actions["Submit"];

        if (navigateAction == null || submitAction == null)
        {
            Debug.LogError("Input actions 'Navigate' or 'Submit' not found.");
            return;
        }

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;

        ShowChoices();
        HighlightChoice(0);
        StartCoroutine(EnableChoiceAfterCooldown());
    }

    private void OnEnable()
    {
        if (navigateAction != null) navigateAction.performed += OnNavigate;
        if (submitAction != null) submitAction.performed += OnSubmit;
    }

    private void OnDisable()
    {
        if (navigateAction != null) navigateAction.performed -= OnNavigate;
        if (submitAction != null) submitAction.performed -= OnSubmit;
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (currentButtons.Count == 0) return;

        Vector2 direction = context.ReadValue<Vector2>();

        if (direction.y > 0)
        {
            selectedIndex = (selectedIndex - 1 + currentButtons.Count) % currentButtons.Count;
        }
        else if (direction.y < 0)
        {
            selectedIndex = (selectedIndex + 1) % currentButtons.Count;
        }

        HighlightChoice(selectedIndex);
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!canSelectChoice) return;
        if (currentButtons.Count == 0) return;

        MakeChoice(selectedIndex);
    }

    private void ShowChoices()
    {
        ClearChoices();

        for (int i = 0; i < currentStory.currentChoices.Count; i++)
        {
            Choice choice = currentStory.currentChoices[i];
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = choice.text;

            int index = i;
            button.onClick.AddListener(() => MakeChoice(index));

            EventTriggerListener listener = buttonObj.AddComponent<EventTriggerListener>();
            listener.OnEnter = () => HighlightChoice(index);

            currentButtons.Add(button);
        }
    }

    private void HighlightChoice(int index)
    {
        selectedIndex = index;
        for (int i = 0; i < currentButtons.Count; i++)
        {
            Button btn = currentButtons[i];
            Image btnImage = btn.GetComponent<Image>();

            if (btnImage != null)
                btnImage.color = (i == index) ? normalColor : dimmedColor;
        }
    }

    private void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ClearChoices();
        if (dialogueManager != null)
            dialogueManager.ContinueAfterChoice();
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
        currentButtons.Clear();
        canSelectChoice = false;
    }

    private IEnumerator EnableChoiceAfterCooldown()
    {
        canSelectChoice = false;
        yield return new WaitForSeconds(choiceCooldown);
        canSelectChoice = true;
    }
}

public class EventTriggerListener : MonoBehaviour, IPointerEnterHandler
{
    public System.Action OnEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }
}