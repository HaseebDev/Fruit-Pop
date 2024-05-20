using TMPro;
using UnityEngine;
public enum CurrencyType
{
    Common,
    Rare
}
public class AdsCurrencyManager : MonoBehaviour
{
    public static AdsCurrencyManager instance;
    private double commonCurrency;
    private double rareCurrency;
    private string commonCurrencyKey = "CommonCurrency";
    private string rareCurrencyKey = "RareCurrency";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        LoadCurrencies();
    }
    private void LoadCurrencies()
    {
        if (PlayerPrefs.HasKey(commonCurrencyKey))
        {
            commonCurrency = PlayerPrefs.GetFloat(commonCurrencyKey);
        }
        else
        {
            commonCurrency = 1000;
        }
        if (PlayerPrefs.HasKey(rareCurrencyKey))
        {
            rareCurrency = PlayerPrefs.GetFloat(rareCurrencyKey);
        }
        else
        {
            rareCurrency = 20;
        }
    }
    public void SaveCurrencies()
    {
        PlayerPrefs.SetFloat(commonCurrencyKey, (float)commonCurrency);
        PlayerPrefs.SetFloat(rareCurrencyKey, (float)rareCurrency);
        PlayerPrefs.Save();
    }
    public string FormatCurrencyValue(double value)
    {
        if (value >= 1_000_000_000_000)
        {
            return (value / 1_000_000_000_000).ToString("0.##") + "T";
        }
        else if (value >= 1_000_000_000)
        {
            return (value / 1_000_000_000).ToString("0.##") + "B";
        }
        else if (value >= 1_000_000)
        {
            return (value / 1_000_000).ToString("0.##") + "M";
        }
        else if (value >= 1_000)
        {
            return (value / 1_000).ToString("0.##") + "K";
        }
        else
        {
            return value.ToString();
        }
    }
    private void AddCommonCurrency(double amount)
    {
        commonCurrency += amount;
        SaveCurrencies();
    }
    private void AddRareCurrency(double amount)
    {
        rareCurrency += amount;
        SaveCurrencies();
    }
    private void DeductCommonCurrency(double amount)
    {
        if (commonCurrency >= amount)
        {
            commonCurrency -= amount;
            SaveCurrencies();
        }
        else
        {
            Debug.LogWarning("Insufficient common currency!");
        }
    }
    private void DeductRareCurrency(double amount)
    {
        if (rareCurrency >= amount)
        {
            rareCurrency -= amount;
            SaveCurrencies();
        }
        else
        {
            Debug.LogWarning("Insufficient rare currency!");
        }
    }
    private double GetCurrencyAmount(CurrencyType currencyType)
    {
        LoadCurrencies();
        if (currencyType == CurrencyType.Common)
        {
            return commonCurrency;
        }
        else if (currencyType == CurrencyType.Rare)
        {
            return rareCurrency;
        }
        else
        {
            Debug.LogWarning("Invalid currency type!");
            return 0;
        }
    }
    private void DeductCurrency(CurrencyType currencyType, double amount)
    {
        LoadCurrencies();
        if (currencyType == CurrencyType.Common)
        {
            DeductCommonCurrency(amount);
        }
        else if (currencyType == CurrencyType.Rare)
        {
            DeductRareCurrency(amount);
        }
        else
        {
            Debug.LogWarning("Invalid currency type!");
        }
    }
    private void AddCurrency(CurrencyType currencyType, double amount)
    {
        LoadCurrencies();
        if (currencyType == CurrencyType.Common)
        {
            AddCommonCurrency(amount);
        }
        else if (currencyType == CurrencyType.Rare)
        {
            AddRareCurrency(amount);
        }
        else
        {
            Debug.LogWarning("Invalid currency type!");
        }
    }
    public bool CanAfford(CurrencyType currencyType, double cost)
    {
        return GetCurrencyAmount(currencyType) >= cost;
    }
    public bool SpendCurrency(CurrencyType currencyType, double amount)
    {
        if (CanAfford(currencyType, amount))
        {
            DeductCurrency(currencyType, amount);
            return true;
        }
        else
        {
            Debug.LogWarning("Insufficient funds!");
            return false;
        }
    }
    public void EarnCurrency(CurrencyType currencyType, double amount)
    {
        AddCurrency(currencyType, amount);
    }
    public void UpdateCurrencyUI(CurrencyType currencyType, TMP_Text currencyText)
    {
        LoadCurrencies();
        currencyText.text = FormatCurrencyValue(GetCurrencyAmount(currencyType));
    }
}