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
        /// PBKDF2 algorithm
        /// USE THIS IN PRODUCTION
        /// </summary>
        /// <code>
        /// PBKDF2(password, salt, HMACSHA256, iterationCount: 65536, outputLength: 32)
        /// </code>
        PBKDF2
    }
}
