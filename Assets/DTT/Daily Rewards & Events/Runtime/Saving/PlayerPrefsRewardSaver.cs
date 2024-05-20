using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.DailyRewards
{
    /// <summary>
    /// The player prefs reward saver saves earned rewards under the
    /// player pref key "reward_{rewardId}".
    /// WARNING, clearing player prefs will clear these saved awards.
    /// </summary>
    public class PlayerPrefsRewardSaver : IRewardSaver
    {
        /// <summary>
        /// The reward name
        /// </summary>
        private readonly string _rewardId;

        /// <summary>
        /// The location the reward is saved in player prefs
        /// </summary>
        private string RewardSaveLocation => $"reward_{_rewardId}";

        /// <summary>
        /// give the name of the reward to be stored.
        /// </summary>
        /// <param name="rewardId"></param>
        public PlayerPrefsRewardSaver(string rewardId)
        {
            _rewardId = rewardId;
        }

        /// <summary>
        /// Load the rewards from player prefs
        /// </summary>
        /// <param name="rewardsCallback">the loaded rewards.</param>
        public void LoadRewards(Action<LoadRewardCallback> rewardsCallback)
        {
            string playerPrefRewards = PlayerPrefs.GetString(RewardSaveLocation, null);
            Reward[] rewards = (string.IsNullOrEmpty(playerPrefRewards)
                ? Array.Empty<Reward>()
                : JsonHelper.FromJson<Reward>(playerPrefRewards)) ?? Array.Empty<Reward>();

            LoadRewardCallback callback = new LoadRewardCallback(rewards.ToList(), true);
            rewardsCallback?.Invoke(callback);
        }

        /// <summary>
        /// Store the earned rewards in player prefs.
        /// </summary>
        /// <param name="rewards">The rewards to be stored.</param>
        /// <param name="successCallback">Callback on success.</param>
        public void StoreRewards(List<Reward> rewards, Action<bool> successCallback)
        {
            string jsonRewards = JsonHelper.ToJson(rewards.ToArray());
            PlayerPrefs.SetString(RewardSaveLocation, jsonRewards);
            successCallback?.Invoke(true);
        }

        /// <summary>
        /// Clear all stored rewards.
        /// </summary>
        /// <param name="successful">True if successful (will always be true).</param>
        public void ClearRewards(Action<bool> successful)
        {
            PlayerPrefs.DeleteKey(RewardSaveLocation);
            successful?.Invoke(true);
        }

    }
}