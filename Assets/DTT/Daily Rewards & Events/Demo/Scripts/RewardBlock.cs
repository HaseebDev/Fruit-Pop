using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.DailyRewards.Demo
{
    /// <summary>
    /// A reward block is a simple displayer of the state of a reward.
    /// </summary>
    public class RewardBlock : MonoBehaviour
    {

        /// <summary>
        /// The background color object of the block.
        /// </summary>
        [SerializeField]
        private Image backgroundColour;

        /// <summary>
        /// The text of the object.
        /// </summary>
        [SerializeField]
        private Text rewardCountText;

        /// <summary>
        /// The color of a claimed reward.
        /// </summary>
        [SerializeField]
        private Color _claimableColor = Color.green;
        
        /// <summary>
        /// The color of a claimable reward.
        /// </summary>
        [SerializeField]
        private Color _claimedColor = Color.cyan;

        /// <summary>
        /// The color of an invalid (non-existent) reward.
        /// </summary>
        [SerializeField]
        private Color _invalidColor = Color.red;

        /// <summary>
        /// The color of an unavailable reward.
        /// </summary>
        [SerializeField]
        private readonly Color _unavailableColor = Color.grey;

        /// <summary>
        /// The reward being displayed by the block.
        /// </summary>
        private Reward _reward;




        /// <summary>
        /// Display a given reward.
        /// </summary>
        /// <param name="newReward">The reward to display.</param>
        public void UpdateReward(Reward newReward)
        {

            //If it's the same reward, no need to update.
            if (_reward != null && newReward != null && _reward.RewardCount == newReward.RewardCount && _reward.RewardStatus == newReward.RewardStatus)
                return;
            _reward = newReward;
            UpdateDisplay();
        }

        /// <summary>
        /// Update the display of the reward
        /// </summary>
        private void UpdateDisplay()
        {
            if (_reward == null)
            {
                backgroundColour.color = _invalidColor;
                rewardCountText.text = "";
                return;
            }
            rewardCountText.text = _reward.RewardCount.ToString();
            switch (_reward.RewardStatus)
            {
                case RewardStatus.UNAVAILABLE:
                    backgroundColour.color = _unavailableColor;
                    rewardCountText.text = "";
                    break;
                case RewardStatus.AVAILABLE:
                    backgroundColour.color = _claimableColor;
                    break;
                case RewardStatus.CLAIMED:
                    backgroundColour.color = _claimedColor;
                    break;
            }
        }
    }
}