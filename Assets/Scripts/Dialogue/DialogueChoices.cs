using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueChoices : MonoBehaviour
{
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    private InkDialogueManager dialogueManager;
    private Story currentStory;
    private List<Button> currentButtons = new List<Button>();

    public void Initialize(Story story, InkDialogueManager manager)
    {
        currentStory = story;
        dialogueManager = manager;
        ShowChoices();
    }

    private void Update()
    {
        // Checa se o jogador apertou 1,2,3... para escolher
        for (int i = 0; i < currentButtons.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))  // Detecta 1, 2, 3, ...
            {
                MakeChoice(i);
            }
        }
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

            // Adiciona o número antes do texto da escolha
            buttonText.text = $"{(i + 1)}. {choice.text}";

            int index = i;
            button.onClick.AddListener(() => MakeChoice(index));

            currentButtons.Add(button);
        }
    }

    private void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ClearChoices();

        if (dialogueManager != null)
        {
            dialogueManager.ContinueAfterChoice();
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
        currentButtons.Clear();
    }
}