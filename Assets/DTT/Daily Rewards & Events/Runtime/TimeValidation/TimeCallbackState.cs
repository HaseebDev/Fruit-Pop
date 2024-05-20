namespace DTT.DailyRewards
{

    /// <summary>
    /// A callback for retrieving the time, and whether the retrieval was successful.
    /// </summary>
    public class TimeCallback
    {
        /// <summary>
        /// The state of the request
        /// </summary>
        public readonly TimeCallbackState State;

        /// <summary>
        /// The unix time if found.
        /// </summary>
        public readonly int? UnixTime;

        /// <summary>
        /// Create a new time callback state.
        /// </summary>
        /// <param name="state">The state of the request</param>
        /// <param name="unixTime">The unix timestamp, if available</param>
        public TimeCallback(TimeCallbackState state, int? unixTime)
        {
            State = state;
            UnixTime = unixTime;
        }
    }

    /// <summary>
    /// the state of a time request to a validator.
    /// </summary>
    public enum TimeCallbackState
    {
        PENDING = 0,
        SUCCESS = 1,
        FAILURE = 2,
    }
}