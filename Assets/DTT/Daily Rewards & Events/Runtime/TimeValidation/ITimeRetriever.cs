using System;

namespace DTT.DailyRewards
{
    /// <summary>
    /// An interface for retrieving a unix time.
    /// </summary>
    public interface ITimeRetriever
    {
        /// <summary>
        /// Try get the current time, if possible.
        /// Some implementations (e.g. remote) may fail.
        /// </summary>
        /// <param name="timeCallback">The time callback.</param>
        void TryGetTime(Action<TimeCallback> timeCallback);

    }
}