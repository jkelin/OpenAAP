namespace OpenAAP.Options
{
    public enum SessionStoreType
    {
        InMemory,
        Redis
    }

    public class SessionOptions
    {
        public const string Section = "Session";

        public ulong ExpirationMs { get; set; } = 30 * 60 * 1000; // 30 minutes

        public SessionStoreType? SessionStoreType { get; set; } = Options.SessionStoreType.InMemory;

        public string ConnectionStringRedis { get; set; }
    }
}
