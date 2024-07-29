using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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
    private float earnedGems;
    private float earnedCoins;

    private const string LevelKey = "PlayerLevel";
    private const string XpKey = "PlayerXP";
    public static bool LevelUpScreenIsScalingUp;
    private void Start()
    {
        LoadProgress();
        UpdateUI();
        CloseLevelUpPanelButton.onClick.AddListener(CloseLevelUpPanel);
    }

    public void AddXp(int xp)
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
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
  
    }

    public void LevelUp()
    {
        if(SceneManager.GetActiveScene().buildIndex != 5)
        {
            currentLevel++;
            currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
            currentXp = 0; // Reset XP for the new levels

            // Increase earned gems and coins rewards for the next level
            earnedGems = Mathf.RoundToInt(initialGemsReward * Mathf.Pow(gemsRewardMultiplier, currentLevel - 1));
            earnedCoins = Mathf.RoundToInt(initialCoinsReward * Mathf.Pow(coinsRewardMultiplier, currentLevel - 1));
            levelUpPanel.SetActive(true);
            LevelUpScreenIsScalingUp = true;
            levelUpText.text = (currentLevel - 1).ToString();
            earnedGemsText.text = "+" + earnedGems.ToString();
            earnedCoinsText.text = "+" + earnedCoins.ToString();

            // Add earned gems and coins to the total
            AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, (int)earnedGems);
            AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, (int)earnedCoins);
            FruitManager.Instance.
            // Start animation coroutine
            StartCoroutine(ScaleUpPanel());
            UpdateUI(); // Update UI immediately after level up
        }
    }
    public void LevelUpGamePlay2()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentActiveLevel", 1);
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
        currentXp = 0; // Reset XP for the new levels

        // Increase earned gems and coins rewards for the next level
        earnedGems = Mathf.RoundToInt(initialGemsReward * Mathf.Pow(gemsRewardMultiplier, currentLevel - 1));
        earnedCoins = Mathf.RoundToInt(initialCoinsReward * Mathf.Pow(coinsRewardMultiplier, currentLevel - 1));
        levelUpPanel.SetActive(true);
        LevelUpScreenIsScalingUp = true;
        levelUpText.text = (currentLevel).ToString();
        earnedGemsText.text = "+" + earnedGems.ToString();
        earnedCoinsText.text = "+" + earnedCoins.ToString();

        // Add earned gems and coins to the total
        AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, (int)earnedGems);
        AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, (int)earnedCoins);
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
        if(SceneManager.GetActiveScene().buildIndex != 5)
        {
            levelText.text = currentLevel.ToString();
            xpBar.maxValue = currentXpRequirement;
            xpBar.value = currentXp;
            CurrentLevelProgress.text = currentXp + "/" + currentXpRequirement;
        }
        

        LosePanellevelText.text = currentLevel.ToString();
       
        LosePanelXpBar.maxValue = currentXpRequirement;
       
        LosePanelXpBar.value = currentXp;
        if(AdsCurrencyManager.instance != null)
        {
            AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, GemsBarText);
            AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, CoinsBarText);
        }

        // Display current XP progress
        
        LosePanelCurrentLevelProgress.text = currentXp + "/" + currentXpRequirement;

        Debug.Log(currentXpRequirement);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(XpKey, currentXp);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, GemsBarText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, CoinsBarText);
    }

    private void LoadProgress()
    {
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            currentXp = PlayerPrefs.GetInt(XpKey, 0);   
        }
        else
        {
            currentLevel = PlayerPrefs.GetInt(LevelKey, 1);
            currentXp = PlayerPrefs.GetInt(XpKey, 0);
        }
       
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, GemsBarText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, CoinsBarText);

    }

    public void CloseLevelUpPanel()
    {
        if(SceneManager.GetActiveScene().buildIndex == 5)
        {

            if (AdsManager.instance.isInitialize)
            {
                AdsManager.instance.ShowInterstitialAd();
            }
            else
            {
                SceneManager.LoadScene(5);
            }
        }
        else
        {
            AdsManager.instance.ShowInterstitialAd();
        }
        //FruitManager.Instance.EnableAllFruitsPhysics();
       
        levelUpPanel.SetActive(false);
        LevelUpScreenIsScalingUp = false;
        //Play ButtonSound
    }
}
