using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider xpBar;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private TextMeshProUGUI earnedGemsText;
    [SerializeField] private TextMeshProUGUI earnedCoinsText;
    [SerializeField] private TextMeshProUGUI GemsBarText;
    [SerializeField] private TextMeshProUGUI CoinsBarText;
    [SerializeField] private Button CloseLevelUpPanelButton;

    [Header("Settings")]
    [SerializeField] private int initialXpRequirement = 25;
    [SerializeField] private float xpRequirementMultiplier = 1.5f;
    [SerializeField] private int initialGemsReward = 1;
    [SerializeField] private int initialCoinsReward = 20;
    [SerializeField] private float gemsRewardMultiplier = 1.2f;
    [SerializeField] private float coinsRewardMultiplier = 1.3f;
    [SerializeField] private float animationDuration = 1f;

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
    }

    public void AddXp(int xp)
    {
        currentXp += xp;

        while (currentXp >= currentXpRequirement)
        {
            currentXp -= currentXpRequirement;
            LevelUp();
        }

        UpdateUI();
        SaveProgress();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
        currentXp = 0; // Reset XP for the new level

        // Increase earned gems and coins rewards for the next level
        earnedGems = Mathf.RoundToInt(initialGemsReward * Mathf.Pow(gemsRewardMultiplier, currentLevel - 1));
        earnedCoins = Mathf.RoundToInt(initialCoinsReward * Mathf.Pow(coinsRewardMultiplier, currentLevel - 1));

        UpdateUI(); // Update UI immediately after level up

        // Enable level up panel and display completed level
        levelUpPanel.SetActive(true);
        levelUpText.text = (currentLevel - 1).ToString();
        earnedGemsText.text = earnedGems.ToString();
        earnedCoinsText.text = earnedCoins.ToString();

        // Start animation coroutine
        StartCoroutine(ScaleUpPanel());
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
        GemsBarText.text = PlayerPrefs.GetInt(GemsKey).ToString();
        CoinsBarText.text = PlayerPrefs.GetInt(CoinsKey).ToString();
    }

    private void UpdateUI()
    {
        levelText.text = currentLevel.ToString();
        xpBar.maxValue = currentXpRequirement;
        xpBar.value = currentXp;
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
