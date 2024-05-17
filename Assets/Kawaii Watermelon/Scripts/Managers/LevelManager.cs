using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider xpBar;

    [Header("Settings")]
    [SerializeField] private int initialXpRequirement = 100;
    [SerializeField] private float xpRequirementMultiplier = 1.5f;

    private int currentLevel;
    private int currentXp;
    private int currentXpRequirement;

    private const string LevelKey = "PlayerLevel";
    private const string XpKey = "PlayerXP";

    private void Start()
    {
        LoadProgress();
        UpdateUI();
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
        UpdateUI(); // Update UI immediately after level up
    }

    private void UpdateUI()
    {
        levelText.text = "Level: " + currentLevel;
        xpBar.maxValue = currentXpRequirement;
        xpBar.value = currentXp;
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(XpKey, currentXp);
    }

    private void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1);
        currentXp = PlayerPrefs.GetInt(XpKey, 0);
        currentXpRequirement = Mathf.RoundToInt(initialXpRequirement * Mathf.Pow(xpRequirementMultiplier, currentLevel - 1));
    }
}
