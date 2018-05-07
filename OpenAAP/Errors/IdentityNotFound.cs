namespace OpenAAP.Errors
{
    public class IdentityNotFound : IError
    {
        public string Name { get; } = "IDENTITY_NOT_FOUND";
        public string Message { get; } = "Identity has not been found";
    }
}
