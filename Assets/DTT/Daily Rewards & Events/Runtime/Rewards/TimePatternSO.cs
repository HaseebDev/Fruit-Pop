using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace DTT.DailyRewards
{

    /// <summary>
    /// The possible interval units
    /// </summary>
    public enum IntervalUnit
    {
        [InspectorName("Seconds")]
        SECONDS = 0,
        [InspectorName("Minutes")]

        MINUTES = 1,
        [InspectorName("Hours")]

        HOURS = 2,
        [InspectorName("Days")]
        DAYS = 3,
    }

    /// <summary>
    /// A time pattern defines how daily rewards will become available.
    /// </summary>
    [CreateAssetMenu(fileName = "TimePattern", menuName = "DailyRewards/TimePattern", order = 0)]
    public class TimePatternSO : ScriptableObject
    {

        /// <summary>
        /// The number of units between each reward, can be days/hours or minutes.
        /// </summary>
        [SerializeField]
        private int unitsBetweenReward = 1;

        /// <summary>
        /// The number of units between a reward.
        /// </summary>
        public int UnitsBetweenReward => unitsBetweenReward;

        /// <summary>
        /// If a custom pattern is to be used.
        /// </summary>
        [SerializeField]
        private bool useCustomPattern = false;

        /// <summary>
        /// If a custom pattern is being used.
        /// </summary>
        public bool UseCustomPattern => useCustomPattern;

        /// <summary>
        /// The custom pattern to use.
        /// </summary>
        [SerializeField]
        private int[] customPattern = Array.Empty<int>();

        /// <summary>
        /// The custom pattern to use.
        /// </summary>
        public ReadOnlyCollection<int> CustomPattern => Array.AsReadOnly(customPattern);

        /// <summary>
        /// the units of the time pattern (days/minutes/hours)
        /// </summary>
        [SerializeField]
        private IntervalUnit units = IntervalUnit.DAYS;

        /// <summary>
        /// The intervals for the units.
        /// </summary>
        public IntervalUnit Units => units;



        /// <summary>
        /// Get the next time the reward will be available.
        /// </summary>
        /// <param name="lastTriggeredTimeUnix">When it was last claimed.</param>
        /// <param name="triggerCount">The number of times the reward has been triggered before (needed for custom patterns).</param>
        /// <returns>The next time a reward is available.</returns>
        public int GetNextTime(int lastTriggeredTimeUnix, int triggerCount)
        {
            return lastTriggeredTimeUnix + GetNextTimePeriod(triggerCount);
        }

        /// <summary>
        /// Given a trigger count, get how long the next time period is.
        /// (Only differs for custom patterns).
        /// </summary>
        /// <param name="triggerCount">The trigger count.</param>
        /// <returns>How long the next time period is.</returns>
        public int GetNextTimePeriod(int triggerCount)
        {
            if (!useCustomPattern) return GetTimePeriod(unitsBetweenReward, units);

            int currentUnitCount = customPattern[triggerCount % customPattern.Length];
            return GetTimePeriod(currentUnitCount, units);
        }



        /// <summary>
        /// Given a reward count and trigger time, get the next claimable reward (assuming we skip rewards).
        /// </summary>
        /// <param name="lastTriggeredTimeUnix">Last time we got a reward.</param>
        /// <param name="currentTimeUnix">Current unix time.</param>
        /// <param name="lastRewardCount">The count of the last reward (used for custom patterns).</param>
        /// <returns></returns>
        public int GetNextRewardCount(int lastTriggeredTimeUnix, int currentTimeUnix, int lastRewardCount)
        {
            float timePassed = (currentTimeUnix - lastTriggeredTimeUnix);

            if (!useCustomPattern)
            {
                float timePeriod = GetTimePeriod(unitsBetweenReward, units);
                return ((int)Math.Floor(timePassed / timePeriod)) + lastRewardCount;
            }

            int[] timePeriods = ConvertCustomPatternToTimePeriods();

            //We run through the custom pattern as many times as we can in the time passed.


            //Get how many times we have looped round the current time period.
            int singleLoopTime = timePeriods.Sum();
            int loops = (int)Math.Floor(timePassed / singleLoopTime);
            int timeInThisLoop = (int)timePassed % singleLoopTime;

            //Work out how far in the final loop we went.
            int finalLoopCount = 0;
            int customPatternCurrentIndex = lastRewardCount % timePeriods.Length;
            while (timeInThisLoop >= timePeriods[customPatternCurrentIndex])
            {
                timeInThisLoop -= timePeriods[customPatternCurrentIndex];
                customPatternCurrentIndex = (customPatternCurrentIndex + 1) % timePeriods.Length;
                finalLoopCount++;
            }
            //times looped added to how far in the final loop we made it
            return loops * timePeriods.Length + finalLoopCount;
        }

        /// <summary>
        /// Converts the custom pattern in to the time pattern in seconds
        /// </summary>
        /// <returns>An array of time seconds, showing the custom pattern in time</returns>
        private int[] ConvertCustomPatternToTimePeriods()
        {
            if (customPattern == null || customPattern.Length == 0)
            {
                Debug.LogError("Didn't add a custom time period, we instead use a single unit as the custom time period");
                return new[] { GetTimePeriod(1, units) };
            }
            else
            {
                return customPattern.Select(x => GetTimePeriod(x, Units)).ToArray();
            }
        }

        /// <summary>
        /// Gets the next times of the time period based on the last time triggered.
        /// </summary>
        /// <param name="lastTriggeredTimeUnix">The last time triggered.</param>
        /// <param name="triggerCount">The number of times it's been triggered before (needed for custom patterns).</param>
        /// <param name="count">Number of next reward times to return.</param>
        /// <returns>The next times rewards become available (assuming they are claimed at the exact time they become available).</returns>
        public int[] GetNextTimes(int lastTriggeredTimeUnix, int triggerCount, int count)
        {
            int[] nextTimes = new int[count];
            if (useCustomPattern)
            {
                int[] timePeriods = ConvertCustomPatternToTimePeriods();
                int startInterval = triggerCount % timePeriods.Length;
                int lastValue = lastTriggeredTimeUnix;

                for (int i = 0; i < count; i++)
                {
                    lastValue += timePeriods[(startInterval + i) % timePeriods.Length];
                    nextTimes[i] = lastValue;
                }
            }
            else
            {
                int timePeriod = GetTimePeriod(unitsBetweenReward, units);
                for (int i = 0; i < count; i++)
                {
                    nextTimes[i] = lastTriggeredTimeUnix + (i + 1) * timePeriod;
                }
            }

            return nextTimes;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Given the unit count and units, return the seconds.
        /// </summary>
        /// <param name="unitCount">Number of units.</param>
        /// <param name="unitType">The unit type.</param>
        /// <returns>The time period in seconds.</returns>
        private int GetTimePeriod(int unitCount, IntervalUnit unitType)
        {
            int unitValue;
            switch (unitType)
            {
                case IntervalUnit.SECONDS:
                    unitValue = 1;
                    break;
                case IntervalUnit.MINUTES:
                    unitValue = 60;
                    break;
                case IntervalUnit.HOURS:
                    unitValue = 3600;
                    break;
                case IntervalUnit.DAYS:
                    unitValue = 86400;
                    break;
                default:
                    throw new NotSupportedException($"{unitType} enum value is not supported");
            }
            return unitCount * unitValue;
        }
    }
}