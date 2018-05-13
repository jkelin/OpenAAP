using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OpenAAP.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    public class PBKDF2PasswordHashingService : IPasswordHashingService
    {
        private KeyDerivationPrf Prf(TargetHashConfigration options)
        {
            switch (options.Algorithm2 ?? throw new InvalidOperationException($"{nameof(options.IterationCount)} required for PBKDF2"))
            {
                case HashingAlgorithm.SHA1:
                    return KeyDerivationPrf.HMACSHA1;
                case HashingAlgorithm.SHA256:
                    return KeyDerivationPrf.HMACSHA256;
                case HashingAlgorithm.SHA512:
                    return KeyDerivationPrf.HMACSHA512;
                default:
                    throw new IndexOutOfRangeException($"{options.Algorithm2.Value} is unsupported Algorithm2 for PBKDF2");
            }
        }

        public Task<byte[]> Hash(byte[] password, byte[] salt, TargetHashConfigration options)
        {
            return Task.Factory.StartNew(() =>
                KeyDerivation.Pbkdf2(
                    password: BitConverter.ToString(password),
                    salt: salt,
                    prf: Prf(options),
                    iterationCount: options.IterationCount ?? throw new InvalidOperationException($"{nameof(options.IterationCount)} required for PBKDF2"),
                    numBytesRequested: options.PasswordHashBytes ?? throw new InvalidOperationException($"{nameof(options.PasswordHashBytes)} required for PBKDF2"))
            );
        }
    }
}
