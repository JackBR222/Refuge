using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InkDialogueManager : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Configurações de Digitação")]
    [SerializeField] private float typingSpeed = 0.02f;
    [SerializeField] private bool useTypewriter = true;

    [Header("Arquivo JSON do Ink")]
    [SerializeField] private TextAsset inkJsonFile;

    [Header("Script dos Retratos")]
    [SerializeField] private DialoguePortraitManager portraitManager;

    [Header("Referências de Jogador")]
    [SerializeField] private PlayerMove playerMove;

    private Story currentStory;
    private Coroutine typingCoroutine;
    private AudioSource currentVoiceAudio;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private InkTagEventTrigger[] tagEventTriggers;

    private NPCInteractDialogue currentNPC;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string AUDIO_TAG = "audio";

    [SerializeField] private DialogueChoices dialogueChoices;

    void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
        else
            Debug.LogWarning("Botão 'Next' não atribuído!");

        dialoguePanel?.SetActive(false);

        if (portraitManager == null)
            Debug.LogWarning("Portrait Manager não atribuído!");

        if (playerMove == null)
            Debug.LogError("PlayerMove não atribuído!");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
                interactAction = playerInput.actions["Interact"];
        }
        else
            Debug.LogError("Jogador com tag 'Player' não encontrado!");
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && interactAction != null && interactAction.triggered)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;

                if (currentVoiceAudio != null)
                {
                    currentVoiceAudio.Stop();
                    currentVoiceAudio.loop = false;
                    currentVoiceAudio = null;
                }

                dialogueText.text = currentStory.currentText.Trim();
                HandleTags(currentStory.currentTags);
                ShowChoices();
            }
            else if (currentStory != null)
            {
                if (currentStory.canContinue)
                {
                    NextLine();
                }
                else if (currentStory.currentChoices.Count == 0)
                {
                    EndDialogue();
                }
            }
        }
    }

    public void StartDialogue(TextAsset inkJSON, NPCInteractDialogue npc = null)
    {
        currentStory = new Story(inkJSON.text);
        dialoguePanel.SetActive(true);

        if (playerMove != null)
            playerMove.canMove = false;

        tagEventTriggers = FindObjectsByType<InkTagEventTrigger>(FindObjectsSortMode.None);

        currentNPC = npc;

        NextLine();
    }

    public void DialogueTrigger(TextAsset customInkFile)
    {
        if (customInkFile == null)
        {
            Debug.LogError("Ink JSON fornecido via evento é nulo!");
            return;
        }

        StartDialogue(customInkFile, null);
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf && currentStory != null;
    }

    public void NextLine()
    {
        if (currentStory == null || !currentStory.canContinue)
        {
            EndDialogue();
            return;
        }

        nextButton.gameObject.SetActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        string line = currentStory.Continue().Trim();

        HandleTags(currentStory.currentTags);

        if (useTypewriter)
            typingCoroutine = StartCoroutine(TypeLine(line));
        else
        {
            dialogueText.text = line;
            ShowChoices();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        if (currentVoiceAudio != null)
        {
            currentVoiceAudio.loop = true;
            currentVoiceAudio.Play();
        }

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (currentVoiceAudio != null)
        {
            currentVoiceAudio.Stop();
            currentVoiceAudio.loop = false;
            currentVoiceAudio = null;
        }

        HandleTags(currentStory.currentTags);
        ShowChoices();

        typingCoroutine = null;
    }

    private void ShowChoices()
    {
        if (currentStory.currentChoices.Count > 0)
        {
            nextButton.gameObject.SetActive(false);
            dialogueChoices.Initialize(currentStory, this);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        string speakerName = null;
        string portraitName = null;
        string audioName = null;

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag inválida: " + tag);
                continue;
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    speakerName = tagValue;
                    break;

                case PORTRAIT_TAG:
                    portraitName = tagValue;
                    break;

                case AUDIO_TAG:
                    audioName = tagValue;
                    break;
            }

            if (tagEventTriggers != null)
            {
                foreach (var trigger in tagEventTriggers)
                {
                    trigger.CheckTag(tag);
                }
            }
        }

        if (portraitManager != null)
            currentVoiceAudio = portraitManager.HandleTags(speakerName, portraitName, audioName);
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (playerMove != null)
            playerMove.canMove = true;

        if (currentNPC != null)
        {
            currentNPC.OnDialogueEnd();
            currentNPC = null;
        }
    }

    public void ContinueAfterChoice()
    {
        if (currentStory == null) return;
        NextLine();
    }
}
