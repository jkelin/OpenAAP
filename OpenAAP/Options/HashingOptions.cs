using OpenAAP.Services.PasswordHashing;

namespace OpenAAP.Options
{
    public class HashingOptions
    {
        public PasswordAuthenticationHashAlgorithm TargetAlgorithm { get; set; } = PasswordAuthenticationHashAlgorithm.Scrypt;
        public int SaltLength { get; set; } = 32;
    }
}
