using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BoostersHandler : MonoBehaviour
{
    public GameObject PowerFreezePanel;
    public GameObject PowerUpPanel;
    public TMP_Text rareCoin;
    private string boosterName;

    private void Start()
    {
        rareCoin.text = PlayerPrefs.GetFloat("RareCurrency").ToString();
    }

    public void PowerUpBtn()
    {
        PowerUpPanel.SetActive(true);
    }

    public void PowerFreezeBtn()
    {
        PowerFreezePanel.SetActive(true);
    }

    public void PowerFreeze()
    {
        if (PlayerPrefs.GetFloat("RareCurrency") >= 15f)
        {
            PlayerPrefs.SetFloat("RareCurrency", PlayerPrefs.GetFloat("RareCurrency") - 15f);
            ApplyPowerFreezeEffect();
        }
    }

    public void PowerUp()
    {
        if (PlayerPrefs.GetFloat("RareCurrency") >= 15f)
        {
            PlayerPrefs.SetFloat("RareCurrency", PlayerPrefs.GetFloat("RareCurrency") - 15f);
            ApplyPowerUpEffect();
        }
    }

    private void ApplyPowerFreezeEffect()
    {
        GamePlay2 gamePlay2 = FindObjectOfType<GamePlay2>();
        gamePlay2.TimerStop = true;
        rareCoin.text = PlayerPrefs.GetFloat("RareCurrency").ToString();
        PowerFreezePanel.SetActive(false);
        StartCoroutine(ResetTimerStopAfterDelay(10f));
    }

    private void ApplyPowerUpEffect()
    {
        GamePlay2 gamePlay2 = FindObjectOfType<GamePlay2>();
        gamePlay2.targetTime += 20;
        rareCoin.text = PlayerPrefs.GetFloat("RareCurrency").ToString();
        PowerUpPanel.SetActive(false);
    }

    public void TimeUpByAds(string name)
    {
        boosterName = name;
        Action reward = GetBoosterReward;
        AdsManager.instance.ShowRewardedAd(reward);
    }

    private void GetBoosterReward()
    {
        if (boosterName.Equals("up"))
        {
            ApplyPowerUpEffect();
        }
        else
        {
            ApplyPowerFreezeEffect();
        }
    }

    private IEnumerator ResetTimerStopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GamePlay2 gamePlay2 = FindObjectOfType<GamePlay2>();
        if (gamePlay2 != null)
        {
            gamePlay2.TimerStop = false; 
        }
    }
}
