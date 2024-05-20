using System;

namespace DTT.DailyRewards
{
    /// <summary>
    /// A set of methods that allow converting to and from unix timestamps to <see cref="DateTime"/>.
    /// </summary>
    public static class UnixHelper
    {
        /// <summary>
        /// The current unix time.
        /// </summary>
        /// <returns>An integer of the current local unix time.</returns>
        public static int GetCurrentUnixTime()
        {
            return ConvertTimeToUnix(DateTime.UtcNow);
        }

        /// <summary>
        /// Converts a datetime to it's unix timestamp.
        /// </summary>
        /// <param name="dateTime">The datetime to convert.</param>
        /// <returns>The unix timestamp.</returns>
        public static int ConvertTimeToUnix(DateTime dateTime)
        {
            int unixTimestamp = (int)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        /// <summary>
        /// Given a unix timestamp, return the corresponding local datetime.
        /// </summary>
        /// <param name="unixTimeStamp">A unix timestamp in seconds.</param>
        /// <returns>The local datetime.</returns>
        public static DateTime ConvertUnixTimeToLocalTime(int unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}