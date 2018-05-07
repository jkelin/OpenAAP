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

        public PasswordHashingService(SHA1PasswordHashingService sha1)
        {
            this.sha1 = sha1;
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
                default:
                    throw new NotImplementedException($"Hashing algorighm {algo} not yet implemented");
            }
        }
    }
}
