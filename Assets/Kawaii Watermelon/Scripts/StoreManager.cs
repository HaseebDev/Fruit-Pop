using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Image bg;
    public GameObject lockScreen, buyButton;
    [SerializeField] List<Item> bgs = new List<Item>();
    [SerializeField] List<Button> bgSelectionButtons = new List<Button>();
    int currentBgIndex;
    private const string LockStatesKey = "BgLockStates";
    private const string SelectedBgKey = "SelectedBg";
    public static StoreManager Instance;
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
        InitializeItems();
        LoadLockStates();
        LoadSelectedBg();

        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);

        for (int i = 0; i < bgSelectionButtons.Count; i++)
        {
            int index = i;  // Capture the current value of i
            bgSelectionButtons[index].onClick.AddListener(() => BgSelection(index));
        }

        UpdateUI();
    }

    private void InitializeItems()
    {
        if (!PlayerPrefs.HasKey(LockStatesKey))
        {
            bgs[0].locked = false;
            SaveLockStates();
        }
    }
    public void UpdateCurrencyUi()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
    private void LoadLockStates()
    {
        if (PlayerPrefs.HasKey(LockStatesKey))
        {
            string[] lockStates = PlayerPrefs.GetString(LockStatesKey).Split(',');
            for (int i = 0; i < lockStates.Length; i++)
            {
                bgs[i].locked = lockStates[i] == "1";
            }
        }
        else
        {
            InitializeItems();
        }

        for (int i = 0; i < bgs.Count; i++)
        {
            bgSelectionButtons[i].transform.GetChild(1).gameObject.SetActive(bgs[i].locked);
        }
    }

    private void SaveLockStates()
    {
        List<string> lockStates = new List<string>();
        foreach (var item in bgs)
        {
            lockStates.Add(item.locked ? "1" : "0");
        }
        PlayerPrefs.SetString(LockStatesKey, string.Join(",", lockStates));
        PlayerPrefs.Save();
    }

    private void LoadSelectedBg()
    {
        if (PlayerPrefs.HasKey(SelectedBgKey))
        {
            int selectedIndex = PlayerPrefs.GetInt(SelectedBgKey);
            BgSelection(selectedIndex);
        }
        else
        {
            BgSelection(0); // Select the first item by default
        }
    }

    public void SaveSelectedBg()
    {
        for (int i = 0; i < bgSelectionButtons.Count; i++)
        {
            bgSelectionButtons[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        PlayerPrefs.SetInt(SelectedBgKey, currentBgIndex);
        bgSelectionButtons[currentBgIndex].transform.GetChild(0).gameObject.SetActive(true);
        PlayerPrefs.Save();
    }

    public void BgSelection(int index)
    {
        currentBgIndex = index;
        bg.sprite = bgs[index].bgSprite;


        if (bgs[index].locked)
        {
            // Show lock screen and buy button
            bgSelectionButtons[index].transform.GetChild(1).gameObject.SetActive(true);
            lockScreen.SetActive(true);
            buyButton.SetActive(true);
            //selectButton.SetActive(false);
            priceText.text = bgs[index].playerCosts.ToString();
        }
        else
        {
            // Show select button and hide buy button
            bgSelectionButtons[index].transform.GetChild(1).gameObject.SetActive(false);
            lockScreen.SetActive(false);
            buyButton.SetActive(false);
            //selectButton.SetActive(true);
            SaveSelectedBg();
        }
    }

    public void BuyBackground()
    {
        Item selectedItem = bgs[currentBgIndex];

        if (AdsCurrencyManager.instance.SpendCurrency(selectedItem.currencyType, selectedItem.playerCosts))
        {
            selectedItem.locked = false;
            SaveLockStates();
            UpdateUI();
            BgSelection(currentBgIndex);  // Refresh UI for selected item
        }
    }

    private void UpdateUI()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
}

[Serializable]
public class Item
{
    public Sprite bgSprite;
    public int playerCosts;
    public bool locked;
    public CurrencyType currencyType;
}
