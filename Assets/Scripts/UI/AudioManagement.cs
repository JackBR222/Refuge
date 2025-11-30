using UnityEngine;
using UnityEngine.UI;

public class AudioManagement : MonoBehaviour
{
    [Header("Referências da UI")]
    public Slider volumeSlider; // Slider de volume
    public Button muteButton; // Botão de cortar áudio

    private bool isMuted = false; // Indica se o áudio está mutado
    private float lastVolume = 1f; // Volume antes de mutar

    private void Start()
    {
        AudioListener.volume = 1f; // Garantir que o volume começa em 1

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // Sempre que o slider mudar, chamamos OnVolumeChanged
        }

        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
        }
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;

        if (value > 0f)
        {
            lastVolume = value;
            isMuted = false;
        }
        else
        {
            isMuted = true;
        }
    }

    public void ToggleMute()
    {
        if (isMuted)
        {
            AudioListener.volume = lastVolume;
            if (volumeSlider != null)
            {
                volumeSlider.value = lastVolume;
            }
            isMuted = false;
        }
        else
        {
            lastVolume = AudioListener.volume;
            AudioListener.volume = 0f;
            if (volumeSlider != null)
            {
                volumeSlider.value = 0f;
            }
            isMuted = true;
        }
    }
}