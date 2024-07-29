using System;
using TMPro;
using UnityEngine;

public class BoostersHandler : MonoBehaviour
{
    public GameObject PowerFreezePlus;
    public GameObject PowerFreezePanel;
    public GameObject PowerUpPanel;
    public GameObject PowerUpPlus;
    public TMP_Text rareCoin;
    string boosterName;
    private void Start()
    {
        PowerUpPlus.SetActive(PlayerPrefs.GetString("PowerUp", "NotBuy") != "Buy");
        PowerFreezePlus.SetActive(PlayerPrefs.GetString("PowerFreeze", "NotBuy") != "Buy");
    }
    public void PowerUpBtn()
    {
        if (PlayerPrefs.GetString("PowerUp", "NotBuy") == "Buy")
        {
            GamePlay2 gamePlay2 = FindObjectOfType<GamePlay2>();
            gamePlay2.targetTime += 60;
            PlayerPrefs.SetString("PowerUp", "NotBuy");
            PowerUpPlus.SetActive(true);
        }
        else
        {
           
            PowerUpPanel.SetActive(true);
        }
        
    }
    public void PowerFreezeBtn()
    {
        if (PlayerPrefs.GetString("PowerFreeze", "NotBuy") == "Buy")
        {
            GamePlay2 gamePlay2 = FindObjectOfType<GamePlay2>();
            gamePlay2.TimerStop = true;
            PlayerPrefs.SetString("PowerFreeze", "NotBuy");
            PowerFreezePlus.SetActive(true);
        }
        else
        {
           
            PowerFreezePanel.SetActive(true);
        }

    }
    public void PowerFreeze()
    {
        if (PlayerPrefs.GetFloat("RareCurrency") >= 15)
        {
            PlayerPrefs.SetFloat("RareCurrency", PlayerPrefs.GetFloat("RareCurrency") - 14);
            PowerFreezePanel.SetActive(false);
            PowerFreezePlus.SetActive(false);
            PlayerPrefs.SetString("PowerFreeze","Buy");
            PlayerPrefs.Save();
            rareCoin.text = (PlayerPrefs.GetFloat("RareCurrency")).ToString();
        }
    }
    public void PowerUp()
    {
        if (PlayerPrefs.GetFloat("RareCurrency") >= 15)
        {
            PlayerPrefs.SetFloat("RareCurrency", PlayerPrefs.GetFloat("RareCurrency") - 15);
            PowerUpPanel.SetActive(false);
            PowerUpPlus.SetActive(false);
            PlayerPrefs.SetString("PowerUp", "Buy");
            PlayerPrefs.Save();
            rareCoin.text = (PlayerPrefs.GetFloat("RareCurrency")).ToString();
        }
    }
   
    public void TimeUpByAds(String name)
    {
        boosterName = name;
        Action Reward = getBoosterReward;
        AdsManager.instance.ShowRewardedAd(Reward);
    }
    
    private void getBoosterReward()
    {
        if (boosterName.Equals("up"))
        {
            PlayerPrefs.SetString("PowerUp", "Buy");
            PowerUpPlus.SetActive(false);
            PowerUpPanel.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetString("PowerFreeze", "Buy");
            PowerFreezePlus.SetActive(false);
            PowerFreezePanel.SetActive(false);
        }
    }
}
