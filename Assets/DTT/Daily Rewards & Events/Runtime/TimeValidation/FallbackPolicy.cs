namespace DTT.DailyRewards
{
    /// <summary>
    /// The fallback policy, e.g. should you use local if remote fails or not.
    /// </summary>
    public enum FallbackPolicy
    {
        ALLOW_LOCAL = 0,
        NO_FALLBACK = 1,
    }
}