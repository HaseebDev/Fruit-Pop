#if TEST_FRAMEWORK

using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace DTT.DailyRewards.Tests
{
    /// <summary>
    /// Testing of the time pattern SO to check it correctly calculates
    /// next times in time patterns.
    /// </summary>
    public class TestRuntimeTimePatternSO
    {
        /// <summary>
        /// A One Minute Time pattern.
        /// </summary>
        private TimePatternSO _timePatternOne;

        /// <summary>
        /// A custom time pattern of [1,2,3] minutes, repeating.
        /// </summary>
        private TimePatternSO _timePatternThree;

        /// <summary>
        /// A 10 hours time pattern
        /// </summary>
        private TimePatternSO _timePatternTwo;

        /// <summary>
        /// Load in the time patters.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            //arrange
            _timePatternOne = Resources.Load<TimePatternSO>("TimePatternOne");
            _timePatternTwo = Resources.Load<TimePatternSO>("TimePatternTwo");
            _timePatternThree = Resources.Load<TimePatternSO>("TimePatternThree");
            Debug.Log("Loaded all messages.");
            Debug.Log(_timePatternOne);
        }

        /// <summary>
        /// Test that the time patterns are correctly calculated.
        /// </summary>
        /// <param name="lastTriggeredTime"></param>
        [Test]
        public void Test_runtime_TimePatternSO_GetNextTime([Values(1, 10, 100)] int lastTriggeredTime)
        {
            //Arrange.
            int expectedNextTriggerTimeOne = lastTriggeredTime + 60;
            int expectedNextTriggerTimeTwo = lastTriggeredTime + 36000;
            int expectedNextTriggerTimeThree = lastTriggeredTime + 60;
            int expectedNextTriggerTimeFour = lastTriggeredTime + 180;

            //Act
            int actualNextTriggerTimeOne = _timePatternOne.GetNextTime(lastTriggeredTime, 0);
            int actualNextTriggerTimeTwo = _timePatternTwo.GetNextTime(lastTriggeredTime, 0);
            int actualNextTriggerTimeThree = _timePatternThree.GetNextTime(lastTriggeredTime, 0);
            int actualNextTriggerTimeFour = _timePatternThree.GetNextTime(lastTriggeredTime, 2);

            //assert
            Assert.AreEqual(expectedNextTriggerTimeOne, actualNextTriggerTimeOne);
            Assert.AreEqual(expectedNextTriggerTimeTwo, actualNextTriggerTimeTwo);
            Assert.AreEqual(expectedNextTriggerTimeThree, actualNextTriggerTimeThree);
            Assert.AreEqual(expectedNextTriggerTimeFour, actualNextTriggerTimeFour);

        }

        /// <summary>
        /// Check that the custom pattern gets multiple next times correctly.
        /// </summary>
        /// <param name="lastTriggeredTime"></param>
        [Test]
        public void Test_runtime_TimePatternSO_CustomNextTimes([Values(1, 10, 100, 0)] int lastTriggeredTime)
        {
            int[] expectedNextTimes = new[] { 1, 3, 6, 7, 9, 12, }.Select(x => x * 60).ToArray();
            expectedNextTimes = expectedNextTimes.Select(x => x + lastTriggeredTime).ToArray();

            int[] actualNextTimes = _timePatternThree.GetNextTimes(lastTriggeredTime, 0, 6);

            CollectionAssert.AreEqual(actualNextTimes, expectedNextTimes);
        }
    }
}

#endif