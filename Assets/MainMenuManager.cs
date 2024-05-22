using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TMP_Text commonCurrencyText;
    [SerializeField] TMP_Text rareCurrencyText;
    // Start is called before the first frame update
    void Start()
    {
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Common, commonCurrencyText);
        AdsCurrencyManager.instance.UpdateCurrencyUI(CurrencyType.Rare, rareCurrencyText);
    }
}
