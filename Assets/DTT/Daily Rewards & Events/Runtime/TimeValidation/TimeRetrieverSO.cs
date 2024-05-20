using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DTT.DailyRewards
{
    /// <summary>
    /// A default setup of a time validator, can check either local time or remote time.
    /// local time can be "hacked" by changing the systems clock. but will never fail.
    /// Remote time is more secure/you can control, but if the remote server is not reachable
    /// users will not be able to retrieve rewards.
    /// </summary>
    [CreateAssetMenu(fileName = "TimeRetriever", menuName = "DailyRewards/TimeRetriever", order = 0)]
    public class TimeRetrieverSO : ScriptableObject, ITimeRetriever
    {
        /// <summary>
        /// Use local or remote time.
        /// </summary>
        [SerializeField]
        protected TimeRetrieverSetup timeRetrieverSetup = TimeRetrieverSetup.LOCAL;

        /// <summary>
        /// The local time retriever.
        /// </summary>
        protected readonly SystemTimeRetriever LocalTime = new SystemTimeRetriever();

        /// <summary>
        /// The remote time retriever.
        /// </summary>
        private RemoteTimeRetriever _remoteTime;

        /// <summary>
        /// The remote time retriever, will create on first get.
        /// </summary>
        [NotNull]
        protected RemoteTimeRetriever RemoteTime => _remoteTime ?? (_remoteTime = new RemoteTimeRetriever(customAPIUrl, isJson ? jsonKey : null,cacheRemoteCall ? cacheTimeoutInSeconds : -1));

        /// <summary>
        /// The url of the  custom API.
        /// </summary>
        [SerializeField]
        protected string customAPIUrl;

        /// <summary>
        /// If the remote call is in JSON form.
        /// </summary>
        [SerializeField]
        protected bool isJson;

        /// <summary>
        /// If in JSON form, what key will the unix time be under.
        /// </summary>
        [SerializeField]
        protected string jsonKey;

        /// <summary>
        /// Cache the result for a certain time to stop the API from being overwhelmed.
        /// Will continue to count with 
        /// </summary>
        [SerializeField]
        protected bool cacheRemoteCall;

        /// <summary>
        /// How long until the cache times out and requires a new time from the API to accept.
        /// </summary>
        [SerializeField]
        protected int cacheTimeoutInSeconds;

        /// <summary>
        /// Do you allow fallback from remote to local time if the remote time getter fails.
        /// </summary>
        [SerializeField]
        protected FallbackPolicy fallbackPolicy;

        /// <summary>
        /// Attempts to get the time, if available, based on the fallback policy.
        /// </summary>
        /// <param name="timeCallback">Callback action on whether getting the time has succeeded or not.</param>
        public void TryGetTime(Action<TimeCallback> timeCallback)
        {
            if (timeCallback == null)
            {
                Debug.LogWarning("GetTime called with no callback,so has no purpose");
                return;
            }
            //Send pending callback, for a loading icon if wanted.
            TimeCallback pendingCallback = new TimeCallback(TimeCallbackState.PENDING, null);
            timeCallback?.Invoke(pendingCallback);

            //Retrieve the current time
            switch (timeRetrieverSetup)
            {
                //If local just get the local time (should never fail).
                case TimeRetrieverSetup.LOCAL:
                    {
                        GetLocalTime(result =>
                        {
                            timeCallback.Invoke((result));
                        });
                        return;
                    }
                //If remote, attempt to get the remote time.
                case TimeRetrieverSetup.REMOTE:
                    {
                        GetRemoteTime(result =>
                        {
                            //if we fail and can fallback, use local time instead.
                            if (result.State == TimeCallbackState.FAILURE && fallbackPolicy == FallbackPolicy.ALLOW_LOCAL)
                            {
                                GetLocalTime(fallbackResult =>
                                {
                                    timeCallback?.Invoke(fallbackResult);
                                });
                            }
                            else
                            {
                                timeCallback?.Invoke(result);
                            }
                        });
                        return;
                    }
                default:
                    TimeCallback failureCallback = new TimeCallback(TimeCallbackState.FAILURE, null);
                    timeCallback?.Invoke(failureCallback);
                    break;
            }
        }


        /// <summary>
        /// Return the local time (will never fail).
        /// </summary>
        /// <param name="callback">The resulting unixTime as a callback.</param>
        protected void GetLocalTime(Action<TimeCallback> callback)
        {
            LocalTime.Retrieve(result =>
            {
                TimeCallback successCallback = new TimeCallback(TimeCallbackState.SUCCESS, result);
                callback?.Invoke(successCallback);
            });
        }

        /// <summary>
        /// Attempt to get the unix timestamp from a remote API.
        /// </summary>
        /// <param name="callback">The callback of the received time if successful.</param>
        protected void GetRemoteTime(Action<TimeCallback> callback)
        {
            RemoteTime.Retrieve(result =>
            {
                TimeCallbackState timeCallbackState =
                    result == null ? TimeCallbackState.FAILURE : TimeCallbackState.SUCCESS;
                TimeCallback timeCallback = new TimeCallback(timeCallbackState, result);
                callback?.Invoke(timeCallback);
            });
        }
    }
}