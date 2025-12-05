using UnityEngine;
using UnityEngine.UI;

public class AudioManagement : MonoBehaviour
{
    public Slider volumeSlider;
    public Button muteButton;

    private bool isMuted = false;
    private float lastVolume = 1f;
    private float savedVolume = 1f;

    private static AudioManagement instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        lastVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        savedVolume = lastVolume;
        isMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;
        ApplyVolume();
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = isMuted ? 0f : lastVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (muteButton != null)
            muteButton.onClick.AddListener(ToggleMute);
    }

    public void OnVolumeChanged(float value)
    {
        lastVolume = value;
        if (!isMuted) savedVolume = value;
        ApplyVolume();
        SaveSettings();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (!isMuted) lastVolume = savedVolume;
        ApplyVolume();
        SaveSettings();
    }

    private void ApplyVolume()
    {
        AudioListener.volume = isMuted ? 0f : lastVolume;

        if (volumeSlider != null && volumeSlider.IsActive() && volumeSlider.interactable)
            volumeSlider.value = AudioListener.volume;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", savedVolume);
        PlayerPrefs.SetInt("MasterMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
