using OpenAAP.Services.PasswordHashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAAP.Context
{
    public class Seeder
    {
        public static readonly Identity IdentityNormal = new Identity
        {
            Id = Guid.NewGuid(),
            Description = "normal user with one valid and one disabled password auth",
            Email = "normal@example.com",
            UserName = "normal"
        };

        public static readonly Identity IdentitySingle = new Identity
        {
            Id = Guid.NewGuid(),
            Description = "user with just one enable password auth",
            Email = "single@example.com",
            UserName = "single"
        };

        public static readonly Identity IdentityNone = new Identity
        {
            Id = Guid.NewGuid(),
            Description = "user with no passwords",
            Email = "none@example.com",
            UserName = "none"
        };

        public static readonly Identity IdentityNone2 = new Identity
        {
            Id = Guid.NewGuid(),
            Description = null,
            Email = "none2@example.com",
            UserName = "none"
        };

        public static readonly Identity[] Identites = new Identity[]
        {
            IdentityNormal,
            IdentitySingle,
            IdentityNone,
            IdentityNone2
        };

        /// <summary>
        /// Password: quick brown fox
        /// Salt: abcd
        /// </summary>
        public static readonly PasswordAuthentication PWNormal1 = new PasswordAuthentication
        {
            Id = Guid.NewGuid(),
            IdentityId = IdentityNormal.Id,
            Algorithm = PasswordAuthenticationHashAlgorithm.SHA1,
            Hash = new byte[] { 157, 59, 235, 120, 236, 113, 126, 90, 189, 167, 62, 13, 114, 113, 143, 62, 145, 206, 100, 198 },
            Salt = new byte[] { 97, 98, 99, 100 },
            CreatedAt = new DateTime(2009, 8, 25),
        };

        /// <summary>
        /// Password: 1234567890
        /// Salt: abcd
        /// </summary>
        public static readonly PasswordAuthentication PWNormal2 = new PasswordAuthentication
        {
            Id = Guid.NewGuid(),
            IdentityId = IdentityNormal.Id,
            Algorithm = PasswordAuthenticationHashAlgorithm.SHA1,
            Hash = new byte[] { 76, 202, 24, 149, 73, 206, 42, 36, 32, 185, 253, 25, 62, 157, 47, 185, 165, 166, 133, 8 },
            Salt = new byte[] { 97, 98, 99, 100 },
            CreatedAt = new DateTime(2009, 7, 25),
            DisabledAt = new DateTime(2009, 8, 25),
        };

        /// <summary>
        /// Password: xyz
        /// Salt: abcd
        /// </summary>
        public static readonly PasswordAuthentication PWSingle = new PasswordAuthentication
        {
            Id = Guid.NewGuid(),
            IdentityId = IdentitySingle.Id,
            Algorithm = PasswordAuthenticationHashAlgorithm.SHA1,
            Hash = new byte[] { 48, 23, 125, 154, 168, 211, 166, 201, 16, 50, 180, 72, 99, 131, 68, 187, 44, 121, 125, 200 },
            Salt = new byte[] { 97, 98, 99, 100 },
            CreatedAt = new DateTime(2009, 7, 25),
        };

        public static readonly PasswordAuthentication[] PasswordAuths = new PasswordAuthentication[]
        {
            PWNormal1,
            PWNormal2,
            PWSingle
        };
    }
}
