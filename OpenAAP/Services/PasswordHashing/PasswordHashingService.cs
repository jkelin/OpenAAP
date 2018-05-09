using OpenAAP.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly SHA1PasswordHashingService sha1;
        private readonly PBKDF2PasswordHashingService pbkdf2;

        public PasswordHashingService(
            SHA1PasswordHashingService sha1,
            PBKDF2PasswordHashingService scrypt
        )
        {
            this.sha1 = sha1;
            this.pbkdf2 = scrypt;
        }

        public Task<byte[]> GenerateSalt(int length)
        {
            var salt = new byte[length];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return Task.FromResult(salt);
        }

        public Task<byte[]> Hash(byte[] password, byte[] salt, PasswordAuthenticationHashAlgorithm algo)
        {
            switch (algo)
            {
                case PasswordAuthenticationHashAlgorithm.SHA1:
                    return sha1.Hash(password, salt, algo);
                case PasswordAuthenticationHashAlgorithm.PBKDF2:
                    return pbkdf2.Hash(password, salt, algo);
                default:
                    throw new NotImplementedException($"Hashing algorighm {algo} not yet implemented");
            }
        }
    }
}
