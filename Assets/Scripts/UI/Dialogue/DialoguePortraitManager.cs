using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePortraitManager : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [Header("Referências de Sprites")]
    [SerializeField] private Sprite sarueNeutral;
    [SerializeField] private Sprite maracajaNeutral;
    [SerializeField] private Sprite jabutiNeutral;
    [SerializeField] private Sprite fireflyNeutral;

    [Header("Referências de Sons")]
    [SerializeField] private AudioSource sarueVoice;
    [SerializeField] private AudioSource maracajaVoice;
    [SerializeField] private AudioSource jabutiVoice;
    [SerializeField] private AudioSource fireflyVoice;

    public AudioSource HandleTags(string speakerName, string portraitName, string audioName = null)
    {
        if (!string.IsNullOrEmpty(speakerName))
            UpdateSpeakerName(speakerName);

        if (!string.IsNullOrEmpty(portraitName))
            SetPortrait(portraitName);

        if (!string.IsNullOrEmpty(audioName))
            return GetAudioSource(audioName);

        return null;
    }

    public void UpdateSpeakerName(string speakerName)
    {
        displayNameText.text = speakerName;
    }

    public void SetPortrait(string portraitName)
    {
        switch (portraitName)
        {
            case "SarueNeutral":
                portraitImage.sprite = sarueNeutral;
                break;

            case "MaracajaNeutral":
                portraitImage.sprite = maracajaNeutral;
                break;

            case "JabutiNeutral":
                portraitImage.sprite = jabutiNeutral;
                break;

            case "FireflyNeutral":
                portraitImage.sprite = fireflyNeutral;
                break;

            default:
                Debug.LogWarning("Retrato não encontrado: " + portraitName);
                break;
        }
    }

    private AudioSource GetAudioSource(string audioName)
    {
        switch (audioName)
        {
            case "SarueVoice":
                return sarueVoice;

            case "MaracajaVoice":
                return maracajaVoice;

            case "JabutiVoice":
                return jabutiVoice;

            case "FireflyVoice":
                return fireflyVoice;

            default:
                Debug.LogWarning("Áudio não encontrado: " + audioName);
                return null;
        }
    }
}