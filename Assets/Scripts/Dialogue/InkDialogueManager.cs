using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InkDialogueManager : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Configurações")]
    [SerializeField] private float typingSpeed = 0.02f;
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float choiceCooldown = 0.3f;

    [Header("Cooldown para Skip Typewriter")]
    [SerializeField] private float skipTypewriterCooldown = 0.2f;

    [Header("Scripts extras de diálogo")]
    [SerializeField] private DialoguePortraitManager portraitManager;
    [SerializeField] private DialogueChoices dialogueChoices;

    [Header("Referências de Jogador")]
    [SerializeField] private PlayerMove playerMove;

    private Story currentStory;
    private Coroutine typingCoroutine;
    private AudioSource currentVoiceAudio;

    private InputAction advanceAction;
    private InkTagEventTrigger[] tagEventTriggers;
    private NPCInteractDialogue currentNPC;

    private bool canAdvanceAfterChoice = true;
    private bool canSkipTypewriter = true;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string AUDIO_TAG = "audio";

    void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);

        dialoguePanel?.SetActive(false);

        var playerInput = InputManager.Instance.playerInput;
        if (playerInput == null)
        {
            Debug.LogError("[InkDialogueManager] PlayerInput não encontrado no InputManager!");
            return;
        }

        advanceAction = playerInput.actions["Submit"];
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf || advanceAction == null || !advanceAction.triggered)
            return;

        if (typingCoroutine != null)
        {
            if (!canSkipTypewriter) return;

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
            return;
        }

        if (currentStory.currentChoices.Count > 0 || !canAdvanceAfterChoice)
            return;

        if (currentStory.canContinue)
        {
            NextLine();
        }
        else if (currentStory.currentChoices.Count == 0)
        {
            EndDialogue();
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

        dialogueChoices.Initialize(currentStory, this, InputManager.Instance.playerInput);

        StartCoroutine(TypewriterSkipCooldown());
        NextLine();
    }

    public void DialogueTrigger(TextAsset customInkFile)
    {
        if (customInkFile != null)
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
            dialogueChoices.Initialize(currentStory, this, InputManager.Instance.playerInput);
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

        InkTagHandler tagHandler = GetComponent<InkTagHandler>();
        if (tagHandler == null)
        {
            Debug.LogError("InkTagHandler não encontrado! Adicione esse script no mesmo GameObject.");
        }

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) continue;

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG: speakerName = tagValue; break;
                case PORTRAIT_TAG: portraitName = tagValue; break;
                case AUDIO_TAG: audioName = tagValue; break;
            }

            if (tagEventTriggers != null)
            {
                foreach (var trigger in tagEventTriggers)
                {
                    trigger.CheckTag(tag);
                }
            }
        }

        if (tagHandler != null)
            tagHandler.HandleTags(currentTags);

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
        StartCoroutine(ChoiceCooldownCoroutine());
    }

    private IEnumerator ChoiceCooldownCoroutine()
    {
        canAdvanceAfterChoice = false;
        yield return new WaitForSeconds(choiceCooldown);
        canAdvanceAfterChoice = true;
        NextLine();
    }

    private IEnumerator TypewriterSkipCooldown()
    {
        canSkipTypewriter = false;
        yield return new WaitForSeconds(skipTypewriterCooldown);
        canSkipTypewriter = true;
    }
}
