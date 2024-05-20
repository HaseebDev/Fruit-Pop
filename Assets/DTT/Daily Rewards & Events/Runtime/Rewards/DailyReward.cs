using System;
using UnityEngine;

namespace DTT.DailyRewards
{
    /// <summary>
    /// The daily Reward is a mono behaviour wrapper for the <see cref="dailyRewardSO"/>,
    /// which allows a single SO to be shared across multiple places.
    /// </summary>
    public class DailyReward : MonoBehaviour
    {
        /// <summary>
        /// A daily reward object that is implemented here.
        /// </summary>
        [SerializeField]
        private DailyRewardSO dailyRewardSO;

        /// <summary>
        /// If this daily reward should initialise on start.
        /// Set to false if you want to use custom savers or time validators.
        /// </summary>
        [SerializeField]
        private bool initialiseOnStart = false;


        /// <summary>
        /// Return the instance (so multiple Mono Behaviours can access the same daily reward from the different locations).
        /// </summary>
        public DailyRewardSO RewardInstance
        {
            get
            {
                if (!dailyRewardSO.Loaded)
                {
                    Debug.LogWarning($"reward {dailyRewardSO.name} accessed before setup, results may be empty/incorrect");
                }
                return dailyRewardSO;
            }
        }



        /// <summary>
        /// Initialise if set to initialise on start.
        /// </summary>
        public void Start()
        {
            if (initialiseOnStart)
                Initialise(null);
        }

        /// <summary>
        /// Only call initialise once, either on startup, or through another script if you wish to
        /// Give custom parameters, and get callbacks on loading progress.
        /// </summary>
        /// <param name="loadingProgress">Callback with the loading progress.</param>
        /// <param name="customSaver">The custom saver for the rewards if wanted.</param>
        /// <param name="customRetriever">Custom time validator to override the default one.</param>
        public void Initialise(Action<LoadState> loadingProgress, IRewardSaver customSaver = null,
            ITimeRetriever customRetriever = null)
        {
            dailyRewardSO.Initialise((progress) =>
            {
                loadingProgress?.Invoke(progress);
            }, customSaver, customRetriever);
        }
    }
}