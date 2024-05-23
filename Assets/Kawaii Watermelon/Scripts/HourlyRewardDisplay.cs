using DTT.DailyRewards;
using DTT.DailyRewards.Demo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple display system for the daily rewards, showing when
/// The reward can be claimed, along with previous claims
/// </summary>
public class HourlyRewardDisplay : MonoBehaviour
{
    /// <summary>
    /// The daily reward to display.
    /// </summary>
    [SerializeField]
    private DailyReward dailyReward;

    /// <summary>
    /// A progress bar that shows how close to being able
    /// to retrieve the reward we are.
    /// </summary>
    [SerializeField]
    private TMP_Text timerText;

    /// <summary>
    /// A button that attempts to claim a reward.
    /// </summary>
    [SerializeField]
    private Button claimButton;

    [SerializeField]
    public int[] rewardAmounts;
    [SerializeField]
    public GameObject rewardPanel;
    public TMP_Text rewardText;

    /// <summary>
    /// A lock on updating the display, to stop interference during
    /// it's asynchronous callback.
    /// </summary>
    private bool _updatingDisplayLock;


    /// <summary>
    /// Add a listener to the claim rewards button.
    /// </summary>
    private void Start()
    {
        claimButton.onClick.AddListener(() => dailyReward.RewardInstance.ClaimReward(result =>
        {
            switch (result.CurrentCallbackStatus)
            {

                case RewardCallbackStatus.PENDING:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} is being claimed.");
                    break;
                case RewardCallbackStatus.COMPLETE:
                    int randomRewardAmount = rewardAmounts[UnityEngine.Random.Range(0, rewardAmounts.Length)];
                    rewardText.text = randomRewardAmount.ToString();
                    rewardPanel.SetActive(true);
                    AdsCurrencyManager.instance.EarnCurrency(CurrencyType.Common, randomRewardAmount);
                    MainMenuManager.Instance.UpdateCurrencyUi();
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} successfully claimed.");

                    break;
                case RewardCallbackStatus.NO_REWARD_AVAILABLE:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} is not available to be claimed.");
                    break;
                case RewardCallbackStatus.ALL_REWARDS_EARNED:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} has had all rewards claimed.");

                    break;
                case RewardCallbackStatus.COULD_NOT_VALIDATE_TIME:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} could not validate the time.");

                    break;
                case RewardCallbackStatus.COULD_NOT_SAVE_REWARD:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} could not save the reward.");
                    break;
                case RewardCallbackStatus.REWARDS_NOT_LOADED:
                    Debug.Log($"Reward {dailyReward.RewardInstance.name} has not yet loaded the rewards.");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }));
    }

    /// <summary>
    /// Update the display every frame.
    /// </summary>
    private void Update()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Update the display with the given rewards.
    /// </summary>
    protected void UpdateDisplay()
    {
        if (_updatingDisplayLock)
            return;
        _updatingDisplayLock = true;
        dailyReward.RewardInstance.CheckAvailableReward(nextAvailableReward =>
        {
            if (nextAvailableReward.CurrentCallbackStatus == RewardCallbackStatus.PENDING)
            {
                if (claimButton.interactable)
                {
                    claimButton.interactable = false;
                }
                return;
            }

            _updatingDisplayLock = false;
            float timeLeft = nextAvailableReward.NextAvailableReward - nextAvailableReward.CurrentUnixTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeft);
            int totalHours = (int)timeSpan.TotalHours;
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                totalHours,
                timeSpan.Minutes,
                timeSpan.Seconds);

            // Update the timerText
            timerText.text = formattedTime;
            if (nextAvailableReward.CurrentCallbackStatus == RewardCallbackStatus.COMPLETE)
            {
                if (!claimButton.interactable)
                {
                    claimButton.interactable = true;
                }
                timerText.text = "Claim";
            }
        });
    }
}
