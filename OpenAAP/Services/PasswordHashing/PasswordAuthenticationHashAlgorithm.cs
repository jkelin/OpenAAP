namespace OpenAAP.Services.PasswordHashing
{
    public enum PasswordAuthenticationHashAlgorithm
    {
        /// <summary>
        /// SHA1 used for debugging and testing purposes
        /// DO NOT USE THIS IN PRODUCTION
        /// </summary>
        /// <code>
        /// SHA1(concat(password, salt))
        /// </code>
        SHA1,

        /// <summary>
        /// Scrypt algorithm
        /// USE THIS IN PRODUCTION
        /// </summary>
        /// <code>
        /// Scrypt(concat(password, salt), output_size: 64, N: 16384, r: 8, p: 1)
        /// </code>
        Scrypt
    }
}
