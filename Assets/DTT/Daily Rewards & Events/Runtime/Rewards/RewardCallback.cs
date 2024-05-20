namespace DTT.DailyRewards
{
    /// <summary>
    /// The state of the rewards callback.
    /// Pending = request till in progress
    /// Complete = Request was successful (assume a reward has been received).
    /// No reward available = No reward was available. (should have a time till reward available).
    /// All rewards available = All rewards have been earned for this finite reward.
    /// Could not validate time = the time validator has failed (no internet most likely if remote).
    /// Could not save reward = the reward saver has failed (no internet most likely if remote).
    /// </summary>
    public enum RewardCallbackStatus
    {
        PENDING = 0,
        COMPLETE = 1,
        NO_REWARD_AVAILABLE = 2,
        ALL_REWARDS_EARNED =3, //When a finite reward system is finished
        COULD_NOT_VALIDATE_TIME = 4,
        COULD_NOT_SAVE_REWARD = 5,
        REWARDS_NOT_LOADED = 6,
    }

    /// <summary>
    /// A callback object giving the state when you get if a reward is available or
    /// if you are checking if you can claim it.
    /// </summary>
    public class RewardCallback
    {
        /// <summary>
        /// The current reward state.
        /// </summary>
        public readonly RewardCallbackStatus CurrentCallbackStatus;

        /// <summary>
        /// The reward returned.
        /// </summary>
        public readonly Reward Reward;

        /// <summary>
        /// The seconds till next reward if could be calculated from available information.
        /// </summary>
        public readonly int NextAvailableReward;

        /// <summary>
        /// The current Unix time (-1 if not found from the API).
        /// </summary>
        public readonly int CurrentUnixTime;

        /// <summary>
        /// Reward state is complete, and a reward was returned.
        /// </summary>
        public bool Success => CurrentCallbackStatus == RewardCallbackStatus.COMPLETE;

        /// <summary>
        /// Unable to get a reward for any reason
        /// Check Current State for a more specific reason.
        /// </summary>
        public bool Failure => CurrentCallbackStatus != RewardCallbackStatus.PENDING && CurrentCallbackStatus != RewardCallbackStatus.COMPLETE;

        /// <summary>
        /// Create a new callback, indicating the state of getting a new reward.
        /// </summary>
        /// <param name="currentCallbackStatus">Current process state.</param>
        /// <param name="reward">Reward if possible.</param>
        /// <param name="currentUnixTime">The current Unix Time </param>
        /// <param name="nextAvailableReward">the time the next reward is available</param>
        public RewardCallback(RewardCallbackStatus currentCallbackStatus, Reward reward, int currentUnixTime = -1, int nextAvailableReward = -1)
        {
            CurrentCallbackStatus = currentCallbackStatus;
            NextAvailableReward = nextAvailableReward;
            CurrentUnixTime = currentUnixTime;
            Reward = reward;
        }
    }
}