using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueChoices : MonoBehaviour
{
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private InkDialogueManager dialogueManager;
    private Story currentStory;
    private List<Button> currentButtons = new List<Button>();
    private int selectedIndex = 0;

    public void Initialize(Story story, InkDialogueManager manager)
    {
        currentStory = story;
        dialogueManager = manager;
        ShowChoices();
        HighlightChoice(0);
    }

    private void Update()
    {
        if (currentButtons.Count == 0) return;

        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
                MakeChoice(i);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + currentButtons.Count) % currentButtons.Count;
            HighlightChoice(selectedIndex);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % currentButtons.Count;
            HighlightChoice(selectedIndex);
        }

        if (Input.GetKeyDown(KeyCode.Return))
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
            Image buttonImage = buttonObj.GetComponent<Image>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"{(i + 1)}. {choice.text}";

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