using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.DailyRewards
{
    /// <summary>
    /// The current load state of the daily reward.
    /// </summary>
    public enum LoadState
    {
        /// <summary>
        /// The daily reward  has not yet been loaded.
        /// </summary>
        NOT_LOADED = 1,
        /// <summary>
        /// The daily reward is in the process of loading.
        /// </summary>
        LOADING = 2,
        /// <summary>
        /// The daily reward has been successfully loaded.
        /// </summary>
        LOADED = 3,
        /// <summary>
        /// The daily reward has failed to load at least once.
        /// </summary>
        FAILED = 4,
    }

    /// <summary>
    /// The policy for when a reward is missed.
    /// Wait = Keep the next reward available until the user claims it.
    /// Skip = Go to the reward they would have earned if they had claimed all rewards immediately
    /// Restart = Start the reward count back to 0 (but do NOT clear old earned rewards).
    /// </summary>
    public enum OnRewardMissedPolicy
    {
        WAIT = 1,
        SKIP = 2,
        RESTART = 3,
    }

    /// <summary>
    /// The daily reward scriptable object, allows for customisation of a re-occurring reward.
    /// </summary>
    [CreateAssetMenu(fileName = "Daily Reward", menuName = "Daily Rewards/Daily Reward", order = 0)]
    public class DailyRewardSO : ScriptableObject
    {
        /// <summary>
        /// Name of the reward, each reward must have a unique name
        /// </summary>
        [SerializeField]
        [Tooltip("Each Reward must have a unique name, and must not be changed once live.")]
        private string _rewardId = "exampleReward";

        /// <summary>
        /// Defines the time intervals the reward will be given at
        /// </summary>
        [SerializeField]
        [Tooltip("The time pattern controlling how often this reward can be claimed.")]
        private TimePatternSO _timePattern;

        /// <summary>
        /// Use this if you want to set the claiming time to midnight
        /// This means that the rewards are truly "daily".
        /// </summary>
        [SerializeField]
        [Tooltip("Set to true if you want a reward to become available each day")]
        private bool _rewardsClaimedAtMidnight;

        /// <summary>
        /// If the reward can only be claimed a certain amount of times.
        /// </summary>
        [SerializeField]
        private bool _isFinite;

        /// <summary>
        /// The amount of finite rewards available.
        /// </summary>
        [SerializeField]
        [Tooltip("The number of rewards available, ")]
        private int _finiteRewardAmount;

        /// <summary>
        /// The policy when a reward is not selected in it's time period.
        /// e.g. Another reward would be available.
        /// </summary>
        [SerializeField]
        private OnRewardMissedPolicy _onRewardMissedPolicy = OnRewardMissedPolicy.WAIT;

        /// <summary>
        /// The time validator Scriptable object (can be overwritten by giving a custom one
        /// at initialisation.
        /// </summary>
        [SerializeField]
        private TimeRetrieverSO _timeRetrieverSO;

        /// <summary>
        /// Allows custom time retrieving from to verify it's true.
        /// </summary>
        private ITimeRetriever _timeRetriever;

        /// <summary>
        /// A list of all rewards that have been earned in order.
        /// </summary>
        private List<Reward> _earnedRewards;

        /// <summary>
        /// The system that saves and loads the rewards,
        /// replacing with a custom system.
        /// </summary>
        private IRewardSaver _rewardSaver;

        /// <summary>
        /// The current loadState of the reward.
        /// </summary>
        public LoadState LoadState { get; private set; } = LoadState.NOT_LOADED;

        /// <summary>
        /// If the rewards are loaded.
        /// </summary>
        public bool Loaded => LoadState == LoadState.LOADED;

        /// <summary>
        /// Initialise the daily reward saver.
        /// Must be done before calling any other methods.
        /// </summary>
        /// <param name="loadingProgress">Callback with progress,(called multiple times).</param>
        /// <param name="customSaver">A custom saving system for rewards.</param>
        /// <param name="customRetriever">Custom time validator to override the one used above.</param>
        public void Initialise(Action<LoadState> loadingProgress, IRewardSaver customSaver = null, ITimeRetriever customRetriever = null)
        {
            _earnedRewards = null;

            LoadState = LoadState.LOADING;
            loadingProgress?.Invoke(LoadState);

            if (customRetriever != null)
                _timeRetriever = customRetriever;
            else if (_timeRetriever == null)
                _timeRetriever = _timeRetrieverSO;

            //try load
            try
            {
                if (_timeRetriever == null)
                {
                    Debug.LogError($"No time validator given to {_rewardId}");
                    throw new NullReferenceException("_timeValidator");
                }

                _rewardSaver = customSaver ?? new PlayerPrefsRewardSaver(_rewardId);

                _rewardSaver.LoadRewards(loadRewardCallback =>
                {
                    if (loadRewardCallback.Success)
                    {
                        _earnedRewards = loadRewardCallback.Rewards.ToList();
                        LoadState = LoadState.LOADED;
                    }
                    else
                    {
                        LoadState = LoadState.FAILED;
                    }

                    loadingProgress?.Invoke(LoadState);
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load daily reward {_rewardId}");
                Debug.LogException(e);
                loadingProgress?.Invoke(LoadState.FAILED);
            }
        }

        /// <summary>
        /// Override the current time validator with a custom one for more complex
        /// time validation.
        /// </summary>
        /// <param name="customTimeRetriever">the custom time validator</param>
        public void OverrideTimeRetriever(ITimeRetriever customTimeRetriever)
        {
            if (customTimeRetriever != null)
                _timeRetriever = customTimeRetriever;
        }

        /// <summary>
        /// Check if there is any reward available.
        /// </summary>
        /// <param name="result">A callback with the state of the request, and the reward if found</param>
        public void CheckAvailableReward(Action<RewardCallback> result)
        {
            //check if we have a finite amount and have exceeded that value?
            if (_isFinite && _earnedRewards != null && _earnedRewards.Count > 0 && _earnedRewards.Count >= _finiteRewardAmount)
            {
                result?.Invoke(new RewardCallback(RewardCallbackStatus.ALL_REWARDS_EARNED, null));
                return;
            }

            //Get the time from the validator
            _timeRetriever.TryGetTime(timeCallback =>
            {
                switch (timeCallback.State)
                {

                    case TimeCallbackState.PENDING:
                        result?.Invoke(new RewardCallback(RewardCallbackStatus.PENDING, null));
                        break;
                    case TimeCallbackState.SUCCESS:
                        if (timeCallback.UnixTime == null)
                        {
                            result?.Invoke(new RewardCallback(RewardCallbackStatus.COULD_NOT_VALIDATE_TIME, null));
                            return;
                        }
                        int time = timeCallback.UnixTime ?? 0;
                        Reward lastReward = _earnedRewards != null && _earnedRewards.Count > 0 ? _earnedRewards.Last() : null;
                        Reward nextReward = GetNextReward(time, lastReward);
                        if (nextReward.RewardStatus == RewardStatus.AVAILABLE)
                        {
                            RewardCallback rewardCallback = new RewardCallback(RewardCallbackStatus.COMPLETE, nextReward,time);
                            result?.Invoke(rewardCallback);
                        }
                        else if (lastReward != null)
                        {
                            int nextRewardAvailableAt = _timePattern.GetNextTime(lastReward.ClaimedAt, lastReward.RewardCount);
                            RewardCallback notAvailableYetCallback = new RewardCallback(RewardCallbackStatus.NO_REWARD_AVAILABLE,
                                nextReward,time, nextRewardAvailableAt);
                            result?.Invoke(notAvailableYetCallback);
                        }
                        else
                        {
                            //should never happen, but if the last reward is null somehow, then this will not break,
                            // but the time till claiming will not be filled.
                            RewardCallback notAvailableYetCallback = new RewardCallback(RewardCallbackStatus.NO_REWARD_AVAILABLE,
                                null);
                            result?.Invoke(notAvailableYetCallback);
                        }
                        break;
                    case TimeCallbackState.FAILURE:
                        result?.Invoke(new RewardCallback(RewardCallbackStatus.COULD_NOT_VALIDATE_TIME, null));
                        break;
                    default:
                        throw new NotSupportedException($"{timeCallback.State} is not supported");
                }
            });
        }

        /// <summary>
        /// Given the current time and last reward earned, get the next reward to give.
        /// </summary>
        /// <param name="currentUnixTime">The current unix time.</param>
        /// <param name="lastReward">The last reward earned (can be null).</param>
        /// <returns>The next available reward if possible, if not available, then null.</returns>
        private Reward GetNextReward(int currentUnixTime, Reward lastReward)
        {
            Reward nextReward;
            int awardCount = 0;

            if (lastReward == null)
            {
                nextReward = new Reward(RewardStatus.AVAILABLE, currentUnixTime, -1, _rewardId, 0);
                return nextReward; 
            }

            int nextRewardAvailableAt = _timePattern.GetNextTime(lastReward.ClaimedAt, lastReward.RewardCount);
            
            bool available = currentUnixTime >= nextRewardAvailableAt;
            if (available)
            {
                int nextRewardCount = lastReward.RewardCount + 1;
                //The value it could be if every reward was claimed at the moment it became available.
                int possibleNextRewardCount = _timePattern.GetNextRewardCount(lastReward.ClaimedAt, currentUnixTime,
                    lastReward.RewardCount);

                switch (_onRewardMissedPolicy)
                {
                    case OnRewardMissedPolicy.WAIT:
                        awardCount = nextRewardCount;
                        break;
                    case OnRewardMissedPolicy.SKIP:
                        awardCount = possibleNextRewardCount;
                        break;
                    case OnRewardMissedPolicy.RESTART:
                        awardCount = possibleNextRewardCount > nextRewardCount ? 0 : possibleNextRewardCount;
                        break;
                    default:
                        throw new NotSupportedException($"{_onRewardMissedPolicy} enum is not supported");
                }
            }
            nextReward = new Reward(available ? RewardStatus.AVAILABLE : RewardStatus.UNAVAILABLE, nextRewardAvailableAt, -1,_rewardId, awardCount);

            return nextReward;
        }

        /// <summary>
        /// Get the time the reward is saved to be claimed at (can be different to actual time based on
        /// settings).
        /// </summary>
        /// <param name="currentUnixTime">The current unix time.</param>
        /// <returns>The time that the next reward should be set to be claimed at.</returns>
        private int GetClaimedTime(int currentUnixTime)
        {
            if (!_rewardsClaimedAtMidnight)
                return currentUnixTime;

            //If must be claimed at midnight, then set to midnight
            DateTime currentDate = UnixHelper.ConvertUnixTimeToLocalTime(currentUnixTime);
            //remove minutes and hours.
            DateTime midnightDate = currentDate.Date;
            //reconvert to unix time.
            currentUnixTime = UnixHelper.ConvertTimeToUnix(midnightDate);
            return currentUnixTime;
        }

        /// <summary>
        /// Attempts to claim any available reward, and saves it to history.
        /// Only counted as successful if able to save.
        /// </summary>
        /// <param name="result">The result of the claim.</param>
        public void ClaimReward(Action<RewardCallback> result)
        {
            if (!Loaded)
            {
                Debug.LogError($"Attempted to claim a reward before the reward {_rewardId} was fully loaded");
                result?.Invoke(new RewardCallback(RewardCallbackStatus.REWARDS_NOT_LOADED,null));
            }
            //get if any reward is available
            CheckAvailableReward(rewardToClaim =>
            {
                if (rewardToClaim.Success && rewardToClaim.Reward.RewardStatus == RewardStatus.AVAILABLE)
                {
                    rewardToClaim.Reward.Claim(GetClaimedTime(rewardToClaim.CurrentUnixTime));

                    //attempt to save the reward.
                    SaveClaimedReward(rewardToClaim.Reward, saveResult =>
                    {
                        result?.Invoke(!saveResult ? new RewardCallback(RewardCallbackStatus.COULD_NOT_SAVE_REWARD, null) : rewardToClaim);
                    });
                }
                else
                {
                    result?.Invoke(rewardToClaim);
                }
            });
        }

        /// <summary>
        /// Get the wait time between the previous claimed reward and it's next one.
        /// </summary>
        /// <returns>The time period of the last claimed rewards.</returns>
        public int CurrentRewardWaitTime()
        {
            int lastCount = _earnedRewards != null && _earnedRewards.Count > 0 ? _earnedRewards.Last().RewardCount : 0;

            return _timePattern.GetNextTimePeriod(lastCount);
        }

        /// <summary>
        /// When a reward is claimed, save it to make sure we don't claim it twice.
        /// </summary>
        /// <param name="reward">Reward to be saved.</param>
        /// <param name="saved">Callback indicating if the reward was correctly saved.</param>
        private void SaveClaimedReward(Reward reward, Action<bool> saved)
        {
            //Add it to the list.
            if (_earnedRewards.Contains(reward))
            {
                saved?.Invoke(true);
                return;
            }

            _earnedRewards.Add(reward);
            _rewardSaver.StoreRewards(_earnedRewards, saved);
        }

        /// <summary>
        /// Get the current streak of the reward.
        /// </summary>
        /// <returns>The number of rewards claimed in a row.</returns>
        public int CurrentStreak()
        {
            if (_earnedRewards == null)
            {
                Debug.LogWarning("Streak retrieved before loading");
                return 0;
            }

            int streak = 0;
            int lastRewardValue = -1;

            for (int i = _earnedRewards.Count - 1; i >= 0; i--)
            {
                //start counting if at -1.
                if (lastRewardValue == -1)
                    streak++;
                //check it's one consecutive to the last value
                else if (_earnedRewards[i].RewardCount == lastRewardValue - 1)
                    streak++;
                //break if not (hit end of streak;
                else
                    break;
                lastRewardValue = _earnedRewards[i].RewardCount;
            }

            return streak;
        }

        /// <summary>
        /// Returns the last N rewards received where N is less than or equal to <see cref="maxCount"/>.
        /// </summary>
        /// <param name="maxCount">The maximum number of previous rewards to return, set to -1 to return all .</param>
        /// <returns>The previously received rewards.</returns>
        public Reward[] EarnedRewards(int maxCount = -1)
        {
            if (maxCount < 0)
                return _earnedRewards.ToArray();

            return _earnedRewards.Skip(Math.Max(0, _earnedRewards.Count - maxCount)).ToArray();
        }

        /// <summary>
        /// Resets this reward as if it had never been claimed.
        /// WARNING, lose all reward progress/information.
        /// </summary>
        /// <param name="successful">True if successful (may fail if using a custom reward saver is used).</param>
        public void ResetRewards(Action<bool> successful) => _rewardSaver.ClearRewards(successful);
    }
}