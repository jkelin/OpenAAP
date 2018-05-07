using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OpenAAP.Services.PasswordHashing
{
    public class SHA1PasswordHashingService : IPasswordHashingService
    {
        public async Task<byte[]> Hash(byte[] password, byte[] salt, PasswordAuthenticationHashAlgorithm algorithm)
        {
            using (var stream = new MemoryStream(password.Length + salt.Length))
            {
                await stream.WriteAsync(password, 0, password.Length);
                await stream.WriteAsync(salt, 0, salt.Length);

                stream.Seek(0, SeekOrigin.Begin);

                using (var sha = new SHA1CryptoServiceProvider())
                {
                    return sha.ComputeHash(stream);
                }
            }
        }
    }
}
