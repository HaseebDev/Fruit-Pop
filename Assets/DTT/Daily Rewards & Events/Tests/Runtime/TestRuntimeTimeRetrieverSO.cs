#if TEST_FRAMEWORK

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.DailyRewards.Tests
{
    /// <summary>
    /// Tests that the time retrievers work correctly for remote and local test.
    /// </summary>
    public class TestRuntimeTimeRetrieverSO
    {
        /// <summary>
        /// Only uses local time.
        /// </summary>
        private TimeRetrieverSO _localTimeRetriever;

        /// <summary>
        /// Should Successfully use a json key to get a remote value.
        /// </summary>
        private TimeRetrieverSO _remoteTimeRetriever;

        /// <summary>
        /// Should fail to get a remote time, and successfully uses local fallback.
        /// </summary>
        private TimeRetrieverSO _remoteTimeRetrieverFallback;

        /// <summary>
        /// Fails to get remote, does not allow fallback, so always fails.
        /// </summary>
        private TimeRetrieverSO _remoteTimeRetrieverNoFallback;

        /// <summary>
        /// Load in the test TimeRetrievers.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _localTimeRetriever = Resources.Load<TimeRetrieverSO>("LocalTimeValidator");
            _remoteTimeRetriever = Resources.Load<TimeRetrieverSO>("RemoteTimeValidator");
            _remoteTimeRetrieverFallback = Resources.Load<TimeRetrieverSO>("RemoteTimeValidatorFallback");
            _remoteTimeRetrieverNoFallback = Resources.Load<TimeRetrieverSO>("RemoteTimeValidatorNoFallback");
        }

        /// <summary>
        /// Test that the local time retriever works correctly
        /// </summary>
        [Test]
        public void Test_Runtime_TimeValidatorSO_LocalTimeCheck()
        {

            int wantedTime = UnixHelper.GetCurrentUnixTime();
            _localTimeRetriever.TryGetTime(result =>
            {
                if (result.State == TimeCallbackState.PENDING)
                {
                    Assert.IsNull(result.UnixTime);
                }
                else
                {
                    Assert.AreEqual(TimeCallbackState.SUCCESS, result.State);
                    Assert.AreEqual(wantedTime, result.UnixTime);
                }
            });
        }

        /// <summary>
        /// Test that the remote time with JSON key works correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Runtime_TimeValidatorSO_RemoteTimeCheck()
        {
            int wantedTime = UnixHelper.GetCurrentUnixTime();
            TimeCallbackState firstResult = TimeCallbackState.PENDING;
            int? secondResult = 0;
            bool finished = false;
            _remoteTimeRetriever.TryGetTime(result =>
            {
                if (result.State == TimeCallbackState.PENDING)
                {
                    Assert.IsNull(result.UnixTime);
                }
                else
                {
                    Debug.Log(result.UnixTime);
                    firstResult = result.State;
                    secondResult = result.UnixTime;
                    finished = true;
                }
            });

            yield return new WaitUntil(() => finished);
            Assert.AreEqual(TimeCallbackState.SUCCESS, firstResult);
            Assert.AreEqual(wantedTime, secondResult, 5);

        }

        /// <summary>
        /// Test that the remote time retriever successfully falls back to local time.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Runtime_TimeValidatorSO_FallbackAllowed()
        {
            int wantedTime = UnixHelper.GetCurrentUnixTime();
            TimeCallbackState firstResult = TimeCallbackState.PENDING;
            int? timeResult = 0;
            bool finished = false;
            _remoteTimeRetrieverFallback.TryGetTime(result =>
            {
                if (result.State == TimeCallbackState.PENDING)
                {
                    Assert.IsNull(result.UnixTime);
                }
                else
                {
                    firstResult = result.State;
                    timeResult = result.UnixTime;
                    finished = true;
                }
            });

            yield return new WaitUntil(() => finished);
            Assert.AreEqual(TimeCallbackState.SUCCESS, firstResult);
            Assert.AreEqual(wantedTime, timeResult, 2);
        }

        /// <summary>
        /// Test that the remote time retriever fails when no fallback given.
        /// </summary>
        [UnityTest]
        public IEnumerator Test_Runtime_TimeValidatorSO_FallbackNotAllowed()
        {
            int wantedTime = UnixHelper.GetCurrentUnixTime();
            TimeCallbackState firstResult = TimeCallbackState.PENDING;
            int? timeResult = 0;
            bool finished = false;
            _remoteTimeRetrieverNoFallback.TryGetTime(result =>
            {
                if (result.State == TimeCallbackState.PENDING)
                {
                    Assert.IsNull(result.UnixTime);
                }
                else
                {
                    firstResult = result.State;
                    timeResult = result.UnixTime;
                    finished = true;
                }
            });

            yield return new WaitUntil(() => finished);
            Assert.AreEqual(TimeCallbackState.FAILURE, firstResult);
            Assert.IsNull(timeResult);
        }
    }
}

#endif