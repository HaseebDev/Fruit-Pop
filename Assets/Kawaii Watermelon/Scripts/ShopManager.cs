using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    [SerializeField] Image gem;
    [SerializeField] Sprite gemDisabled;
    [SerializeField] Sprite gemEnabled;
    [SerializeField] GameObject gemHolder;
    [SerializeField] Image coin;
    [SerializeField] Sprite coinDisabled;
    [SerializeField] Sprite coinEnabled;
    [SerializeField] GameObject coinHolder;


    // Start is called before the first frame update
    void Start()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
        OpenCoinsTab();
    }

    public void OpenGemTab()
    {
        coin.sprite = coinDisabled;
        coin.transform.GetChild(0).gameObject.SetActive(false);
        coinHolder.SetActive(false);
        gem.sprite = gemEnabled;
        gem.transform.GetChild(0).gameObject.SetActive(true);
        gemHolder.SetActive(true);
    }
    public void OpenCoinsTab()
    {

        gem.sprite = gemDisabled;
        gem.transform.GetChild(0).gameObject.SetActive(false);
        gemHolder.SetActive(false);
        coin.sprite = coinEnabled;
        coin.transform.GetChild(0).gameObject.SetActive(true);
        coinHolder.SetActive(true);
    }

    public void AddGems(int amount)
    {
        AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, amount);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
    public void AddCoins(int index)
    {
        switch (index)
        {
            case 0:
                if (AdsCurrencyManager.instance.SpendCurrency(CurrencyType.Rare, 12))
                {
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 1500);
                }
                else
                {
                    OpenGemTab();
                }
                break;
            case 1:
                if (AdsCurrencyManager.instance.SpendCurrency(CurrencyType.Rare, 48))
                {
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 4000);
                }
                else
                {
                    OpenGemTab();
                }
                break;
            case 2:
                if (AdsCurrencyManager.instance.SpendCurrency(CurrencyType.Rare, 120))
                {
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 12000);
                }
                else
                {
                    OpenGemTab();
                }
                break;
            case 3:
                if (AdsCurrencyManager.instance.SpendCurrency(CurrencyType.Rare, 240))
                {
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 25000);
                }
                else
                {
                    OpenGemTab();
                }
                break;
            case 4:
                if (AdsCurrencyManager.instance.SpendCurrency(CurrencyType.Rare, 480))
                {
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 60000);
                }
                else
                {
                    OpenGemTab();
                }
                break;
            default:
                Debug.LogError("Invalid Purchase");
                break;

        }
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);

    }
}
