using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI LosePanellevelText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private Slider LosePanelXpBar;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private TextMeshProUGUI earnedGemsText;
    [SerializeField] private TextMeshProUGUI earnedCoinsText;
    [SerializeField] private TextMeshProUGUI GemsBarText;
    [SerializeField] private TextMeshProUGUI CoinsBarText;
    [SerializeField] private TextMeshProUGUI CurrentLevelProgress;
    [SerializeField] private TextMeshProUGUI LosePanelCurrentLevelProgress;
    [SerializeField] private Button CloseLevelUpPanelButton;

    [Header("Settings")]
    [SerializeField] private int initialXpRequirement = 50;
    [SerializeField] private float xpRequirementMultiplier = 1.5f;
    [SerializeField] private int initialGemsReward = 1;
    [SerializeField] private int initialCoinsReward = 20;
    [SerializeField] private float gemsRewardMultiplier = 1.2f;
    [SerializeField] private float coinsRewardMultiplier = 1.3f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private AudioSource LevelComplete;

    private int currentLevel;
    private int currentXp;
    private int currentXpRequirement;
    private int earnedGems;
    private int earnedCoins;

    private const string LevelKey = "PlayerLevel";
    private const string XpKey = "PlayerXP";
    private const string GemsKey = "RareCurrency";
    private const string CoinsKey = "CommonCurrency";


    private void Start()
    {
        LoadProgress();
        UpdateUI();
        CloseLevelUpPanelButton.onClick.AddListener(CloseLevelUpPanel);
        //PlayerPrefs.SetInt(GemsKey, 50);
    }

    public void AddXp(int xp)
    {
        currentXp += xp;

        while (currentXp >= currentXpRequirement)
        {
            currentXp -= currentXpRequirement;
            LevelUp();
            LevelComplete.Play();
        }

        UpdateUI();
        SaveProgress();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
        currentXp = 0; // Reset XP for the new levels

        // Increase earned gems and coins rewards for the next level
        earnedGems = Mathf.RoundToInt(initialGemsReward * Mathf.Pow(gemsRewardMultiplier, currentLevel - 1));
        earnedCoins = Mathf.RoundToInt(initialCoinsReward * Mathf.Pow(coinsRewardMultiplier, currentLevel - 1));



        // Enable level up panel and display completed level
        levelUpPanel.SetActive(true);
        levelUpText.text = (currentLevel - 1).ToString();
        earnedGemsText.text = earnedGems.ToString();
        earnedCoinsText.text = earnedCoins.ToString();
        PlayerPrefs.SetInt(GemsKey, PlayerPrefs.GetInt(GemsKey) + earnedGems);
        PlayerPrefs.SetInt(CoinsKey, PlayerPrefs.GetInt(CoinsKey) + earnedCoins);
        // Start animation coroutine
        StartCoroutine(ScaleUpPanel());
        UpdateUI(); // Update UI immediately after level up
    }

    private IEnumerator ScaleUpPanel()
    {
        float timer = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        while (timer < animationDuration)
        {
            float scale = timer / animationDuration;
            levelUpPanel.transform.localScale = Vector3.Lerp(initialScale, targetScale, scale);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the panel is exactly at the target scale
        levelUpPanel.transform.localScale = targetScale;
        UpdateUI();
    }

    public void UpdateUI()
    {
        levelText.text = currentLevel.ToString();
        LosePanellevelText.text = currentLevel.ToString();
        xpBar.maxValue = currentXpRequirement;
        LosePanelXpBar.maxValue = currentXpRequirement;
        xpBar.value = currentXp;
        LosePanelXpBar.value = currentXp;
        GemsBarText.text = PlayerPrefs.GetInt(GemsKey).ToString();
        CoinsBarText.text = PlayerPrefs.GetInt(CoinsKey).ToString();

        // Display current XP progress
        CurrentLevelProgress.text = currentXp + "/" + currentXpRequirement;
        LosePanelCurrentLevelProgress.text = currentXp + "/" + currentXpRequirement;

        Debug.Log(currentXpRequirement);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(XpKey, currentXp);
        PlayerPrefs.SetInt(GemsKey, earnedGems);
        PlayerPrefs.SetInt(CoinsKey, earnedCoins);
    }

    private void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1);
        currentXp = PlayerPrefs.GetInt(XpKey, 0);
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
        earnedGems = PlayerPrefs.GetInt(GemsKey, 0);
        earnedCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        GemsBarText.text = PlayerPrefs.GetInt(GemsKey).ToString();
        CoinsBarText.text = PlayerPrefs.GetInt(CoinsKey).ToString();
    }


    private void CloseLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
        //Play ButtonSound
    }

}
