namespace OpenAAP.Options
{
    public enum SessionStoreType
    {
        InMemory,
        Redis
    }

    public class SessionOptions
    {
        public ulong SessionExpirationMs { get; set; } = 30 * 60 * 1000; // 30 minutes

        public SessionStoreType? SessionStoreType { get; set; } = Options.SessionStoreType.InMemory;

        public string RedisConnectionString { get; set; }
    }
}
