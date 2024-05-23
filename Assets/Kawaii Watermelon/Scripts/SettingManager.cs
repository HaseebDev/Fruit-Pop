using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] Button vibrationButton;
    [SerializeField] Sprite vibrationDisabled;
    [SerializeField] Sprite vibrationEnabled;

    public Slider soundVolume;
    public Slider musicVolume;
    SoundManager audioManager;

    private const string VibrationKey = "VibrationEnabled";
    private bool isVibrationEnabled;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = SoundManager.instance;

        // Load the saved volume values or use default values
        musicVolume.value = PlayerPrefs.GetFloat(audioManager.MusicVolumeKey, 1f);
        soundVolume.value = PlayerPrefs.GetFloat(audioManager.SoundVolumeKey, 1f);
        musicVolume.onValueChanged.AddListener((volume) => audioManager.UpdateMusicVolume(volume));
        soundVolume.onValueChanged.AddListener((volume) => audioManager.UpdateSoundVolume(volume));

        // Load vibration setting
        isVibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        UpdateVibrationButtonSprite();

        vibrationButton.onClick.AddListener(ToggleVibration);

 
    }

    private void OnDisable()
    {
        // Save the volume values and vibration state when the script is disabled
        PlayerPrefs.SetFloat(audioManager.MusicVolumeKey, musicVolume.value);
        PlayerPrefs.SetFloat(audioManager.SoundVolumeKey, soundVolume.value);
        PlayerPrefs.SetInt(VibrationKey, isVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ToggleVibration()
    {
        isVibrationEnabled = !isVibrationEnabled;
        UpdateVibrationButtonSprite();
        PlayerPrefs.SetInt(VibrationKey, isVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateVibrationButtonSprite()
    {
        vibrationButton.image.sprite = isVibrationEnabled ? vibrationEnabled : vibrationDisabled;
    }
}
