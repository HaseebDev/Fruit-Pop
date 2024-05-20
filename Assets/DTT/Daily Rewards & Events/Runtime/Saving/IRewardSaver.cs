using System;
using System.Collections.Generic;

namespace DTT.DailyRewards
{
    /// <summary>
    /// Interface for a reward saver/loader.
    /// Can be inherited to save the rewards in custom ways.
    /// e.g. remote/in files.
    /// </summary>
    public interface IRewardSaver
    {
        /// <summary>
        /// Load the saved rewards
        /// </summary>
        /// <param name="rewardsCallback">The result of loading.</param>
        void LoadRewards(Action<LoadRewardCallback> rewardsCallback);

        /// <summary>
        /// Store the rewards (overwriting the currently saved rewards).
        /// </summary>
        /// <param name="rewards">The rewards to be saved.</param>
        /// <param name="successCallback">Callback indicating if saving was successful.</param>
        void StoreRewards(List<Reward> rewards, Action<bool> successCallback);

        /// <summary>
        /// Clears all saved rewards.
        /// </summary>
        /// <param name="successful">Callback indicating if clearing or rewards was successful.</param>
        void ClearRewards(Action<bool> successful);
    }
}