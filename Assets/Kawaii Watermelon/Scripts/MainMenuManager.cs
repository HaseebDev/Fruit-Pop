using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1); // Set currentLevel from PlayerPrefs
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1)); // Calculate currentXpRequirement after setting currentLevel
        currentXp = PlayerPrefs.GetInt(XpKey, 0);

        // Update UI elements
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
        LevelSlider.maxValue = currentXpRequirement;
        LevelSlider.value = currentXp;
        CurrentProgressLevelText.text = currentXp + "/" + currentXpRequirement;
        CurrentLevelText.text = currentLevel.ToString();
    }


    public void TapToPlayButton()
    {
        SceneManager.LoadScene(2);
    }

}
