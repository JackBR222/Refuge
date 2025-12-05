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
    [SerializeField] private Sprite birdNeutral;
    [SerializeField] private Sprite antaNeutral;
    [SerializeField] private Sprite queixadaNeutral;
    [SerializeField] private Sprite oncaNeutral;

    [Header("Expressões: Saruê")]
    [SerializeField] private Sprite maracajaEvil;
    [SerializeField] private Sprite maracajaSad;
    [SerializeField] private Sprite maracajaScared;
    [SerializeField] private Sprite maracajaAngry;
    [SerializeField] private Sprite maracajaSmile;

    [Header("Expressões: Beija-Flor")]
    [SerializeField] private Sprite birdHappy;
    [SerializeField] private Sprite birdScared;
    [SerializeField] private Sprite birdUpset;
    [SerializeField] private Sprite birdSad;

    [Header("Referências de Sons")]
    [SerializeField] private AudioSource sarueVoice;
    [SerializeField] private AudioSource maracajaVoice;
    [SerializeField] private AudioSource jabutiVoice;
    [SerializeField] private AudioSource fireflyVoice;
    [SerializeField] private AudioSource birdVoice;
    [SerializeField] private AudioSource antaVoice;
    [SerializeField] private AudioSource queixadaVoice;
    [SerializeField] private AudioSource oncaVoice;

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

            case "BirdNeutral":
                portraitImage.sprite = birdNeutral;
                break;

            case "AntaNeutral":
                portraitImage.sprite = antaNeutral;
                break;

            case "QueixadaNeutral":
                portraitImage.sprite = queixadaNeutral;
                break;

            case "OncaNeutral":
                portraitImage.sprite = oncaNeutral;
                break;

            default:
                Debug.LogWarning("Retrato não encontrado: " + portraitName);
                break;

            // Expressões do Maracajá
            case "MaracajaEvil":
                portraitImage.sprite = maracajaEvil;
                break;

            case "MaracajaSad":
                portraitImage.sprite = maracajaSad;
                break;

            case "MaracajaScared":
                portraitImage.sprite = maracajaScared;
                break;

            case "MaracajaAngry":
                portraitImage.sprite = maracajaAngry;
                break;

            case "MaracajaSmile":
                portraitImage.sprite = maracajaSmile;
                break;

            // Expressões do Beija-Flor
            case "BirdHappy":
                portraitImage.sprite = birdHappy;
                break;

            case "BirdScared":
                portraitImage.sprite = birdScared;
                break;

            case "BirdUpset":
                portraitImage.sprite = birdUpset;
                break;

            case "BirdSad":
                portraitImage.sprite = birdSad;
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

            case "BirdVoice":
                return birdVoice;

            case "AntaVoice":
                return antaVoice;

            case "QueixadaVoice":
                return queixadaVoice;

            case "OncaVoice":
                return oncaVoice;

            default:
                Debug.LogWarning("Áudio não encontrado: " + audioName);
                return null;
        }
    }
}