
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    [SerializeField] TMP_Text CurrentLevelText;
    [SerializeField] TMP_Text CurrentProgressLevelText;
    [SerializeField] Slider LevelSlider;
    private int initialXpRequirement = 50;
    private float xpRequirementMultiplier = 1.5f;
    private const string LevelKey = "PlayerLevel";
    private const string XpKey = "PlayerXP";
    private int currentLevel;
    private int currentXp;
    private int currentXpRequirement;
    [SerializeField] AudioSource musicSource;

    public static MainMenuManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1); // Set currentLevel from PlayerPrefs
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1)); // Calculate currentXpRequirement after setting currentLevel
        currentXp = PlayerPrefs.GetInt(XpKey, 0);

        // Update UI elements
        UpdateCurrencyUi();
        LevelSlider.maxValue = currentXpRequirement;
        LevelSlider.value = currentXp;
        CurrentProgressLevelText.text = currentXp + "/" + currentXpRequirement;
        CurrentLevelText.text = currentLevel.ToString();
        //--------------
        InitialSetupMusicAudio();
        SoundManager.OnMusicVolumeUpdated += UpdateMusic;
    }

    private void UpdateMusic(float volume)
    {
        // Your logic to update music based on the volume
        Debug.Log("Music Volume is " + volume);

        float mappedMusicVolume = volume * 0.25f; // Map the value to a maximum of 0.25
        musicSource.volume = mappedMusicVolume;
        Debug.Log("Adjusted Music Volume is " + mappedMusicVolume);
    }
    private void InitialSetupMusicAudio()
    {
        float MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        Debug.Log("Music Volume is " + MusicVolume);

        float mappedMusicVolume = MusicVolume * 0.25f; // Map the value to a maximum of 0.25
        musicSource.volume = mappedMusicVolume;
        Debug.Log("Adjusted Music Volume is " + mappedMusicVolume);
    }
    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        SoundManager.OnMusicVolumeUpdated -= UpdateMusic;
    }
    public void UpdateCurrencyUi()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
    public void TapToPlayButton()
    {
        SceneManager.LoadScene(2);
    }

}
