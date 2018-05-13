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
            PBKDF2PasswordHashingService pbkdf2
        )
        {
            this.sha1 = sha1;
            this.pbkdf2 = pbkdf2;
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

        public Task<byte[]> Hash(byte[] password, byte[] salt, TargetHashConfigration options)
        {
            switch (options.Algorithm ?? throw new InvalidOperationException($"{nameof(options.Algorithm)} required for hashing passwords"))
            {
                case HashingAlgorithm.SHA1:
                    return sha1.Hash(password, salt, options);
                case HashingAlgorithm.PBKDF2:
                    return pbkdf2.Hash(password, salt, options);
                default:
                    throw new NotImplementedException($"Hashing algorithm {options.Algorithm} not yet implemented");
            }
        }
    }
}
