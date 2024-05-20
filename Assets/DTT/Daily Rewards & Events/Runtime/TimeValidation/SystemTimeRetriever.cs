using System;

namespace DTT.DailyRewards
{
    /// <summary>
    /// Retrieves the system time from the local system.
    /// </summary>
    public class SystemTimeRetriever
    {
        /// <summary>
        /// Return the current unix time.
        /// </summary>
        /// <param name="time">the current UTC local time in unix form</param>
        public void Retrieve(Action<int?> time)
        {
            time?.Invoke(UnixHelper.GetCurrentUnixTime());
        }
    }
}