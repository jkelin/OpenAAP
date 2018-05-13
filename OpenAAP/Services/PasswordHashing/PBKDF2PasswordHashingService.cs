using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public Task<byte[]> Hash(byte[] password, byte[] salt, PasswordAuthenticationHashAlgorithm algorithm)
        {
            return Task.Factory.StartNew(() =>
                KeyDerivation.Pbkdf2(
                    password: BitConverter.ToString(password),
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 1024,
                    numBytesRequested: 32)
            );
        }
    }
}
