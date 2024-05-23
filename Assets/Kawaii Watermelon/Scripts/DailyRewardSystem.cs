using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DailyRewardSystem : MonoBehaviour
{
    private const string LastClaimedDateKey = "LastClaimedDate";
    private const string RewardDayKey = "RewardDay";
    private const int TotalDays = 7;
    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/Europe/Amsterdam";
    bool currentRewardClaimed;
    public bool isLocal = true;
    public List<Button> dayButtons; // List of buttons for each reward day

    void Start()
    {
        InitializeButtonListeners();

        if (isLocal)
        {
            CheckRewards(DateTime.Now);
        }
        else
        {
            StartCoroutine(GetInternetTime());
        }

    }

    private IEnumerator GetInternetTime()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(TimeApiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error getting internet time: " + webRequest.error);
                // Fallback to local time if internet time fails
                CheckRewards(DateTime.Now);
            }
            else
            {
                string jsonResult = webRequest.downloadHandler.text;
                DateTime internetTime = ParseInternetTime(jsonResult);
                CheckRewards(internetTime);
            }
        }
    }

    private DateTime ParseInternetTime(string json)
    {
        try
        {
            var jsonObject = JsonUtility.FromJson<TimeApiResponse>(json);
            return DateTime.Parse(jsonObject.datetime);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse internet time: " + e.Message);
            return DateTime.Now; // Fallback to local time if parsing fails
        }
    }

    [Serializable]
    private class TimeApiResponse
    {
        public string datetime;
    }

    void CheckRewards(DateTime currentDate)
    {
        string lastClaimedDateStr = PlayerPrefs.GetString(LastClaimedDateKey, string.Empty);
        int rewardDay = PlayerPrefs.GetInt(RewardDayKey, 1);

        if (string.IsNullOrEmpty(lastClaimedDateStr))
        {
            // No reward has been claimed yet
            Debug.Log("No rewards claimed yet. Starting at Day 1.");
            ShowReward(1);
            return;
        }

        DateTime lastClaimedDate = DateTime.Parse(lastClaimedDateStr);
        int daysDifference = (currentDate.Date - lastClaimedDate.Date).Days;

        if (daysDifference == 0)
        {
            // Reward already claimed today
            Debug.Log($"Reward already claimed today. Current reward day: {rewardDay}");
            currentRewardClaimed = true;
        }
        else if (daysDifference == 1)
        {
            // Reward can be claimed
            rewardDay = (rewardDay % TotalDays) + 1;
            ShowReward(rewardDay);
        }
        else
        {
            // Missed reward day(s)
            Debug.Log("Missed one or more days. Restarting at Day 1.");
            ShowReward(1);
        }

        UpdateButtons(rewardDay);
    }

    void ShowReward(int day)
    {
        Debug.Log($"Showing reward for Day {day}");
        // You can display the reward UI or grant the reward here

        // Set the next reward day in PlayerPrefs
        PlayerPrefs.SetInt(RewardDayKey, day);

        UpdateButtons(day);
    }

    void UpdateButtons(int rewardDay)
    {

        for (int i = 0; i < dayButtons.Count; i++)
        {
            if (dayButtons[i] != null)
            {
                if (!currentRewardClaimed)
                {
                    // If reward is claimed, activate the first child
                    if (PlayerPrefs.GetInt(RewardDayKey, 1) > i + 1)
                    {
                        if (dayButtons[i].transform.childCount > 0)
                        {
                            dayButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (dayButtons[i].transform.childCount > 0)
                        {
                            dayButtons[i].transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    // If reward is claimed, activate the first child
                    if (PlayerPrefs.GetInt(RewardDayKey, 1) >= i + 1)
                    {
                        if (dayButtons[i].transform.childCount > 0)
                        {
                            dayButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (dayButtons[i].transform.childCount > 0)
                        {
                            dayButtons[i].transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }

                dayButtons[i].interactable = (i + 1 == rewardDay && !currentRewardClaimed);

            }
        }
    }

    void InitializeButtonListeners()
    {
        for (int i = 0; i < dayButtons.Count; i++)
        {
            int day = i + 1; // Local copy to avoid closure issue in the loop
            dayButtons[i].onClick.AddListener(() => ClaimReward(day));
        }
    }

    public void ClaimReward(int day)
    {
        DateTime currentDate = isLocal ? DateTime.Now : DateTime.UtcNow;
        string lastClaimedDateStr = PlayerPrefs.GetString(LastClaimedDateKey, string.Empty);
        int rewardDay = PlayerPrefs.GetInt(RewardDayKey, 1);

        if (string.IsNullOrEmpty(lastClaimedDateStr) || (currentDate.Date - DateTime.Parse(lastClaimedDateStr).Date).Days != 0)
        {
            // Update the last claimed date and reward day
            PlayerPrefs.SetString(LastClaimedDateKey, currentDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.SetInt(RewardDayKey, rewardDay);
            PlayerPrefs.Save();
            // Debug which day is claimed
            switch (rewardDay)
            {
                case 1:
                    Debug.Log("Claimed reward for Day 1");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 200);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 2:
                    Debug.Log("Claimed reward for Day 2");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, 10);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 3:
                    Debug.Log("Claimed reward for Day 3");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 500);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 4:
                    Debug.Log("Claimed reward for Day 4");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, 20);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 5:
                    Debug.Log("Claimed reward for Day 5");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 1000);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 6:
                    Debug.Log("Claimed reward for Day 6");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, 30);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                case 7:
                    Debug.Log("Claimed reward for Day 7");
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, 1500);
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Rare, 50);
                    dayButtons[rewardDay - 1].transform.GetChild(0).gameObject.SetActive(true);
                    dayButtons[rewardDay - 1].interactable = false;
                    break;
                default:
                    Debug.LogError("Invalid reward day");
                    break;
            }
            MainMenuManager.Instance.UpdateCurrencyUi();
        }
        else
        {
            Debug.Log("Reward already claimed today.");
        }

    }
}
