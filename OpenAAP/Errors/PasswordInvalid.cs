namespace OpenAAP.Errors
{
    public class PasswordInvalid : IError
    {
        public string Name { get; } = "PASSWORD_INVALID";
        public string Message { get; } = "Password invalid";
    }
}
