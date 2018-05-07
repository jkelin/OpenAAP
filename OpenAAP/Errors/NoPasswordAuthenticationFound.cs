namespace OpenAAP.Errors
{
    /// <summary>
    /// There is no password authentication in the database associated with this identity
    /// This is most likely caused by not yet configuring password for this identity
    /// </summary>
    public class NoPasswordAuthenticationFound : IError
    {
        public string Name { get; } = "NO_PASSWORD_AUTHENTICATION_FOUND";
        public string Message { get; } = "No password authentication has been found";
    }
}
