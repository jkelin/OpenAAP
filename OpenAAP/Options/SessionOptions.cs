namespace OpenAAP.Options
{
    public class SessionOptions
    {
        public ulong SessionExpirationMs { get; set; } = 30 * 60 * 1000; // 30 minutes
    }
}
