using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.DailyRewards.Demo
{
    /// <summary>
    /// A test timer, if put on an object with a mono behaviour and enabled it overwrites itself
    ///  as the timer, allowing you to speed through and check how your rewarding system works.
    /// </summary>
    public class TestTimeRetriever : MonoBehaviour, ITimeRetriever
    {

        /// <summary>
        /// Dummy start Time. if 0 will be set to current unix time
        /// </summary>
        [SerializeField]
        private int currentTime;


        /// <summary>
        /// How much faster than real-time the reward system should go.
        /// </summary>
        [SerializeField]
        private float timeSpeedup = 1;

        /// <summary>
        /// The input field that takes the new time.
        /// </summary>
        [SerializeField]
        private InputField timeInput;


        /// <summary>
        /// List of rewards to attach the test timer to.
        /// </summary>
        [SerializeField]
        private DailyReward[] toOverride = { };

        /// <summary>
        /// used to keep calculations precise between ints and floats.
        /// </summary>
        private float _preciseTimeDelta;

        /// <summary>
        ///     Set itself as as the time validator for a daily reward.
        /// </summary>
        private void Start()
        {
            int testTimeOnFinish = PlayerPrefs.GetInt("lastTestTime", 0);
            int lastRunTime = PlayerPrefs.GetInt("lastStoppedTime", 0);

            if (timeInput != null)
                timeInput.onValueChanged.AddListener(value =>
                {
                    if (float.TryParse(value, out float floatInput))
                        timeSpeedup = floatInput;
                });

            currentTime = UnixHelper.GetCurrentUnixTime();
            currentTime = testTimeOnFinish == 0 ? currentTime : testTimeOnFinish + lastRunTime - currentTime;
            
            foreach (DailyReward dailyReward in toOverride)
                dailyReward.RewardInstance.OverrideTimeRetriever(this);
        }

        /// <summary>
        /// Increase the current time by the speedup value * delta time
        /// </summary>
        public void Update()
        {
            _preciseTimeDelta += Time.deltaTime * timeSpeedup;
            if (_preciseTimeDelta < 1)
                return;
            currentTime += Mathf.FloorToInt(_preciseTimeDelta);
            _preciseTimeDelta %= 1;
        }

        /// <summary>
        /// Store values so it continues correctly on restart.
        /// </summary>
        private void OnDestroy()
        {
            PlayerPrefs.SetInt("lastTestTime", currentTime);
            PlayerPrefs.SetInt("lastStoppedTime", UnixHelper.GetCurrentUnixTime());

        }

        /// <summary>
        /// Return the test time.
        /// </summary>
        /// <param name="timeCallback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void TryGetTime(Action<TimeCallback> timeCallback)
        {
            timeCallback?.Invoke(new TimeCallback(TimeCallbackState.SUCCESS, currentTime));
        }
    }
}