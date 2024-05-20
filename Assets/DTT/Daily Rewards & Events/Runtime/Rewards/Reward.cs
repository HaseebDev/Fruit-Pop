using System;
using UnityEngine;

namespace DTT.DailyRewards
{
    
    /// <summary>
    /// The status of a reward being able to be claimed.
    /// </summary>
    public enum RewardStatus
    {
        UNAVAILABLE = 0,
        AVAILABLE = 1,
        CLAIMED = 2,
    }

    /// <summary>
    /// A reward object, that holds how a single earned reward
    /// from a daily rewards system.
    /// </summary>
    [Serializable]
    public class Reward
    {

        /// <summary>
        /// the status of this reward
        /// </summary>
        [SerializeField]
        private RewardStatus _rewardStatus;

        /// <summary>
        /// the time the reward was claimed at.
        /// -1 means unclaimed
        /// </summary>
        [SerializeField]
        private int _claimedAt;

        /// <summary>
        /// The time the reward became/becomes available at.
        /// -1 means unknown.
        /// </summary>
        [SerializeField]
        private int availableAt;

        /// <summary>
        /// The name of the daily reward.
        /// </summary>
        [SerializeField]
        private string dailyRewardName;

        /// <summary>
        /// How far in the pattern is this reward. (is the streak count for rewards that are setup
        /// as streaks).
        /// </summary>
        [SerializeField]
        private int rewardCount;

        /// <summary>
        /// The time the rewards was claimed at.
        /// </summary>
        public int ClaimedAt => _claimedAt;

        /// <summary>
        /// The name of the daily reward.
        /// </summary>
        public string DailyRewardName => dailyRewardName;

        /// <summary>
        /// The time the reward became/becomes available at.
        /// -1 means unknown.
        /// </summary>
        public int AvailableAt => availableAt;

        /// <summary>
        /// How far in the pattern is this reward. (is the streak count for rewards that are setup
        /// as streaks).
        /// </summary>
        public int RewardCount => rewardCount;

        /// <summary>
        /// If the reward is available or not.
        /// </summary>
        public RewardStatus RewardStatus => _rewardStatus;

        /// <summary>
        /// Create a new reward object.
        /// </summary>
        /// <param name="rewardStatus">the current status of the reward</param>
        /// <param name="availableAt">when the reward becomes/became available (set to -1 if unknown)</param>
        /// <param name="claimedAt">When it was claimed (set to -1 if not claimed).</param>
        /// <param name="dailyRewardName">The name of the daily reward this reward came from.</param>
        /// <param name="rewardCount">The count of this reward (the streak number for rewards that are streaks.</param>
        public Reward(RewardStatus rewardStatus,int availableAt, int claimedAt, string dailyRewardName, int rewardCount)
        {
            _rewardStatus = rewardStatus;
            this.availableAt = availableAt;
            _claimedAt = claimedAt;
            this.dailyRewardName = dailyRewardName;
            this.rewardCount = rewardCount;
        }

        /// <summary>
        /// Claims the reward.
        /// </summary>
        /// <param name="timeClaimedAt">The time the reward was claimed at.</param>
        public void Claim(int timeClaimedAt)
        {
            _rewardStatus = RewardStatus.CLAIMED;
            _claimedAt = timeClaimedAt;
        }
    }
}