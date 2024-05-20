using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DTT.DailyRewards
{
    /// <summary>
    /// A callback for when rewards are loaded. holding the rewards, and the
    /// state of loading.
    /// </summary>
    public class LoadRewardCallback
    {
        /// <summary>
        /// The loaded rewards, if any.
        /// </summary>
        private readonly List<Reward> _rewards;

        /// <summary>
        /// The loadedRewards, if any.
        /// </summary>
        public ReadOnlyCollection<Reward> Rewards => _rewards?.AsReadOnly();

        /// <summary>
        /// if loading was successful.
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// The callback showing the success of loading/saving.
        /// </summary>
        /// <param name="rewards"></param>
        /// <param name="success"></param>
        public LoadRewardCallback(List<Reward> rewards, bool success)
        {
            _rewards = rewards;
            Success = success;
        }
    }
}