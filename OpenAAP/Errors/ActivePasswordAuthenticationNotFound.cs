namespace OpenAAP.Errors
{
    /// <summary>
    /// This happens when there are password authentications in the database but none are active
    /// For example: when password is configured, then disabled but no new password is configured
    /// </summary>
    public class ActivePasswordAuthenticationNotFound : IError
    {
        public string Name { get; } = "ACTIVE_PASSWORD_AUTHENTICATION_NOT_FOUND";
        public string Message { get; } = "Active password authentication has not been found";
    }
}
